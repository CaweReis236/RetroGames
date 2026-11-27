using RetroGames.Model.Model;
using RetroGames.Repository.Base;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;
namespace RetroGames.Repository;
public class SituacaoPedidoRepository(RepositoryConfig config) : RepositoryBase(config), ISituacaoPedidoRepository
{
    public async Task<List<SituacaoPedido>> ListarAsync()
    {
        await using var conexao = await CriarConexaoAbertaAsync();
        await using var comando = CriarComando(conexao, "SELECT * FROM tb_situacao_pedido ORDER BY co_situacao_pedido");
        await using var leitor = await comando.ExecuteReaderAsync();
        var situacoes = new List<SituacaoPedido>();
        while(await leitor.ReadAsync()) situacoes.Add(new SituacaoPedido { Id=leitor.GetInt32("co_situacao_pedido"), Descricao=leitor.GetString("ds_situacao_pedido") });
        return situacoes;
    }
}
