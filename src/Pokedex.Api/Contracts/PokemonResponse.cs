using Pokedex.Domain.Models;

namespace Pokedex.Api.Contracts;

public sealed class PokemonResponse
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Habitat { get; init; } = string.Empty;
    public bool IsLegendary { get; init; }

    public static PokemonResponse FromDomain(Pokemon pokemon)
    {
        return new PokemonResponse
        {
            Name = pokemon.Name,
            Description = pokemon.Description,
            Habitat = pokemon.Habitat,
            IsLegendary = pokemon.IsLegendary
        };
    }
}
