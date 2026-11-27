using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Exceptions;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;
using RetroGames.Services.Errors;

namespace RetroGames.WebApi.UnitTests.Services;

public class PlataformaServiceTests
{
    private readonly Mock<IPlataformaRepository> _plataformas = new();
    private readonly Mock<IRepositorySet> _repositorio = new();

    private PlataformaService CriarService()
    {
        _repositorio.SetupGet(r => r.Plataforma).Returns(_plataformas.Object);
        return new PlataformaService(_repositorio.Object);
    }

    [Fact]
    public async Task BuscarPorId_IdInexistente_LancaNaoEncontrada()
    {
        // Arrange
        _plataformas.Setup(r => r.BuscarPorIdAsync(99)).ReturnsAsync((Plataforma?)null);
        var service = CriarService();

        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.BuscarPorIdAsync(99));

        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NAO_ENCONTRADA, erro.Erro);
    }

    [Fact]
    public async Task Inserir_NomeDuplicado_NaoGrava()
    {
        // Arrange
        var plataforma = new Plataforma { Nome = "PC" };
        _plataformas.Setup(r => r.ExisteComNomeAsync("PC", null)).ReturnsAsync(true);
        var service = CriarService();
        // Act
        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));
        // Assert
        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(It.IsAny<Plataforma>()), Times.Never);
    }

    [Fact]
    public async Task Inserir_RegistroDuplicadoNoRepository_LancaErroDeNegocio()
    {
        var plataforma = new Plataforma { Nome = "PC" };
        _plataformas.Setup(r => r.ExisteComNomeAsync("PC", null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(plataforma)).ThrowsAsync(new RegistroDuplicadoException());
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.InserirAsync(plataforma));

        Assert.Equal(BusinessErrors.PLATAFORMA_NOME_DUPLICADO, erro.Erro);
        _plataformas.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Inserir_NomeLivre_RetornaIdGerado()
    {
        var plataforma = new Plataforma { Nome = " PC " };
        _plataformas.Setup(r => r.ExisteComNomeAsync("PC", null)).ReturnsAsync(false);
        _plataformas.Setup(r => r.InserirAsync(plataforma)).ReturnsAsync(42);
        var service = CriarService();

        var resultado = await service.InserirAsync(plataforma);

        Assert.Equal(42, resultado.Id);
        Assert.Equal("PC", resultado.Nome);
        _plataformas.Verify(r => r.InserirAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Alterar_MesmoNomeDaPropriaPlataforma_AceitaAlteracao()
    {
        var plataforma = new Plataforma { Id = 1, Nome = "PC", Fabricante = "IBM" };
        _plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(plataforma);
        _plataformas.Setup(r => r.ExisteComNomeAsync("PC", 1)).ReturnsAsync(false);
        _plataformas.Setup(r => r.AlterarAsync(plataforma)).ReturnsAsync(1);
        var service = CriarService();

        var resultado = await service.AlterarAsync(plataforma);

        Assert.Same(plataforma, resultado);
        _plataformas.Verify(r => r.ExisteComNomeAsync("PC", 1), Times.Once);
        _plataformas.Verify(r => r.AlterarAsync(plataforma), Times.Once);
    }

    [Fact]
    public async Task Excluir_PlataformaPossuiJogos_NaoExclui()
    {
        _plataformas.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(new Plataforma { Id = 1 });
        _plataformas.Setup(r => r.PossuiJogosAsync(1)).ReturnsAsync(true);
        var service = CriarService();

        var erro = await Assert.ThrowsAsync<BusinessRuleException>(() => service.ExcluirAsync(1));

        Assert.Equal(BusinessErrors.PLATAFORMA_POSSUI_JOGOS, erro.Erro);
        _plataformas.Verify(r => r.ExcluirAsync(It.IsAny<int>()), Times.Never);
    }
}
