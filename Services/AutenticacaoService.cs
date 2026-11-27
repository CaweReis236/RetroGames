using Microsoft.AspNetCore.Identity;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
using RetroGames.Repository.Interfaces;
using RetroGames.Services.Interfaces;
namespace RetroGames.Services;
public class AutenticacaoService(IUsuarioRepository repositorio, IPasswordHasher<Usuario> hasher) : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await repositorio.BuscarPorEmailAsync(email.Trim());
        if (usuario is null || !usuario.Ativo) return null;
        return hasher.VerifyHashedPassword(usuario, usuario.SenhaHash, senha) == PasswordVerificationResult.Failed ? null : usuario;
    }
    public async Task GarantirAdministradorInicialAsync(string? email, string? senha)
    {
        if (await repositorio.ExisteAdministradorAsync()) return;
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || senha == "SUA_SENHA")
            throw new InvalidOperationException("Configure AdminInicial__Email e AdminInicial__Senha no .env.");
        var usuario = new Usuario { Nome = "Administrador", Email = email.Trim(), Ativo = true,
            IdPerfil = PerfilIds.Administrador, DataCadastro = DateTime.UtcNow };
        usuario.SenhaHash = hasher.HashPassword(usuario, senha);
        await repositorio.InserirAsync(usuario);
    }
}
