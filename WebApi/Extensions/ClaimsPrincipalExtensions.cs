using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.Model.Model;
using RetroGames.Model.Constantes;
namespace RetroGames.WebApi.Extensions;
public static class ClaimsPrincipalExtensions
{
    public static string NomePerfil(this Usuario usuario) => usuario.IdPerfil == PerfilIds.Administrador ? Perfis.Administrador : Perfis.Cliente;
    public static ClaimsPrincipal CriarPrincipal(this Usuario usuario) => new(new ClaimsIdentity(new[] {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), new Claim(ClaimTypes.Name, usuario.Nome),
        new Claim(ClaimTypes.Email, usuario.Email), new Claim(ClaimTypes.Role, usuario.NomePerfil())
    }, CookieAuthenticationDefaults.AuthenticationScheme));
}
