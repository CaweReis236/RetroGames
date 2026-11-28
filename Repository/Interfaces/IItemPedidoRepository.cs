using RetroGames.Model.Model;
namespace RetroGames.Repository.Interfaces;
public interface IItemPedidoRepository
{
    Task<int> InserirAsync(ItemPedido item);
    Task<List<ItemPedido>> ListarPorPedidosAsync(List<int> idsPedidos);
}
