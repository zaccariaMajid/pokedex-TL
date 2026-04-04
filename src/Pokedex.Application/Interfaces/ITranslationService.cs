using Pokedex.Domain.Models;

namespace Pokedex.Application.Interfaces;

public interface ITranslationService
{
    Task<string> TranslateDescription(Pokemon pokemon);
}
