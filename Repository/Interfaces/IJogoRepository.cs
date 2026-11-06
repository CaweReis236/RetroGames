using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IJogoRepository
{
    // Lista todos os jogos.
    Task<List<Jogo>> ListarAsync();

    // Busca um jogo pelo ID.
    Task<Jogo?> BuscarPorIdAsync(int id);

    // Insere um jogo e retorna seu ID.
    Task<int> InserirAsync(Jogo jogo);

    // Altera os dados de um jogo.
    Task<int> AlterarAsync(Jogo jogo);

    // Ativa ou desativa um jogo.
    Task<int> AlterarStatusAsync(int id, bool ativo);

    // Exclui um jogo do banco.
    Task<int> ExcluirAsync(int id);
}
