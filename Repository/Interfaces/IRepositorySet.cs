namespace RetroGames.Repository.Interfaces;

public interface IRepositorySet
{
    IPedidoRepository Pedido { get; }
    IItemPedidoRepository ItemPedido { get; }
    ISituacaoPedidoRepository SituacaoPedido { get; }
    IUsuarioRepository Usuario { get; }
    IPlataformaRepository Plataforma { get; }

    IJogoRepository Jogo { get; }

    IJogoGeneroRepository JogoGenero { get; }

    IGeneroRepository Genero { get; }

    ICondicaoJogoRepository CondicaoJogo { get; }
}
