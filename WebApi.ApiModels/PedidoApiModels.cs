namespace RetroGames.WebApi.ApiModels;
public class CompraItemRequestApiModel
{
    public int IdJogo { get; set; }
    public int Quantidade { get; set; }
}
public class PedidoRequestApiModel
{
    public List<CompraItemRequestApiModel> Itens { get; set; } = new();
}
public record ItemPedidoResponseApiModel(int IdJogo, string NomeJogo, int Quantidade, decimal ValorUnitario);
public record PedidoResponseApiModel(int Id, DateTime DataPedido, decimal Total, string Situacao, List<ItemPedidoResponseApiModel> Itens);
