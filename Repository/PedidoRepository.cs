using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
namespace RetroGames.Repository;
public class PedidoRepository(RepositoryConfig config) : RepositoryBase(config), IPedidoRepository
{
    public async Task<int> InserirAsync(Pedido pedido)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "INSERT INTO tb_pedido (dt_pedido,vl_total,co_usuario,co_situacao_pedido) VALUES (@data,@total,@usuario,@situacao)");
        AdicionarParametro(comando,"@data",pedido.DataPedido);
        AdicionarParametro(comando,"@total",pedido.Total);
        AdicionarParametro(comando,"@usuario",pedido.IdUsuario);
        AdicionarParametro(comando,"@situacao",pedido.IdSituacao);
        await ExecutarGravacaoAsync(comando);
        return checked((int)comando.LastInsertedId);
    }
    public async Task<List<Pedido>> ListarPorUsuarioAsync(int idUsuario)
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "SELECT * FROM tb_pedido WHERE co_usuario=@usuario ORDER BY dt_pedido DESC, co_seq_pedido DESC");
        AdicionarParametro(comando,"@usuario",idUsuario);
        await using var leitor = await comando.ExecuteReaderAsync();
        var pedidos = new List<Pedido>();
        while(await leitor.ReadAsync()) pedidos.Add(new Pedido { Id=leitor.GetInt32("co_seq_pedido"), DataPedido=leitor.GetDateTime("dt_pedido"), Total=leitor.GetDecimal("vl_total"), IdUsuario=leitor.GetInt32("co_usuario"), IdSituacao=leitor.GetInt32("co_situacao_pedido") });
        return pedidos;
    }
}
