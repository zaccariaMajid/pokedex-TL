using Microsoft.Extensions.Caching.Memory;
using Pokedex.Application.Interfaces;
using Pokedex.Domain.Models;

namespace Pokedex.Application.Services;

public sealed class PokemonService(IMemoryCache cache, IPokeApiClient pokeApiClient, ITranslationService translationService)
    : IPokemonService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public async Task<Pokemon?> GetPokemon(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var cacheKey = $"pokemon:{name.Trim().ToLowerInvariant()}";

        // Return a copy so callers cannot accidentally mutate the cached instance.
        if (cache.TryGetValue<Pokemon>(cacheKey, out var cachedPokemon) && cachedPokemon is not null)
        {
            return ClonePokemon(cachedPokemon);
        }

        var pokemon = await pokeApiClient.GetPokemon(name);

        if (pokemon is null)
            return null;

        // Cache only successful lookups and keep the cached value isolated from later mutations.
        cache.Set(cacheKey, ClonePokemon(pokemon), CacheDuration);

        return ClonePokemon(pokemon);
    }
    
    public async Task<Pokemon?> GetTranslatedPokemon(string name)
    {
        var pokemon = await GetPokemon(name);

        if (pokemon is null)
        {
            return null;
        }

        // Preserve the base Pokemon data and only replace the description for the translated variant.
        var translatedPokemon = ClonePokemon(pokemon);
        translatedPokemon.Description = await translationService.TranslateDescription(pokemon);

        return translatedPokemon;
    }

    private static Pokemon ClonePokemon(Pokemon pokemon)
    {
        // Pokemon is mutable, so callers should never share the cached/reference instance directly.
        return new Pokemon
        {
            Name = pokemon.Name,
            Description = pokemon.Description,
            Habitat = pokemon.Habitat,
            IsLegendary = pokemon.IsLegendary
        };
    }
}
