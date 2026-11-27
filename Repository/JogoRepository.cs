using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Filtros;

namespace RetroGames.Repository;

public class JogoRepository : RepositoryBase, IJogoRepository
{
    public JogoRepository(RepositoryConfig config) : base(config)
    {
    }

    // Converte uma linha do banco em um objeto Jogo.
    private static Jogo Converter(MySqlDataReader leitor)
    {
        return new Jogo
        {
            Id = leitor.GetInt32("co_seq_jogo"),
            Nome = leitor.GetString("no_jogo"),

            Descricao = leitor.IsDBNull(leitor.GetOrdinal("ds_jogo"))
                ? null
                : leitor.GetString("ds_jogo"),

            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }

    // Monta as condições SQL usadas pela listagem e pela contagem.
    // Todos os valores são enviados como parâmetros.
    private static string MontarFiltros(
        MySqlCommand comando,
        JogoFiltro filtro)
    {
        var condicoes = new List<string>();

        // Por padrão, retorna apenas jogos ativos.
        if (!filtro.IncluirInativos)
        {
            condicoes.Add("st_ativo = @ativo");

            comando.Parameters.AddWithValue(
                "@ativo",
                true
            );
        }

        // Filtro opcional por parte do nome.
        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            condicoes.Add("no_jogo LIKE @nome");

            comando.Parameters.AddWithValue(
                "@nome",
                $"%{filtro.Nome.Trim()}%"
            );
        }

        // Filtro opcional por plataforma.
        if (filtro.IdPlataforma.HasValue)
        {
            condicoes.Add("co_plataforma = @plataforma");

            comando.Parameters.AddWithValue(
                "@plataforma",
                filtro.IdPlataforma.Value
            );
        }

        // Filtro opcional por lista de IDs.
        if (filtro.IdsJogos != null)
        {
            var parametros = new List<string>();

            for (int i = 0; i < filtro.IdsJogos.Count; i++)
            {
                string nomeParametro = $"@id{i}";

                parametros.Add(nomeParametro);

                comando.Parameters.AddWithValue(
                    nomeParametro,
                    filtro.IdsJogos[i]
                );
            }

            condicoes.Add(
                $"co_seq_jogo IN ({string.Join(", ", parametros)})"
            );
        }

        if (condicoes.Count == 0)
        {
            return string.Empty;
        }

        return " WHERE " + string.Join(" AND ", condicoes);
    }

    // Lista jogos com filtros opcionais, paginação e total.
    public async Task<(List<Jogo> Jogos, int Total)> ListarAsync(
        JogoFiltro filtro)
    {
        ArgumentNullException.ThrowIfNull(filtro);

        if (filtro.Pagina < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(filtro.Pagina),
                "A página deve ser maior ou igual a 1."
            );
        }

