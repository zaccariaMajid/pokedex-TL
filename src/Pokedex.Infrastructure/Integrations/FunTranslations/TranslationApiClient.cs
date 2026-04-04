using System.Net.Http.Json;
using System.Text.Json;
using Pokedex.Application.Interfaces;

namespace Pokedex.Infrastructure.Integrations.FunTranslations;

public sealed class TranslationApiClient(HttpClient httpClient) : ITranslationApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public Task<string> TranslateYoda(string text)
    {
        return Translate("yoda", text);
    }

    public Task<string> TranslateShakespeare(string text)
    {
        return Translate("shakespeare", text);
    }

    // The endpoint segment selects the translation style while the request/response flow stays the same.
    private async Task<string> Translate(string endpoint, string text)
    {
        using var response = await httpClient.PostAsJsonAsync(endpoint, new TranslationRequest(text));
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        var payload = await JsonSerializer.DeserializeAsync<Translation>(stream, JsonOptions);
        var translatedText = payload?.Contents?.Translated;

        if (string.IsNullOrWhiteSpace(translatedText))
        {
            throw new InvalidOperationException("Translation API returned an invalid response.");
        }

        return translatedText;
    }
}
