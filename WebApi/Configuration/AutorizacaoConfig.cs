using Microsoft.AspNetCore.Authorization;
using RetroGames.Model.Constantes;
namespace RetroGames.WebApi.Configuration;
public static class Politicas { public const string Administrador = "SomenteAdministrador"; }
public static class AutorizacaoConfig
{
    public static IServiceCollection AddPoliticas(this IServiceCollection services) => services.AddAuthorization(opcoes =>
    {
        opcoes.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        opcoes.AddPolicy(Politicas.Administrador, p => p.RequireAuthenticatedUser().RequireRole(Perfis.Administrador));
    });
}
