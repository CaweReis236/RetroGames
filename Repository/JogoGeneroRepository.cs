using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class JogoGeneroRepository : RepositoryBase, IJogoGeneroRepository
{
    public JogoGeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    // Converte uma linha do banco em um objeto JogoGenero.
    private static JogoGenero Converter(MySqlDataReader leitor)
    {
        return new JogoGenero
        {
            IdJogo = leitor.GetInt32("co_jogo"),
            IdGenero = leitor.GetInt32("co_genero")
        };
    }

    // Lista os gêneros associados a um jogo.
    public async Task<List<JogoGenero>> ListarPorJogoAsync(int idJogo)
    {
        const string sql = """
            SELECT co_jogo, co_genero
            FROM tb_jogo_genero
            WHERE co_jogo = @idJogo
            ORDER BY co_genero
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        await using var leitor = await comando.ExecuteReaderAsync();

        var generos = new List<JogoGenero>();

        while (await leitor.ReadAsync())
        {
            generos.Add(Converter(leitor));
        }

        return generos;
    }

    // Verifica se a associação já existe.
    public async Task<bool> ExisteAsync(int idJogo, int idGenero)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM tb_jogo_genero
            WHERE co_jogo = @idJogo
              AND co_genero = @idGenero
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);
        AdicionarParametro(comando, "@idGenero", idGenero);

        var resultado = await comando.ExecuteScalarAsync();

        return Convert.ToInt64(resultado) > 0;
    }

    // Associa um gênero a um jogo.
    public async Task<int> InserirAsync(JogoGenero jogoGenero)
    {
        const string sql = """
            INSERT INTO tb_jogo_genero (co_jogo, co_genero)
            VALUES (@idJogo, @idGenero)
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", jogoGenero.IdJogo);
        AdicionarParametro(comando, "@idGenero", jogoGenero.IdGenero);

        return await ExecutarGravacaoAsync(comando);
    }

    // Remove uma associação específica.
    public async Task<int> ExcluirAsync(int idJogo, int idGenero)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo
              AND co_genero = @idGenero
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);
        AdicionarParametro(comando, "@idGenero", idGenero);

        return await comando.ExecuteNonQueryAsync();
    }

    // Remove todos os gêneros associados a um jogo.
    public async Task<int> ExcluirPorJogoAsync(int idJogo)
    {
        const string sql = """
            DELETE FROM tb_jogo_genero
            WHERE co_jogo = @idJogo
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);

        AdicionarParametro(comando, "@idJogo", idJogo);

        return await comando.ExecuteNonQueryAsync();
    }
}
