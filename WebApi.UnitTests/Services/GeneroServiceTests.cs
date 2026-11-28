using Moq;
using RetroGames.Model.Model;
using RetroGames.Repository.Interfaces;
using RetroGames.Services;

namespace RetroGames.WebApi.UnitTests.Services;

public class GeneroServiceTests
{
    [Fact]
    public async Task Listar_ComGeneros_RetornaDadosDoRepository()
    {
        var generos = new List<Genero> { new() { Id = 1, Nome = "Aventura" } };
        var mock = new Mock<IGeneroRepository>();
        mock.Setup(r => r.ListarAsync()).ReturnsAsync(generos);
        var repositorio = new Mock<IRepositorySet>();
        repositorio.SetupGet(r => r.Genero).Returns(mock.Object);
        var service = new GeneroService(repositorio.Object);

        var resultado = await service.ListarAsync();

        Assert.Same(generos, resultado);
        mock.Verify(r => r.ListarAsync(), Times.Once);
    }
}
