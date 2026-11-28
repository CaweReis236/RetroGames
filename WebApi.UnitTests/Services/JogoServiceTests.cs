using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Filtros;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Errors;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.UnitTests.Services;

public class JogoServiceTests
{
    private readonly Mock<IRepositorySet> _repositorio = new();
    private readonly Mock<IJogoRepository> _jogos = new();
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<ICondicaoJogoRepository> _condicoes = new();
    private readonly Mock<IGeneroRepository> _generos = new();
    private readonly Mock<IJogoGeneroRepository> _vinculos = new();
    private readonly Mock<IUsuarioAtualService> _usuario = new();

    private JogoService CriarService()
    {
        _repositorio.SetupGet(r => r.Jogo).Returns(_jogos.Object);
        _repositorio.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        _repositorio.SetupGet(r => r.CondicaoJogo).Returns(_condicoes.Object);
        _repositorio.SetupGet(r => r.Genero).Returns(_generos.Object);
        _repositorio.SetupGet(r => r.JogoGenero).Returns(_vinculos.Object);
        return new JogoService(_repositorio.Object, _usuario.Object);
    }

    private static Jogo CriarJogo() => new()
    {
        Id = 10, Nome = "Vector Search", IdPlataforma = 1,
        IdCondicaoJogo = 2, Ativo = true, Preco = 49.90m, Estoque = 10
    };

    private void PrepararReferencias()
    {
        _plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1, Nome = "PC" });
        _plataformas.Setup(r => r.ListarAsync()).ReturnsAsync(new List<Plataforma> { new() { Id = 1, Nome = "PC" } });
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync(new List<CondicaoJogo> { new() { Id = 2, Descricao = "Novo" } });
        _generos.Setup(r => r.ListarAsync()).ReturnsAsync(new List<Genero> { new() { Id = 3, Nome = "Aventura" } });
        _vinculos.Setup(r => r.ListarPorJogosAsync(It.IsAny<List<int>>())).ReturnsAsync(new List<JogoGenero> { new() { IdJogo = 10, IdGenero = 3 } });
    }

    private void VerificarSemGravacao()
    {
        _jogos.Verify(r => r.InserirAsync(It.IsAny<Jogo>()), Times.Never);
        _vinculos.Verify(r => r.InserirAsync(It.IsAny<JogoGenero>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_PlataformaInexistente_NaoGrava()
    {
        _plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync((Plataforma?)null);
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(CriarJogo(), new List<int> { 3 }));

        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Inserir_CondicaoInexistente_NaoGrava()
    {
        PrepararReferencias();
        _condicoes.Setup(r => r.ListarAsync()).ReturnsAsync(new List<CondicaoJogo>());
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(CriarJogo(), new List<int> { 3 }));

        Assert.Equal(BusinessErrors.CONDICAO_NAO_ENCONTRADA, erro.Erro);
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Inserir_GeneroInexistente_NaoGrava()
    {
        PrepararReferencias();
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(CriarJogo(), new List<int> { 999 }));

        Assert.Equal(BusinessErrors.GENERO_NAO_ENCONTRADO, erro.Erro);
        VerificarSemGravacao();
    }

    [Fact]
    public async Task Listar_ComGenero_EnviaSomenteIdsDoGenero()
    {
        _vinculos.Setup(r => r.ListarTodosAsync()).ReturnsAsync(new List<JogoGenero>
        {
            new() { IdJogo = 10, IdGenero = 3 },
            new() { IdJogo = 20, IdGenero = 3 },
            new() { IdJogo = 30, IdGenero = 4 }
        });
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync((new List<Jogo>(), 0));
        var service = CriarService();

        await service.ListarAsync(1, 12, idGenero: 3);

        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f =>
            f.IdsJogos != null && f.IdsJogos.SequenceEqual(new[] { 10, 20 }) &&
            f.Pagina == 1 && f.TamanhoPagina == 12)), Times.Once);
    }

    [Fact]
    public async Task Listar_SemGenero_NaoRestringeIds()
    {
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync((new List<Jogo>(), 0));
        var service = CriarService();

        await service.ListarAsync(1, 12);

        _vinculos.Verify(r => r.ListarTodosAsync(), Times.Never);
        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f => f.IdsJogos == null)), Times.Once);
    }

    [Fact]
    public async Task Listar_ComJogos_MontaDetalhesComLinq()
    {
        PrepararReferencias();
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync((new List<Jogo> { CriarJogo() }, 1));
        var service = CriarService();

        var resultado = await service.ListarAsync(1, 12);

        var detalhe = Assert.Single(resultado.Jogos);
        Assert.Equal(1, resultado.Total);
        Assert.Equal("PC", detalhe.NomePlataforma);
        Assert.Equal("Novo", detalhe.DescricaoCondicao);
        Assert.Equal("Aventura", Assert.Single(detalhe.Generos).Nome);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrado()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(99));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }

    [Fact]
    public async Task Desativar_IdInexistente_NaoAlteraStatus()
    {
        _jogos.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Jogo?)null);
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.DesativarAsync(99));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
        _jogos.Verify(r => r.AlterarStatusAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, true)]
    public async Task Listar_IncluirInativos_RespeitaPerfil(bool administrador, bool esperado)
    {
        _usuario.SetupGet(u => u.EhAdministrador).Returns(administrador);
        _jogos.Setup(r => r.ListarAsync(It.IsAny<JogoFiltro>())).ReturnsAsync((new List<Jogo>(), 0));
        var service = CriarService();

        await service.ListarAsync(1, 12, incluirInativos: true);

        _jogos.Verify(r => r.ListarAsync(It.Is<JogoFiltro>(f => f.IncluirInativos == esperado)), Times.Once);
    }

    [Fact]
    public async Task BuscarPorId_InativoComoVisitante_LancaNaoEncontrado()
    {
        var jogo = CriarJogo();
        jogo.Ativo = false;
        _jogos.Setup(r => r.BuscarPorIdAsync(jogo.Id)).ReturnsAsync(jogo);
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(jogo.Id));

        Assert.Equal(BusinessErrors.JOGO_NAO_ENCONTRADO, erro.Erro);
    }
}
