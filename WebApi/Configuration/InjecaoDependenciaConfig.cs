using Microsoft.Extensions.DependencyInjection;

using RetroGames.Repository;
using RetroGames.Repository.Config;
using RetroGames.Repository.Interfaces;

using RetroGames.Services;
using RetroGames.Services.Interfaces;

namespace RetroGames.WebApi.Configuration;

public static class InjecaoDependenciaConfig
{
    public static IServiceCollection AddRepositorios(
        this IServiceCollection services,
        string connectionString)
    {
        // Configuração compartilhada entre os repositórios.
        services.AddSingleton(new RepositoryConfig
        {
            ConnectionString = connectionString
        });

        // Repositórios: uma instância por escopo HTTP.
        services.AddScoped<IPlataformaRepository, PlataformaRepository>();

        services.AddScoped<IJogoRepository, JogoRepository>();

        services.AddScoped<IJogoGeneroRepository, JogoGeneroRepository>();

        services.AddScoped<IGeneroRepository, GeneroRepository>();

        services.AddScoped<ICondicaoJogoRepository, CondicaoJogoRepository>();

        // Agrupador dos repositórios.
        services.AddScoped<IRepositorySet, RepositorySet>();

        return services;
    }

    public static IServiceCollection AddServicosDeNegocio(
        this IServiceCollection services)
    {
        services.AddScoped<IPlataformaService, PlataformaService>();

        services.AddScoped<IJogoService, JogoService>();

        services.AddScoped<IGeneroService, GeneroService>();

        services.AddScoped<ICondicaoJogoService, CondicaoJogoService>();

        return services;
    }
}
