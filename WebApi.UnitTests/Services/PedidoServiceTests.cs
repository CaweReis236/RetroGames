using Moq;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Errors;
using RetroGames.Services.ServiceModels;
using System.Transactions;

namespace RetroGames.WebApi.UnitTests.Services;

public class PedidoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPedidoRepository> _pedidos = new();
    private readonly Mock<IItemPedidoRepository> _itens = new();
    private readonly Mock<ISituacaoPedidoRepository> _situacoes = new();

    private PedidoService CriarService()
    {
        _repositorio.SetupGet(r=>r.Jogo).Returns(_jogos.Object);
        _repositorio.SetupGet(r=>r.Pedido).Returns(_pedidos.Object);
        _repositorio.SetupGet(r=>r.ItemPedido).Returns(_itens.Object);
        _repositorio.SetupGet(r=>r.SituacaoPedido).Returns(_situacoes.Object);
        return new PedidoService(_repositorio.Object);
    }

    private void PrepararJogo(int id, int estoque=5, decimal preco=49.90m, bool ativo=true)
    {
        _jogos.Setup(r=>r.BuscarParaCompraAsync(id)).ReturnsAsync(new Jogo { Id=id, Nome="Jogo "+id, Ativo=ativo, Estoque=estoque, Preco=preco });
        _jogos.Setup(r=>r.BaixarEstoqueAsync(id,It.IsAny<int>())).ReturnsAsync(1);
    }

    private void VerificarSemGravacao()
    {
        _pedidos.Verify(r=>r.InserirAsync(It.IsAny<Pedido>()),Times.Never);
        _itens.Verify(r=>r.InserirAsync(It.IsAny<ItemPedido>()),Times.Never);
        _jogos.Verify(r=>r.BaixarEstoqueAsync(It.IsAny<int>(),It.IsAny<int>()),Times.Never);
    }

    [Fact]
    public async Task Finalizar_JogoInexistente_NaoGrava()
    {
        _jogos.Setup(r=>r.BuscarParaCompraAsync(99)).ReturnsAsync((Jogo?)null);
        var service=CriarService();
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>service.FinalizarAsync(1,new(){new(99,1)}));
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL,erro.Erro);
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Finalizar_JogoInativo_NaoGrava()
    {
        PrepararJogo(1,ativo:false);
        var service=CriarService();
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>service.FinalizarAsync(1,new(){new(1,1)}));
        Assert.Equal(BusinessErrors.JOGO_INDISPONIVEL,erro.Erro);
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Finalizar_SegundoJogoSemEstoque_NaoGravaNada()
    {
        PrepararJogo(1);PrepararJogo(2,estoque:1);
        var service=CriarService();
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>service.FinalizarAsync(1,new(){new(1,1),new(2,2)}));
        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE,erro.Erro);
        VerificarSemGravacao();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public async Task Finalizar_QuantidadeInvalida_NaoGrava(int quantidade)
    {
        var service=CriarService();
        await Assert.ThrowsAsync<ArgumentException>(()=>service.FinalizarAsync(1,new(){new(1,quantidade)}));
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Finalizar_CarrinhoVazio_NaoGrava()
    {
        var service=CriarService();
        await Assert.ThrowsAsync<ArgumentException>(()=>service.FinalizarAsync(1,new()));
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Finalizar_JogoDuplicado_NaoGrava()
    {
        var service=CriarService();
        await Assert.ThrowsAsync<ArgumentException>(()=>service.FinalizarAsync(1,new(){new(1,1),new(1,2)}));
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Finalizar_CompraValida_UsaPrecoDoBancoETransacao()
    {
        PrepararJogo(1,preco:50m);PrepararJogo(2,preco:25m);
        Pedido? gravado=null;
        _pedidos.Setup(r=>r.InserirAsync(It.IsAny<Pedido>())).Callback<Pedido>(p=>{Assert.NotNull(Transaction.Current);gravado=p;}).ReturnsAsync(42);
        _itens.Setup(r=>r.InserirAsync(It.IsAny<ItemPedido>())).Callback<ItemPedido>(_=>Assert.NotNull(Transaction.Current)).ReturnsAsync(1);
        var service=CriarService();

        var resultado=await service.FinalizarAsync(7,new(){new(1,2),new(2,1)});

        Assert.NotNull(gravado);
        Assert.Equal(7,gravado.IdUsuario);
        Assert.Equal(SituacaoPedidoIds.Pendente,gravado.IdSituacao);
        Assert.Equal(DateTimeKind.Utc,gravado.DataPedido.Kind);
        Assert.Equal(125m,gravado.Total);
        Assert.Equal(42,resultado.Id);
        Assert.Equal(125m,resultado.Total);
        _jogos.Verify(r=>r.BaixarEstoqueAsync(1,2),Times.Once);
        _jogos.Verify(r=>r.BaixarEstoqueAsync(2,1),Times.Once);
        _itens.Verify(r=>r.InserirAsync(It.Is<ItemPedido>(i=>i.IdPedido==42&&i.IdJogo==1&&i.Quantidade==2&&i.ValorUnitario==50m)),Times.Once);
        _itens.Verify(r=>r.InserirAsync(It.Is<ItemPedido>(i=>i.IdPedido==42&&i.IdJogo==2&&i.Quantidade==1&&i.ValorUnitario==25m)),Times.Once);
    }

    [Fact]
    public async Task Finalizar_BaixaRecusada_AbortaTransacao()
    {
        PrepararJogo(1);
        _jogos.Setup(r=>r.BaixarEstoqueAsync(1,1)).ReturnsAsync(0);
        TransactionStatus? estado=null;
        _pedidos.Setup(r=>r.InserirAsync(It.IsAny<Pedido>())).Callback<Pedido>(_=>{
            Transaction.Current!.TransactionCompleted += (_,e)=>estado=e.Transaction!.TransactionInformation.Status;
        }).ReturnsAsync(42);
        var service=CriarService();

        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>service.FinalizarAsync(1,new(){new(1,1)}));

        Assert.Equal(BusinessErrors.ESTOQUE_INSUFICIENTE,erro.Erro);
        Assert.Equal(TransactionStatus.Aborted,estado);
        _itens.Verify(r=>r.InserirAsync(It.IsAny<ItemPedido>()),Times.Never);
    }

    [Fact]
    public async Task ListarMeus_UsuarioComPedidos_OrdenaEUsaPrecoPago()
    {
        var data=new DateTime(2026,11,27,12,0,0,DateTimeKind.Utc);
        _pedidos.Setup(r=>r.ListarPorUsuarioAsync(7)).ReturnsAsync(new List<Pedido>{
            new(){Id=1,IdUsuario=7,IdSituacao=1,DataPedido=data.AddDays(-1),Total=20m},
            new(){Id=2,IdUsuario=7,IdSituacao=1,DataPedido=data,Total=30m}
        });
        _itens.Setup(r=>r.ListarPorPedidosAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<ItemPedido>{
            new(){IdPedido=1,IdJogo=5,Quantidade=1,ValorUnitario=20m},
            new(){IdPedido=2,IdJogo=5,Quantidade=2,ValorUnitario=15m}
        });
        _jogos.Setup(r=>r.ListarPorIdsAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<Jogo>{new(){Id=5,Nome="Vector Search",Preco=100m,Ativo=false}});
        _situacoes.Setup(r=>r.ListarAsync()).ReturnsAsync(new List<SituacaoPedido>{new(){Id=1,Descricao="Pendente"}});
        var service=CriarService();

        var resultado=await service.ListarMeusAsync(7);

        Assert.Equal(new[]{2,1},resultado.Select(p=>p.Id).ToArray());
        Assert.Equal("Pendente",resultado[0].Situacao);
        Assert.Equal(15m,Assert.Single(resultado[0].Itens).ValorUnitario);
        Assert.Equal("Vector Search",resultado[0].Itens[0].NomeJogo);
        _pedidos.Verify(r=>r.ListarPorUsuarioAsync(7),Times.Once);
        _itens.Verify(r=>r.ListarPorPedidosAsync(It.Is<List<int>>(ids=>ids.SequenceEqual(new[]{1,2}))),Times.Once);
    }

    [Fact]
    public async Task ListarMeus_SemPedidos_RetornaVazio()
    {
        _pedidos.Setup(r=>r.ListarPorUsuarioAsync(7)).ReturnsAsync(new List<Pedido>());
        var resultado=await CriarService().ListarMeusAsync(7);
        Assert.Empty(resultado);
        _itens.Verify(r=>r.ListarPorPedidosAsync(It.IsAny<List<int>>()),Times.Never);
    }
}
