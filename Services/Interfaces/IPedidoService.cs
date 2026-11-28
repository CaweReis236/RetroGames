using RetroGames.Services.ServiceModels;
namespace RetroGames.Services.Interfaces;
public interface IPedidoService
{
    Task<PedidoServiceModel> FinalizarAsync(int idUsuario, List<CompraItemServiceModel> itens);
    Task<List<PedidoServiceModel>> ListarMeusAsync(int idUsuario);
}
