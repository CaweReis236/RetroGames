using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class CondicaoJogoServiceTests
{
    [Fact]
    public async Task Listar_ComCondicoes_RetornaDadosDoRepository()
    {
        var condicoes = new List<CondicaoJogo> { new() { Id = 1, Descricao = "Novo" } };
        var mock = new Mock<ICondicaoJogoRepository>();
        mock.Setup(r => r.ListarAsync()).ReturnsAsync(condicoes);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.CondicaoJogo).Returns(mock.Object);
        var service = new CondicaoJogoService(repositorio.Object);

        var resultado = await service.ListarAsync();

        Assert.Same(condicoes, resultado);
        mock.Verify(r => r.ListarAsync(), Times.Once);
    }
}
