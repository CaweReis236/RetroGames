using RetroGames.Model.Model;
using RetroGames.Services.ServiceModels;

namespace RetroGames.Services.Interfaces;

public interface IJogoService
{
    Task<(List<JogoDetalheServiceModel> Jogos, int Total)> ListarAsync(
        int pagina,
        int tamanhoPagina,
        string? nome = null,
        int? idPlataforma = null,
        int? idGenero = null,
        bool incluirInativos = false
    );

    Task<JogoDetalheServiceModel> BuscarPorIdAsync(int id);

    Task<JogoDetalheServiceModel> InserirAsync(
        Jogo jogo,
        List<int> idsGeneros
    );

    Task<JogoDetalheServiceModel> AlterarAsync(
        Jogo jogo,
        List<int> idsGeneros
    );

    Task DesativarAsync(int id);
}
