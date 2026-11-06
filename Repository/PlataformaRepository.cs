using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class PlataformaRepository : RepositoryBase, IPlataformaRepository
{
    public PlataformaRepository(RepositoryConfig config) : base(config)
    {
    }

    private static Plataforma Converter(MySqlDataReader leitor)
    {
        return new Plataforma
        {
            Id = leitor.GetInt32("co_seq_plataforma"),
            Nome = leitor.GetString("no_plataforma"),
            Fabricante = leitor.IsDBNull(leitor.GetOrdinal("no_fabricante"))
                ? string.Empty
                : leitor.GetString("no_fabricante"),
            AnoLancamento = leitor.IsDBNull(leitor.GetOrdinal("nu_ano_lancamento"))
                ? 0
                : leitor.GetInt32("nu_ano_lancamento")
        };
    }

    public async Task<List<Plataforma>> ListarAsync()
    {
        const string sql = """
            SELECT co_seq_plataforma, no_plataforma,
                   no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            ORDER BY no_plataforma
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var plataformas = new List<Plataforma>();

        while (await leitor.ReadAsync())
        {
            plataformas.Add(Converter(leitor));
        }

        return plataformas;
    }

    public async Task<Plataforma?> BuscarPorIdAsync(int id)
    {
        const string sql = """
            SELECT co_seq_plataforma, no_plataforma,
                   no_fabricante, nu_ano_lancamento
            FROM tb_plataforma
            WHERE co_seq_plataforma = @id
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

    public async Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_plataforma
            WHERE no_plataforma COLLATE utf8mb4_unicode_ci =
                  @nome COLLATE utf8mb4_unicode_ci
              AND (@idIgnorar IS NULL OR co_seq_plataforma <> @idIgnorar)
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", nome);
        AdicionarParametro(comando, "@idIgnorar", idIgnorar);

        var resultado = await comando.ExecuteScalarAsync();
        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<bool> PossuiJogosAsync(int id)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_jogo
            WHERE co_plataforma = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        var resultado = await comando.ExecuteScalarAsync();
        return Convert.ToInt64(resultado) > 0;
    }

    public async Task<int> InserirAsync(Plataforma plataforma)
    {
        const string sql = """
            INSERT INTO tb_plataforma
                (no_plataforma, no_fabricante, nu_ano_lancamento)
            VALUES
                (@nome, @fabricante, @ano)
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);

        await ExecutarGravacaoAsync(comando);

        return checked((int)comando.LastInsertedId);
    }

    public async Task<int> AlterarAsync(Plataforma plataforma)
    {
        const string sql = """
            UPDATE tb_plataforma
            SET no_plataforma = @nome,
                no_fabricante = @fabricante,
                nu_ano_lancamento = @ano
            WHERE co_seq_plataforma = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", plataforma.Id);
        AdicionarParametro(comando, "@nome", plataforma.Nome);
        AdicionarParametro(comando, "@fabricante", plataforma.Fabricante);
        AdicionarParametro(comando, "@ano", plataforma.AnoLancamento);

        return await ExecutarGravacaoAsync(comando);
    }

    public async Task<int> ExcluirAsync(int id)
    {
        const string sql = """
            DELETE FROM tb_plataforma
            WHERE co_seq_plataforma = @id
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@id", id);

        return await comando.ExecuteNonQueryAsync();
    }
}
