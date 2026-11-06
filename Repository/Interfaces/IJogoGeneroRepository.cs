using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoGeneroRepository
{
    // Lista os gêneros associados a um jogo.
    Task<List<JogoGenero>> ListarPorJogoAsync(int idJogo);

    // Verifica se um jogo já possui determinado gênero.
    Task<bool> ExisteAsync(int idJogo, int idGenero);

    // Associa um gênero a um jogo.
    Task<int> InserirAsync(JogoGenero jogoGenero);

    // Remove a associação entre um jogo e um gênero.
    Task<int> ExcluirAsync(int idJogo, int idGenero);

    // Remove todos os gêneros associados a um jogo.
    Task<int> ExcluirPorJogoAsync(int idJogo);
}
