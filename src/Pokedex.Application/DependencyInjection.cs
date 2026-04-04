using Microsoft.Extensions.DependencyInjection;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Services;

namespace Pokedex.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IPokemonService, PokemonService>();
        services.AddScoped<ITranslationService, TranslationService>();

        return services;
    }
}
