using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
namespace RetroGames.Repository;
public class ItemPedidoRepository(RepositoryConfig config) : RepositoryBase(config), IItemPedidoRepository
{
    public async Task<int> InserirAsync(ItemPedido item)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "INSERT INTO tb_item_pedido (qt_item,vl_unitario,co_pedido,co_jogo) VALUES (@quantidade,@valor,@pedido,@jogo)");
        AdicionarParametro(comando,"@quantidade",item.Quantidade);
        AdicionarParametro(comando,"@valor",item.ValorUnitario);
        AdicionarParametro(comando,"@pedido",item.IdPedido);
        AdicionarParametro(comando,"@jogo",item.IdJogo);
        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
    public async Task<List<ItemPedido>> ListarPorPedidosAsync(List<int> idsPedidos)
    {
        if(idsPedidos.Count == 0) return new();
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "");
        var parametros = idsPedidos.Select((id,i) => { var p="@id"+i; AdicionarParametro(comando,p,id); return p; }).ToList();
        comando.CommandText = "SELECT * FROM tb_item_pedido WHERE co_pedido IN ("+string.Join(",",parametros)+") ORDER BY co_seq_item_pedido";
        await using var leitor = await comando.ExecuteReaderAsync();
        var itens = new List<ItemPedido>();
        while(await leitor.ReadAsync()) itens.Add(new ItemPedido { Id=leitor.GetInt32("co_seq_item_pedido"), Quantidade=leitor.GetInt32("qt_item"), ValorUnitario=leitor.GetDecimal("vl_unitario"), IdPedido=leitor.GetInt32("co_pedido"), IdJogo=leitor.GetInt32("co_jogo") });
        return itens;
    }
}
