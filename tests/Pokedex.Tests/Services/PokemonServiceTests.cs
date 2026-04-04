using Microsoft.Extensions.Caching.Memory;
using Moq;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Services;
using Pokedex.Domain.Models;

namespace Pokedex.Tests.Services;

[TestClass]
public sealed class PokemonServiceTests
{
    [TestMethod]
    public async Task GetPokemon_ReturnsPokemonFromClient()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("mew"))
            .ReturnsAsync(CreatePokemon());

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var result = await service.GetPokemon("mew");

        Assert.IsNotNull(result);
        Assert.AreEqual("mew", result.Name);
        Assert.AreEqual("base-description", result.Description);
        pokeApiClient.Verify(client => client.GetPokemon("mew"), Times.Once);
    }

    [TestMethod]
    public async Task GetPokemon_ReturnsNullWhenPokemonDoesNotExist()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("missing"))
            .ReturnsAsync((Pokemon?)null);

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var result = await service.GetPokemon("missing");

        Assert.IsNull(result);
        pokeApiClient.Verify(client => client.GetPokemon("missing"), Times.Once);
    }

    [TestMethod]
    public async Task GetPokemon_UsesCacheForRepeatedLookups()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("mew"))
            .ReturnsAsync(CreatePokemon());

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var firstResult = await service.GetPokemon("mew");
        var secondResult = await service.GetPokemon("mew");

        Assert.IsNotNull(firstResult);
        Assert.IsNotNull(secondResult);
        // Cached reads should still return fresh clones so callers cannot share mutable state.
        Assert.AreNotSame(firstResult, secondResult);
        pokeApiClient.Verify(client => client.GetPokemon("mew"), Times.Once);
    }

    [TestMethod]
    public async Task GetPokemon_ReturnsCloneInsteadOfCachedInstance()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("mew"))
            .ReturnsAsync(CreatePokemon());

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var firstResult = await service.GetPokemon("mew");

        Assert.IsNotNull(firstResult);
        firstResult.Description = "mutated-description";

        var secondResult = await service.GetPokemon("mew");

        Assert.IsNotNull(secondResult);
        Assert.AreEqual("base-description", secondResult.Description);
    }

    [TestMethod]
    public async Task GetTranslatedPokemon_ReusesBasePokemonAndReplacesOnlyDescription()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("mew"))
            .ReturnsAsync(CreatePokemon());

        Pokemon? translatedInput = null;
        // Capture the Pokemon passed into the translation service so the test can verify
        // that only the description changes in the translated result.
        translationService
            .Setup(service => service.TranslateDescription(It.IsAny<Pokemon>()))
            .Callback<Pokemon>(pokemon => translatedInput = pokemon)
            .ReturnsAsync("translated-description");

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var result = await service.GetTranslatedPokemon("mew");

        Assert.IsNotNull(result);
        Assert.AreEqual("mew", result.Name);
        Assert.AreEqual("translated-description", result.Description);
        Assert.AreEqual("forest", result.Habitat);
        Assert.IsTrue(result.IsLegendary);
        pokeApiClient.Verify(client => client.GetPokemon("mew"), Times.Once);
        translationService.Verify(service => service.TranslateDescription(It.IsAny<Pokemon>()), Times.Once);
        Assert.IsNotNull(translatedInput);
        Assert.AreEqual("base-description", translatedInput.Description);
        Assert.AreEqual("forest", translatedInput.Habitat);
        Assert.IsTrue(translatedInput.IsLegendary);
    }

    [TestMethod]
    public async Task GetTranslatedPokemon_ReturnsNullWhenBasePokemonDoesNotExist()
    {
        using var cache = CreateCache();
        var pokeApiClient = new Mock<IPokeApiClient>();
        var translationService = new Mock<ITranslationService>();

        pokeApiClient
            .Setup(client => client.GetPokemon("missing"))
            .ReturnsAsync((Pokemon?)null);

        var service = new PokemonService(cache, pokeApiClient.Object, translationService.Object);

        var result = await service.GetTranslatedPokemon("missing");

        Assert.IsNull(result);
        pokeApiClient.Verify(client => client.GetPokemon("missing"), Times.Once);
        translationService.Verify(service => service.TranslateDescription(It.IsAny<Pokemon>()), Times.Never);
    }

    private static MemoryCache CreateCache()
    {
        return new MemoryCache(new MemoryCacheOptions());
    }

    private static Pokemon CreatePokemon()
    {
        return new Pokemon
        {
            Name = "mew",
            Description = "base-description",
            Habitat = "forest",
            IsLegendary = true
        };
    }
}