        if (filtro.TamanhoPagina < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(filtro.TamanhoPagina),
                "O tamanho da página deve ser maior ou igual a 1."
            );
        }

        // Lista vazia: nenhum jogo corresponde ao filtro.
        // Não realiza nenhuma consulta ao banco.
        if (filtro.IdsJogos is { Count: 0 })
        {
            return (new List<Jogo>(), 0);
        }

        await using var conexao = await CriarConexaoAbertaAsync();

        // ==========================================
        // CONSULTA 1: CONTAGEM TOTAL
        // ==========================================

        await using var comandoTotal = CriarComando(
            conexao,
            "SELECT COUNT(*) FROM tb_jogo"
        );

        string filtrosTotal = MontarFiltros(
            comandoTotal,
            filtro
        );

        comandoTotal.CommandText += filtrosTotal;

        var resultadoTotal = await comandoTotal.ExecuteScalarAsync();

        int total = Convert.ToInt32(resultadoTotal);

        // ==========================================
        // CONSULTA 2: LISTAGEM PAGINADA
        // ==========================================

        await using var comandoLista = CriarComando(
            conexao,
            "SELECT * FROM tb_jogo"
        );

        string filtrosLista = MontarFiltros(
            comandoLista,
            filtro
        );

        comandoLista.CommandText += filtrosLista + """
            
            ORDER BY no_jogo, co_seq_jogo
            LIMIT @limite OFFSET @offset
            """;

        // Quantidade de registros por página.
        AdicionarParametro(
            comandoLista,
            "@limite",
            filtro.TamanhoPagina
        );

        // Calcula o deslocamento.
        int offset = checked(
            (filtro.Pagina - 1) * filtro.TamanhoPagina
        );

        AdicionarParametro(
            comandoLista,
            "@offset",
            offset
        );

        var jogos = new List<Jogo>();

        await using var leitor = await comandoLista.ExecuteReaderAsync();

        while (await leitor.ReadAsync())
        {
            jogos.Add(Converter(leitor));
        }

        return (jogos, total);
    }

    // Busca um jogo pelo ID.
    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT *
            FROM tb_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        await using var leitor = await comando.ExecuteReaderAsync();

        if (await leitor.ReadAsync())
        {
            return Converter(leitor);
        }

        return null;
    }

    // Bloqueia a linha até o fim da transação de compra.
    public async Task<Jogo?> BuscarParaCompraAsync(int id)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "SELECT * FROM tb_jogo WHERE co_seq_jogo=@id FOR UPDATE");
        AdicionarParametro(comando,"@id",id);
        await using var leitor = await comando.ExecuteReaderAsync();
        return await leitor.ReadAsync() ? Converter(leitor) : null;
    }
    public async Task<int> BaixarEstoqueAsync(int id, int quantidade)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "UPDATE tb_jogo SET qt_estoque=qt_estoque-@quantidade WHERE co_seq_jogo=@id AND st_ativo=TRUE AND qt_estoque>=@quantidade");
        AdicionarParametro(comando,"@id",id);
        AdicionarParametro(comando,"@quantidade",quantidade);
        return await ExecutarGravacaoAsync(comando);
    }
    public async Task<List<Jogo>> ListarPorIdsAsync(List<int> ids)
    {
        if(ids.Count==0) return new();
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "");
        var parametros=ids.Select((id,i)=>{var p="@id"+i; AdicionarParametro(comando,p,id);return p;}).ToList();
        comando.CommandText="SELECT * FROM tb_jogo WHERE co_seq_jogo IN ("+string.Join(",",parametros)+")";
        var jogos=new List<Jogo>();
        await using var leitor=await comando.ExecuteReaderAsync();
        while(await leitor.ReadAsync()) jogos.Add(Converter(leitor));
        return jogos;
    }

    // Insere um jogo e retorna seu ID.
    public async Task<int> InserirAsync(Jogo jogo)
    {
        const string sql = """
            INSERT INTO tb_jogo
                (no_jogo, ds_jogo, nu_ano_lancamento,
                 vl_preco, qt_estoque, st_ativo,
                 dt_cadastro, co_plataforma, co_condicao_jogo)
            VALUES
                (@nome, @descricao, @ano, @preco, @estoque,
                 @ativo, @data, @plataforma, @condicao)
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametrosJogo(comando, jogo);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    // Altera os dados de um jogo.
    public async Task<int> AlterarAsync(Jogo jogo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET no_jogo = @nome,
                ds_jogo = @descricao,
                nu_ano_lancamento = @ano,
                vl_preco = @preco,
                qt_estoque = @estoque,
                st_ativo = @ativo,
                dt_cadastro = @data,
                co_plataforma = @plataforma,
                co_condicao_jogo = @condicao
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", jogo.Id);

        AdicionarParametrosJogo(comando, jogo);

        return await ExecutarGravacaoAsync(comando);
    }

    // Ativa ou desativa um jogo.
    public async Task<int> AlterarStatusAsync(int id, bool ativo)
    {
        const string sql = """
            UPDATE tb_jogo
            SET st_ativo = @ativo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);
        AdicionarParametro(comando, "@ativo", ativo);

        return await ExecutarGravacaoAsync(comando);
    }

    // Exclui um jogo fisicamente do banco.
    // Mantido por compatibilidade com a Aula 2.
    // O JogoService usará desativação, não exclusão.
    public async Task<int> ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_jogo
            WHERE co_seq_jogo = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        return await comando.ExecuteNonQueryAsync();
    }

    // Evita repetir os parâmetros do INSERT e UPDATE.
    private void AdicionarParametrosJogo(
        MySqlCommand comando,
        Jogo jogo)
    {
        AdicionarParametro(comando, "@nome", jogo.Nome);
        AdicionarParametro(comando, "@descricao", jogo.Descricao);
        AdicionarParametro(comando, "@ano", jogo.AnoLancamento);
        AdicionarParametro(comando, "@preco", jogo.Preco);
        AdicionarParametro(comando, "@estoque", jogo.Estoque);
        AdicionarParametro(comando, "@ativo", jogo.Ativo);
        AdicionarParametro(comando, "@data", jogo.DataCadastro);
        AdicionarParametro(comando, "@plataforma", jogo.IdPlataforma);
        AdicionarParametro(comando, "@condicao", jogo.IdCondicaoJogo);
    }
}
