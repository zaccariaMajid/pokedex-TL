using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.Integrations.PokeApi;

internal sealed class PokeApiSpecies
{
    public string? Name { get; init; }
    public Habitat? Habitat { get; init; }

    [JsonPropertyName("is_legendary")]
    public bool IsLegendary { get; init; }

    [JsonPropertyName("flavor_text_entries")]
    public IReadOnlyList<FlavorTextEntry>? FlavorTextEntries { get; init; }
}
