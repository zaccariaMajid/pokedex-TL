using Pokedex.Domain.Models;

namespace Pokedex.Application.Interfaces;

public interface IPokeApiClient
{
    Task<Pokemon?> GetPokemon(string name);
}
