using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;
using MySqlConnector;

internal static class TestesIntegracao
{
    private static HttpClient Cliente() => new(new HttpClientHandler
    {
        CookieContainer = new CookieContainer(), AllowAutoRedirect = false
    }) { BaseAddress = new Uri("http://localhost:5080/api/"), Timeout = TimeSpan.FromSeconds(30) };

    private static async Task<JsonElement> Chamar(HttpClient cliente, string rota, int status, int codigo, object? corpo = null)
    {
        using var resposta = corpo is null ? await cliente.GetAsync(rota) : await cliente.PostAsJsonAsync(rota, corpo);
        if ((int)resposta.StatusCode != status)
            throw new InvalidOperationException($"{rota}: HTTP esperado {status}, recebido {(int)resposta.StatusCode}.");
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var raiz = json.RootElement;
        if (raiz.GetProperty("responseCode").GetInt32() != codigo)
            throw new InvalidOperationException($"{rota}: código de resposta diferente de {codigo}.");
        Console.WriteLine($"[OK] {(corpo is null ? "GET" : "POST")} {rota}: HTTP {status}, código {codigo}");
        return raiz.TryGetProperty("data", out var dados) ? dados.Clone() : default;
    }

    private static async Task<(long Pedidos, long Itens, string Estoques)> Estado(MySqlConnection conexao)
    {
        await using var p = new MySqlCommand("SELECT COUNT(*) FROM tb_pedido", conexao);
        var pedidos = Convert.ToInt64(await p.ExecuteScalarAsync());
        await using var i = new MySqlCommand("SELECT COUNT(*) FROM tb_item_pedido", conexao);
        var itens = Convert.ToInt64(await i.ExecuteScalarAsync());
        await using var j = new MySqlCommand("SELECT co_seq_jogo,qt_estoque FROM tb_jogo ORDER BY co_seq_jogo", conexao);
        await using var leitor = await j.ExecuteReaderAsync();
        var estoque = new List<string>();
        while (await leitor.ReadAsync()) estoque.Add($"{leitor.GetInt32(0)}:{leitor.GetInt32(1)}");
        return (pedidos, itens, string.Join(",", estoque));
    }

    public static async Task<int> ExecutarAsync(MySqlConnection conexao)
    {
        // Não realiza compras válidas; preserva pedidos e estoque.
        // Uma nova conta de teste é criada pela API, sem depender de senhas antigas.
        using var visitante = Cliente();
        using var segundo = Cliente();
        var vazio = new { itens = Array.Empty<object>() };
        await Chamar(visitante, "Pedido/meus", 401, 4);
        await Chamar(visitante, "Pedido", 401, 4, vazio);

        var antes = await Estado(conexao);
        if (antes.Pedidos == 0)
            throw new InvalidOperationException("É necessário um pedido existente para comprovar o isolamento.");
        Console.WriteLine($"[OK] Existem {antes.Pedidos} pedidos anteriores no banco.");
        var cadastro = new { nome = "Cliente Teste Integracao", email = $"integracao-{Guid.NewGuid():N}@example.com", senha = Convert.ToHexString(RandomNumberGenerator.GetBytes(24)) };
        await Chamar(segundo, "Autenticacao/cadastrar", 201, 0, cadastro);
        var atual = await Chamar(segundo, "Autenticacao/eu", 200, 0);
        if (atual.GetProperty("perfil").GetString() != "Cliente" || atual.GetProperty("email").GetString() != cadastro.email)
            throw new InvalidOperationException("Cadastro não iniciou a sessão correta.");
        var outros = await Chamar(segundo, "Pedido/meus", 200, 0);
        if (outros.GetArrayLength() != 0) throw new InvalidOperationException("O novo cliente recebeu pedidos indevidos.");
        Console.WriteLine("[OK] Cliente novo não vê pedidos existentes de outras contas.");
        await Chamar(segundo, "Autenticacao/cadastrar", 409, 106, cadastro);
        await Chamar(segundo, "Autenticacao/logout", 200, 0, new { });
        await Chamar(segundo, "Pedido/meus", 401, 4);
        await Chamar(segundo, "Autenticacao/login", 200, 0, new { cadastro.email, cadastro.senha });

        var jogos = new List<(int Id, int Estoque, bool Ativo)>();
        await using (var cmd = new MySqlCommand("SELECT co_seq_jogo,qt_estoque,st_ativo FROM tb_jogo ORDER BY co_seq_jogo", conexao))
        await using (var leitor = await cmd.ExecuteReaderAsync())
            while (await leitor.ReadAsync()) jogos.Add((leitor.GetInt32(0),leitor.GetInt32(1),leitor.GetBoolean(2)));
        var limitado = jogos.FirstOrDefault(j => j.Ativo && j.Estoque >= 0 && j.Estoque < 10);
        var disponivel = jogos.FirstOrDefault(j => j.Ativo && j.Estoque > 0 && j.Id != limitado.Id);
        if (limitado.Id == 0 || disponivel.Id == 0)
            throw new InvalidOperationException("São necessários dois jogos para testar estoque insuficiente.");
        var inexistente = checked(jogos.Max(j => j.Id) + 1);
        await Chamar(segundo, "Pedido", 400, 3, vazio);
        await Chamar(segundo, "Pedido", 400, 3, new { itens = new[] { new { idJogo = disponivel.Id, quantidade = 11 } } });
        await Chamar(segundo, "Pedido", 400, 110, new { itens = new[] { new { idJogo = inexistente, quantidade = 1 } } });
        var inativo = jogos.FirstOrDefault(j => !j.Ativo);
        if (inativo.Id != 0)
            await Chamar(segundo, "Pedido", 400, 110, new { itens = new[] { new { idJogo = inativo.Id, quantidade = 1 } } });
        else Console.WriteLine("[PULADO] Nenhum jogo inativo disponível para teste.");
        await Chamar(segundo, "Pedido", 409, 111, new { itens = new[] {
            new { idJogo = disponivel.Id, quantidade = 1 },
            new { idJogo = limitado.Id, quantidade = limitado.Estoque + 1 }
        } });
        var depois = await Estado(conexao);
        if (antes != depois) throw new InvalidOperationException("Pedidos, itens ou estoque mudaram após compra recusada. Verifique também compras concorrentes.");
        Console.WriteLine("[OK] Estoque e contagens idênticos antes/depois da compra recusada.");

        var novamente = await Chamar(segundo, "Pedido/meus", 200, 0);
        if (novamente.GetArrayLength() != 0 || antes != await Estado(conexao))
            throw new InvalidOperationException("Dados originais não permaneceram estáveis.");
        await Chamar(segundo, "Jogo", 403, 5, new { });
        await Chamar(segundo, "Autenticacao/logout", 200, 0, new { });
        await Chamar(segundo, "Pedido/meus", 401, 4);
        Console.WriteLine("[OK] Isolamento de clientes e autorização confirmados.");
        Console.WriteLine("INTEGRACAO DE COMPRAS: OK");
        return 0;
    }
}
