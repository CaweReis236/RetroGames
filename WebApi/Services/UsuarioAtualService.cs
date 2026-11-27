using System.Security.Claims;
using RetroGames.Model.Constantes;
using RetroGames.Services.Interfaces;
namespace RetroGames.WebApi.Services;
public class UsuarioAtualService(IHttpContextAccessor accessor) : IUsuarioAtualService
{
    public bool EstaAutenticado => accessor.HttpContext?.User.Identity?.IsAuthenticated == true;
    public int? IdUsuario => int.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : null;
    public bool EhAdministrador => EstaAutenticado && accessor.HttpContext?.User.IsInRole(Perfis.Administrador) == true;
}
