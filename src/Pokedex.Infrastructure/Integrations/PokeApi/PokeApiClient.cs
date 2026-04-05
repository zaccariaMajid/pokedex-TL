using System.Net;
using System.Text.Json;
using Pokedex.Application.Interfaces;
using Pokedex.Domain.Models;

namespace Pokedex.Infrastructure.Integrations.PokeApi;

public sealed class PokeApiClient(HttpClient httpClient) : IPokeApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<Pokemon?> GetPokemon(string name)
    {
        var response = await httpClient.GetAsync($"pokemon-species/{Uri.EscapeDataString(name)}");

        // A missing species is a valid lookup result for the application, not an infrastructure failure.
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        // Any other non-success status means the upstream API failed the request.
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var payload = await JsonSerializer.DeserializeAsync<PokeApiSpecies>(stream, JsonOptions);

        if (payload is null)
            throw new InvalidOperationException("PokéAPI returned an empty response.");

        var description = payload.FlavorTextEntries?
            .FirstOrDefault(entry => string.Equals(entry.Language?.Name, "en", StringComparison.OrdinalIgnoreCase))
            ?.FlavorText;

        return new Pokemon
        (
            Name: payload.Name ?? string.Empty,
            Description: NormalizeFlavorText(description),
            Habitat: string.IsNullOrWhiteSpace(payload.Habitat?.Name) ? "unknown" : payload.Habitat.Name,
            IsLegendary: payload.IsLegendary
        );
    }

    private static string NormalizeFlavorText(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // PokéAPI flavor text contains line breaks and form-feed characters.
        return string.Join(
            " ",
            text.Replace('\n', ' ')
                .Replace('\f', ' ')
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
