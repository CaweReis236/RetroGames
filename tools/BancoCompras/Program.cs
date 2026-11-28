using MySqlConnector;

// Ferramenta administrativa local, separada da API e dos Services.
// Não imprime a string de conexão, e nunca recria tabelas existentes.
if (args.Length != 1 || args[0] is not ("preparar" or "verificar" or "testar"))
{
    Console.Error.WriteLine("Uso: dotnet run --project tools/BancoCompras -- preparar|verificar|testar");
    return 1;
}

try
{
    DotNetEnv.Env.TraversePath().Load();
    var configuracao = Environment.GetEnvironmentVariable("ConnectionStrings__Default");
    if (string.IsNullOrWhiteSpace(configuracao))
        throw new InvalidOperationException("ConnectionStrings__Default não encontrada no .env.");
    var builder = new MySqlConnectionStringBuilder(configuracao);
    if (string.IsNullOrWhiteSpace(builder.Database))
        throw new InvalidOperationException("A conexão não define um banco de dados.");

    await using var conexao = new MySqlConnection(configuracao);
    await conexao.OpenAsync();
    Console.WriteLine("[OK] Conexão com o banco configurado.");
    if (args[0] == "testar") return await TestesIntegracao.ExecutarAsync(conexao);
    var tabelas = new[] { "tb_perfil", "tb_usuario", "tb_jogo", "tb_situacao_pedido", "tb_pedido", "tb_item_pedido" };
    foreach (var tabela in tabelas)
    {
        await using var cmd = new MySqlCommand("SELECT ENGINE FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME=@tabela", conexao);
        cmd.Parameters.AddWithValue("@tabela", tabela);
        var engine = Convert.ToString(await cmd.ExecuteScalarAsync());
        if (!string.Equals(engine, "InnoDB", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Tabela ausente ou não transacional: {tabela}. Nenhum dado de compras foi alterado.");
    }
    Console.WriteLine("[OK] Tabelas necessárias existem e usam InnoDB.");

    await using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM tb_perfil WHERE co_perfil=2", conexao))
    {
        if (Convert.ToInt32(await cmd.ExecuteScalarAsync()) != 1)
            throw new InvalidOperationException("Perfil Cliente (2) ausente. Nenhum dado de compras foi alterado.");
    }

    var situacoes = new Dictionary<int, string> { [1]="Pendente", [2]="Pago", [3]="Enviado", [4]="Cancelado" };
    if (args[0] == "preparar")
    {
        // Operação equivalente ao 005_compras.sql, sem reexecutar o esquema.
        await using var transacao = await conexao.BeginTransactionAsync();
        foreach (var (id, descricao) in situacoes)
        {
            await using var consultar = new MySqlCommand("SELECT ds_situacao_pedido FROM tb_situacao_pedido WHERE co_situacao_pedido=@id FOR UPDATE", conexao, transacao);
            consultar.Parameters.AddWithValue("@id", id);
            var existente = await consultar.ExecuteScalarAsync();
            if (existente is not null)
            {
                if (!string.Equals(Convert.ToString(existente), descricao, StringComparison.Ordinal))
                    throw new InvalidOperationException($"Situação {id} divergente. Preparação cancelada.");
                continue;
            }
            await using var inserir = new MySqlCommand("INSERT INTO tb_situacao_pedido (co_situacao_pedido,ds_situacao_pedido) VALUES (@id,@descricao)", conexao, transacao);
            inserir.Parameters.AddWithValue("@id", id);
            inserir.Parameters.AddWithValue("@descricao", descricao);
            await inserir.ExecuteNonQueryAsync();
        }
        await transacao.CommitAsync();
        Console.WriteLine("[OK] Situações preparadas. Dados existentes preservados.");
    }

    var encontradas = new Dictionary<int, string>();
    await using (var cmd = new MySqlCommand("SELECT co_situacao_pedido,ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido", conexao))
    await using (var leitor = await cmd.ExecuteReaderAsync())
        while (await leitor.ReadAsync())
            encontradas[leitor.GetInt32(0)] = leitor.GetString(1);
    foreach (var (id, descricao) in situacoes)
    {
        if (!encontradas.TryGetValue(id, out var atual) || atual != descricao)
            throw new InvalidOperationException($"Situação {id} ausente ou divergente. Rode preparar ou revise o esquema.");
        Console.WriteLine($"[OK] {id} - {descricao}");
    }

    foreach (var (titulo, sql) in new[] {
        ("Pedidos", "SELECT COUNT(*) FROM tb_pedido"),
        ("Itens de pedido", "SELECT COUNT(*) FROM tb_item_pedido")
    })
    {
        await using var cmd = new MySqlCommand(sql, conexao);
        Console.WriteLine($"{titulo}: {await cmd.ExecuteScalarAsync()}");
    }

    await using (var cmd = new MySqlCommand("SELECT co_seq_jogo,no_jogo,vl_preco,qt_estoque,st_ativo FROM tb_jogo ORDER BY co_seq_jogo", conexao))
    await using (var leitor = await cmd.ExecuteReaderAsync())
    {
        Console.WriteLine("ID | Jogo | Preço | Estoque | Ativo");
        while (await leitor.ReadAsync())
            Console.WriteLine($"{leitor.GetInt32(0)} | {leitor.GetString(1)} | {leitor.GetDecimal(2):F2} | {leitor.GetInt32(3)} | {leitor.GetBoolean(4)}");
    }
    Console.WriteLine("BANCO DE COMPRAS: OK");
    return 0;
}
catch (MySqlException erro)
{
    // Não imprime a string de conexão, credenciais ou a exceção completa.
    Console.Error.WriteLine($"Falha MySQL (código {erro.Number}). Confira a conexão e o esquema.");
    return 1;
}
catch (InvalidOperationException erro)
{
    Console.Error.WriteLine(erro.Message);
    return 1;
}
catch (Exception erro)
{
    Console.Error.WriteLine($"Falha na ferramenta: {erro.GetType().Name}. Nenhuma credencial será exibida.");
    return 1;
}
