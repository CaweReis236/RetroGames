using RetroGames.Model.Model;

namespace RetroGames.Repository.Interfaces;

public interface IPlataformaRepository
{
    Task<List<Plataforma>> ListarAsync();
    Task<Plataforma?> BuscarPorIdAsync(int id);
    Task<bool> ExisteComNomeAsync(string nome, int? idIgnorar = null);
    Task<bool> PossuiJogosAsync(int id);
    Task<int> InserirAsync(Plataforma plataforma);
    Task<int> AlterarAsync(Plataforma plataforma);
    Task<int> ExcluirAsync(int id);
}
