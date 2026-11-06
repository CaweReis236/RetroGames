using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

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
                ? null : leitor.GetString("ds_jogo"),
            AnoLancamento = leitor.GetInt32("nu_ano_lancamento"),
            Preco = leitor.GetDecimal("vl_preco"),
            Estoque = leitor.GetInt32("qt_estoque"),
            Ativo = leitor.GetBoolean("st_ativo"),
            DataCadastro = leitor.GetDateTime("dt_cadastro"),
            IdPlataforma = leitor.GetInt32("co_plataforma"),
            IdCondicaoJogo = leitor.GetInt32("co_condicao_jogo")
        };
    }

    // Lista todos os jogos.
    public async Task<List<Jogo>> ListarAsync()
    {
        const string sql = "SELECT * FROM tb_jogo ORDER BY no_jogo";

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var jogos = new List<Jogo>();

        while (await leitor.ReadAsync())
        {
            jogos.Add(Converter(leitor));
        }

        return jogos;
    }

    // Busca um jogo pelo ID.
    public async Task<Jogo?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT * FROM tb_jogo
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

    // Insere um jogo e retorna o ID gerado.
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

    // Exclui um jogo.
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

    // Evita repetir os parâmetros no INSERT e no UPDATE.
    private void AdicionarParametrosJogo(MySqlCommand comando, Jogo jogo)
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
