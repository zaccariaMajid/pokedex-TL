using Pokedex.Domain.Models;

namespace Pokedex.Application.Interfaces;

public interface IPokemonService
{
    Task<Pokemon?> GetPokemon(string name);
    Task<Pokemon?> GetTranslatedPokemon(string name);
}
