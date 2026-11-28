using Microsoft.AspNetCore.Identity;
using Moq;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Interfaces;
using RetroGames.Repository.Exceptions;
using RetroGames.Services;
using RetroGames.Services.Errors;
namespace RetroGames.WebApi.UnitTests.Services;
public class AutenticacaoServiceTests
{
    [Fact]
    public async Task Cadastrar_EmailLivre_CriaClienteAtivoComHash()
    {
        var repo=new Mock<IUsuarioRepository>();
        var hasher=new PasswordHasher<Usuario>();
        repo.Setup(r=>r.BuscarPorEmailAsync("cliente@example.test")).ReturnsAsync((Usuario?)null);
        repo.Setup(r=>r.InserirAsync(It.IsAny<Usuario>())).ReturnsAsync(17);
        var u=await new AutenticacaoService(repo.Object,hasher).CadastrarAsync("Cliente","cliente@example.test","senha-teste-exclusiva");
        Assert.Equal(17,u.Id);
        Assert.Equal(PerfilIds.Cliente,u.IdPerfil);
        Assert.True(u.Ativo);
        Assert.NotEqual("senha-teste-exclusiva",u.SenhaHash);
        Assert.NotEqual(PasswordVerificationResult.Failed,hasher.VerifyHashedPassword(u,u.SenhaHash,"senha-teste-exclusiva"));
    }
    [Fact]
    public async Task Cadastrar_EmailDuplicado_NaoGrava()
    {
        var repo=new Mock<IUsuarioRepository>();
        repo.Setup(r=>r.BuscarPorEmailAsync("cliente@example.test")).ReturnsAsync(new Usuario());
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>new AutenticacaoService(repo.Object,new PasswordHasher<Usuario>()).CadastrarAsync("Cliente","cliente@example.test","senha-teste-exclusiva"));
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO,erro.Erro);
        repo.Verify(r=>r.InserirAsync(It.IsAny<Usuario>()),Times.Never);
    }
    [Fact]
    public async Task Cadastrar_DuplicidadeConcorrente_ConverteErro()
    {
        var repo=new Mock<IUsuarioRepository>();
        repo.Setup(r=>r.BuscarPorEmailAsync("cliente@example.test")).ReturnsAsync((Usuario?)null);
        repo.Setup(r=>r.InserirAsync(It.IsAny<Usuario>())).ThrowsAsync(new RegistroDuplicadoException());
        var erro=await Assert.ThrowsAsync<BusinessRuleException>(()=>new AutenticacaoService(repo.Object,new PasswordHasher<Usuario>()).CadastrarAsync("Cliente","cliente@example.test","senha-teste-exclusiva"));
        Assert.Equal(BusinessErrors.EMAIL_JA_CADASTRADO,erro.Erro);
    }
}
