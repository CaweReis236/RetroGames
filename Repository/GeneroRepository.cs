using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class GeneroRepository : RepositoryBase, IGeneroRepository
{
    public GeneroRepository(RepositoryConfig config) : base(config)
    {
    }

    // Converte o resultado do banco em um objeto Genero.
    private static Genero Converter(MySqlDataReader leitor)
    {
        return new Genero
        {
            Id = leitor.GetInt32("co_seq_genero"),
            Nome = leitor.GetString("no_genero")
        };
    }

    // Lista os gêneros em ordem alfabética.
    public async Task<List<Genero>> ListarAsync()
    {
        const string sql = """
            SELECT co_seq_genero, no_genero
            FROM tb_genero
            ORDER BY no_genero
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var generos = new List<Genero>();

        while (await leitor.ReadAsync())
        {
            generos.Add(Converter(leitor));
        }

        return generos;
    }
}
