namespace RetroGames.Services.ServiceModels;
public record CompraItemServiceModel(int IdJogo, int Quantidade);
public record ItemPedidoServiceModel(int IdJogo, string NomeJogo, int Quantidade, decimal ValorUnitario);
public record PedidoServiceModel(int Id, DateTime DataPedido, decimal Total, string Situacao, List<ItemPedidoServiceModel> Itens);
