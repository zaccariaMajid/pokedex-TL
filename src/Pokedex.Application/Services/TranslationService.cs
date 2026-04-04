using System.Text.Json;
using Pokedex.Application.Interfaces;
using Pokedex.Domain.Models;

namespace Pokedex.Application.Services;

public sealed class TranslationService(ITranslationApiClient translationApiClient) : ITranslationService
{
    public async Task<string> TranslateDescription(Pokemon pokemon)
    {
        ArgumentNullException.ThrowIfNull(pokemon);

        if (string.IsNullOrWhiteSpace(pokemon.Description))
        {
            return string.Empty;
        }

        try
        {
            // Legendary Pokemon and cave Pokemon use the Yoda translation rule.
            return ShouldUseYodaTranslation(pokemon)
                ? await translationApiClient.TranslateYoda(pokemon.Description)
                : await translationApiClient.TranslateShakespeare(pokemon.Description);
        }
        // Upstream translation failures are expected here, so the application falls back to the original text.
        catch (Exception ex) when (
            ex is HttpRequestException or
            TaskCanceledException or
            JsonException or
            InvalidOperationException)
        {
            return pokemon.Description;
        }
    }

    private static bool ShouldUseYodaTranslation(Pokemon pokemon)
    {
        return pokemon.IsLegendary || string.Equals(pokemon.Habitat, "cave", StringComparison.OrdinalIgnoreCase);
    }
}
