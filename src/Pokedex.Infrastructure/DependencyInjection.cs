using Microsoft.Extensions.DependencyInjection;
using Pokedex.Application.Interfaces;
using Pokedex.Infrastructure.Integrations.FunTranslations;
using Pokedex.Infrastructure.Integrations.PokeApi;

namespace Pokedex.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Typed clients keep external API configuration close to the concrete integration.
        services.AddHttpClient<IPokeApiClient, PokeApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        services.AddHttpClient<ITranslationApiClient, TranslationApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.funtranslations.mercxry.me/v1/translate/");
            client.Timeout = TimeSpan.FromSeconds(5);
        });

        return services;
    }
}
