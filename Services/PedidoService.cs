using System.Transactions;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Errors;
using RetroGames.Services.Interfaces;
using RetroGames.Services.ServiceModels;
namespace RetroGames.Services;
public class PedidoService(IRepositorySet repositorio) : IPedidoService
{
    public async Task<PedidoServiceModel> FinalizarAsync(int idUsuario, List<CompraItemServiceModel> itens)
    {
        ArgumentNullException.ThrowIfNull(itens);
        if(itens.Count == 0 || itens.Any(i => i.IdJogo <= 0 || i.Quantidade is < 1 or > 10)
            || itens.Select(i=>i.IdJogo).Distinct().Count() != itens.Count)
            throw new ArgumentException("Itens inválidos.", nameof(itens));
        var transactionOptions = new TransactionOptions
        {
            IsolationLevel = IsolationLevel.ReadCommitted,
            Timeout = TransactionManager.DefaultTimeout
        };
        using var transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled);
        var detalhes = new List<ItemPedidoServiceModel>();
        // Ordem estável de bloqueio para evitar inversão de locks em compras concorrentes.
        foreach(var item in itens.OrderBy(i=>i.IdJogo))
        {
            var jogo = await repositorio.Jogo.BuscarParaCompraAsync(item.IdJogo);
            if(jogo is null || !jogo.Ativo) throw new BusinessRuleException(BusinessErrors.JOGO_INDISPONIVEL);
            if(jogo.Estoque < item.Quantidade) throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);
            detalhes.Add(new ItemPedidoServiceModel(jogo.Id,jogo.Nome,item.Quantidade,jogo.Preco));
        }
        var pedido = new Pedido
        {
            IdUsuario = idUsuario, IdSituacao = SituacaoPedidoIds.Pendente,
            DataPedido = DateTime.UtcNow, Total = detalhes.Sum(i=>i.ValorUnitario*i.Quantidade)
        };
        pedido.Id = await repositorio.Pedido.InserirAsync(pedido);
        foreach(var item in detalhes)
        {
            if(await repositorio.Jogo.BaixarEstoqueAsync(item.IdJogo,item.Quantidade) != 1)
                throw new BusinessRuleException(BusinessErrors.ESTOQUE_INSUFICIENTE);
            await repositorio.ItemPedido.InserirAsync(new ItemPedido
            {
                IdPedido=pedido.Id, IdJogo=item.IdJogo, Quantidade=item.Quantidade, ValorUnitario=item.ValorUnitario
            });
        }
        transactionScope.Complete();
        return new PedidoServiceModel(pedido.Id,pedido.DataPedido,pedido.Total,"Pendente",detalhes);
    }
    public async Task<List<PedidoServiceModel>> ListarMeusAsync(int idUsuario)
    {
        var pedidos = await repositorio.Pedido.ListarPorUsuarioAsync(idUsuario);
        if(pedidos.Count == 0) return new();
        var itens = await repositorio.ItemPedido.ListarPorPedidosAsync(pedidos.Select(p=>p.Id).ToList());
        var jogos = await repositorio.Jogo.ListarPorIdsAsync(itens.Select(i=>i.IdJogo).Distinct().ToList());
        var situacoes = await repositorio.SituacaoPedido.ListarAsync();
        var itensPorPedido = (from item in itens join jogo in jogos on item.IdJogo equals jogo.Id
            select new { item.IdPedido, Detalhe = new ItemPedidoServiceModel(jogo.Id,jogo.Nome,item.Quantidade,item.ValorUnitario) })
            .ToLookup(i=>i.IdPedido,i=>i.Detalhe);
        return (from pedido in pedidos
            join situacao in situacoes on pedido.IdSituacao equals situacao.Id
            orderby pedido.DataPedido descending, pedido.Id descending
            select new PedidoServiceModel(pedido.Id,DateTime.SpecifyKind(pedido.DataPedido,DateTimeKind.Utc),pedido.Total,situacao.Descricao,itensPorPedido[pedido.Id].ToList())).ToList();
    }
}
