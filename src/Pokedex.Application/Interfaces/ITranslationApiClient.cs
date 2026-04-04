namespace Pokedex.Application.Interfaces;

public interface ITranslationApiClient
{
    Task<string> TranslateYoda(string text);
    Task<string> TranslateShakespeare(string text);
}
