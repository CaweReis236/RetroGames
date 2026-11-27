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
}
