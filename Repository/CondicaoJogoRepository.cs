using MySqlConnector;
using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

namespace RetroGames.Repository;

public class CondicaoJogoRepository : RepositoryBase, ICondicaoJogoRepository
{
    public CondicaoJogoRepository(RepositoryConfig config) : base(config)
    {
    }

    // Converte o resultado do banco em um objeto CondicaoJogo.
    private static CondicaoJogo Converter(MySqlDataReader leitor)
    {
        return new CondicaoJogo
        {
            Id = leitor.GetInt32("co_condicao_jogo"),
            Descricao = leitor.GetString("ds_condicao_jogo")
        };
    }

    // Lista as condições dos jogos.
    public async Task<List<CondicaoJogo>> ListarAsync()
    {
        const string sql = """
            SELECT co_condicao_jogo, ds_condicao_jogo
            FROM tb_condicao_jogo
            """;

        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, sql);
        await using var leitor = await comando.ExecuteReaderAsync();

        var condicoes = new List<CondicaoJogo>();

        while (await leitor.ReadAsync())
        {
            condicoes.Add(Converter(leitor));
        }

        return condicoes;
    }
}
