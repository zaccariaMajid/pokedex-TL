using Microsoft.AspNetCore.Mvc;
using Pokedex.Api.Contracts;
using Pokedex.Application.Interfaces;

namespace Pokedex.Api.Controllers;

[ApiController]
[Route("pokemon")]
public sealed class PokemonController(IPokemonService pokemonService) : ControllerBase
{
    [HttpGet("{name}")]
    [ProducesResponseType<PokemonResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PokemonResponse>> GetPokemon(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(CreateInvalidNameProblemDetails());
        }

        var pokemon = await pokemonService.GetPokemon(name);

        if (pokemon is null)
        {
            return NotFound(CreateNotFoundProblemDetails(name));
        }

        return Ok(PokemonResponse.FromDomain(pokemon));
    }

    [HttpGet("translated/{name}")]
    [ProducesResponseType<PokemonResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PokemonResponse>> GetTranslatedPokemon(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(CreateInvalidNameProblemDetails());
        }

        var pokemon = await pokemonService.GetTranslatedPokemon(name);

        if (pokemon is null)
        {
            return NotFound(CreateNotFoundProblemDetails(name));
        }

        return Ok(PokemonResponse.FromDomain(pokemon));
    }

    private static ProblemDetails CreateInvalidNameProblemDetails()
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid pokemon name",
            Detail = "Pokemon name must not be empty or whitespace."
        };
    }

    private static ProblemDetails CreateNotFoundProblemDetails(string name)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Pokemon not found",
            Detail = $"Pokemon '{name}' was not found."
        };
    }
}
