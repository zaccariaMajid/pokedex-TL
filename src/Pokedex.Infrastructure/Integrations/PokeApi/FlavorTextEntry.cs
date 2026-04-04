using System.Text.Json.Serialization;

namespace Pokedex.Infrastructure.Integrations.PokeApi;

internal sealed class FlavorTextEntry
{
    [JsonPropertyName("flavor_text")]
    public string? FlavorText { get; init; }

    public Language? Language { get; init; }
}
