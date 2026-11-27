using Microsoft.AspNetCore.Authentication.Cookies;
using RetroGames.WebApi.ApiModels;
namespace RetroGames.WebApi.Configuration;
public static class AutenticacaoConfig
{
    public static IServiceCollection AddSessao(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o =>
        {
            o.Cookie.Name = "retrogames.sessao";
            o.Cookie.HttpOnly = true;
            o.Cookie.SameSite = SameSiteMode.Strict;
            o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            o.ExpireTimeSpan = TimeSpan.FromHours(2);
            o.SlidingExpiration = true;
            o.Events.OnRedirectToLogin = c => Responder(c.Response, 401, ResponseCodes.NAO_AUTENTICADO);
            o.Events.OnRedirectToAccessDenied = c => Responder(c.Response, 403, ResponseCodes.ACESSO_NEGADO);
        });
        return services;
    }
    private static Task Responder(HttpResponse resposta, int status, (int Code, string DefaultMessage) erro)
    {
        resposta.StatusCode = status;
        return resposta.WriteAsJsonAsync(new RetroGamesResponseApiModel { ResponseCode = erro.Code, ErrorMessage = erro.DefaultMessage });
    }
}
