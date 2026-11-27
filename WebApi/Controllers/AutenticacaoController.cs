using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RetroGames.Services.Interfaces;
using RetroGames.WebApi.Extensions;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AutenticacaoController(IAutenticacaoService service) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestApiModel request)
    {
        var usuario = await service.AutenticarAsync(request.Email, request.Senha);
        if (usuario is null) return Unauthorized(new RetroGamesResponseApiModel { ResponseCode = ResponseCodes.CREDENCIAIS_INVALIDAS.Code, ErrorMessage = ResponseCodes.CREDENCIAIS_INVALIDAS.DefaultMessage });
        var principal = usuario.CriarPrincipal();
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return Ok(RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>.Sucesso(ParaResponse(principal)));
    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(RetroGamesResponseApiModel.Sucesso());
    }
    [HttpGet("eu")]
    public IActionResult Eu() => Ok(RetroGamesResponseApiModel<UsuarioLogadoResponseApiModel>.Sucesso(ParaResponse(User)));
    private static UsuarioLogadoResponseApiModel ParaResponse(ClaimsPrincipal usuario) => new()
    {
        Id = int.Parse(usuario.FindFirstValue(ClaimTypes.NameIdentifier)!),
        Nome = usuario.FindFirstValue(ClaimTypes.Name) ?? "",
        Email = usuario.FindFirstValue(ClaimTypes.Email) ?? "",
        Perfil = usuario.FindFirstValue(ClaimTypes.Role) ?? ""
    };
}
