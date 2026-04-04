using Moq;
using Pokedex.Application.Interfaces;
using Pokedex.Application.Services;
using Pokedex.Domain.Models;

namespace Pokedex.Tests.Services;

[TestClass]
public sealed class TranslationServiceTests
{
    [TestMethod]
    public async Task TranslateDescription_UsesYodaForLegendaryPokemon()
    {
        var client = new Mock<ITranslationApiClient>();

        client
            .Setup(api => api.TranslateYoda("original-description"))
            .ReturnsAsync("legendary-yoda");

        var service = new TranslationService(client.Object);
        var pokemon = CreatePokemon(isLegendary: true, habitat: "forest");

        var result = await service.TranslateDescription(pokemon);

        Assert.AreEqual("legendary-yoda", result);
        // Verifying the client call is what proves the rule selection, not just the returned text.
        client.Verify(api => api.TranslateYoda("original-description"), Times.Once);
        client.Verify(api => api.TranslateShakespeare(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task TranslateDescription_UsesYodaForCavePokemon()
    {
        var client = new Mock<ITranslationApiClient>();

        client
            .Setup(api => api.TranslateYoda("original-description"))
            .ReturnsAsync("cave-yoda");

        var service = new TranslationService(client.Object);
        var pokemon = CreatePokemon(isLegendary: false, habitat: "cave");

        var result = await service.TranslateDescription(pokemon);

        Assert.AreEqual("cave-yoda", result);
        client.Verify(api => api.TranslateYoda("original-description"), Times.Once);
        client.Verify(api => api.TranslateShakespeare(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task TranslateDescription_UsesShakespeareForNonLegendaryNonCavePokemon()
    {
        var client = new Mock<ITranslationApiClient>();

        client
            .Setup(api => api.TranslateShakespeare("original-description"))
            .ReturnsAsync("normal-shakespeare");

        var service = new TranslationService(client.Object);
        var pokemon = CreatePokemon(isLegendary: false, habitat: "forest");

        var result = await service.TranslateDescription(pokemon);

        Assert.AreEqual("normal-shakespeare", result);
        client.Verify(api => api.TranslateYoda(It.IsAny<string>()), Times.Never);
        client.Verify(api => api.TranslateShakespeare("original-description"), Times.Once);
    }

    [TestMethod]
    public async Task TranslateDescription_ReturnsOriginalDescriptionWhenTranslationFails()
    {
        var client = new Mock<ITranslationApiClient>();

        client
            .Setup(api => api.TranslateShakespeare("base-description"))
            .ThrowsAsync(new HttpRequestException("translation failed"));

        var service = new TranslationService(client.Object);
        var pokemon = CreatePokemon(isLegendary: false, habitat: "forest", description: "base-description");

        var result = await service.TranslateDescription(pokemon);

        Assert.AreEqual("base-description", result);
        client.Verify(api => api.TranslateShakespeare("base-description"), Times.Once);
    }

    [TestMethod]
    public async Task TranslateDescription_ReturnsEmptyStringWhenDescriptionIsEmpty()
    {
        var client = new Mock<ITranslationApiClient>();
        var service = new TranslationService(client.Object);
        var pokemon = CreatePokemon(description: "");

        var result = await service.TranslateDescription(pokemon);

        Assert.AreEqual(string.Empty, result);
        client.Verify(api => api.TranslateYoda(It.IsAny<string>()), Times.Never);
        client.Verify(api => api.TranslateShakespeare(It.IsAny<string>()), Times.Never);
    }

    private static Pokemon CreatePokemon(
        bool isLegendary = false,
        string habitat = "forest",
        string description = "original-description")
    {
        return new Pokemon
        {
            Name = "mew",
            Description = description,
            Habitat = habitat,
            IsLegendary = isLegendary
        };
    }
}
