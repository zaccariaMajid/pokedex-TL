# Pokemon fun pokedex

**Fun Pokédex** is a RESTful API designed to provide structured and comprehensive information about Pokémon, with a twist.

## Integrations

- Pokémon data is retrieved from the public PokéAPI, specifically the Pokémon species endpoint.
- Description translations are handled via the FunTranslations API, using either the Yoda or Shakespeare translation based on the defined rules.

## Assumptions

- The first available English flavor text is used as the Pokémon description
- External APIs may fail or rate limit requests, so a fallback strategy is applied

## Design decisions & trade-offs

- Controllers were chosen over minimal APIs to provide a clearer structure and better separation of concerns

## Error handling

- If the translation API fails, the original description is returned as a fallback
- External API calls are wrapped in dedicated clients. Currently only basic error handling is implemented. In production I would introduce retry policies and timeouts to handle transient failures.

## Testing

Unit tests focus on:
- Translation decision logic (Yoda vs Shakespeare)
- Fallback behavior when translation fails

## Performance considerations

- Caching is implemented at the service layer using the built-in .NET MemoryCache to reduce repeated external API calls for the same Pokémon.
- In a production environment, a distributed cache (e.g. Redis) would be more appropriate.

## Possible improvements

- Introduce resilience policies (retry, circuit breaker)
- Add integration tests
- Improve observability with structured logging

## Project Structure

The project is implemented as a .NET Web API.

The solution uses the `.sln` format to organize multiple projects.

The project is organized as follows:


```
/src
  /Pokedex.Api
    Controllers/
      PokemonController.cs
    Program.cs

  /Pokedex.Application
    DependencyInjection.cs
    Interfaces/
      IPokeApiClient.cs
      IPokemonService.cs
      ITranslationApiClient.cs
      ITranslationService.cs
    Services/
      PokemonService.cs
      TranslationService.cs

  /Pokedex.Domain
    Models/
      Pokemon.cs

  /Pokedex.Infrastructure
    DependencyInjection.cs
    Integrations/
      /FunTranslations
        TranslationApiClient.cs
        TranslationRequest.cs
        Translation.cs
        TranslationContents.cs
      /PokeApi
        PokeApiClient.cs
        PokeApiSpecies.cs
        FlavorTextEntry.cs
        Habitat.cs
        Language.cs

/tests
  /Pokedex.Tests
```

The project is structured following Clean Architecture principles, with a clear separation of responsibilities between layers.

The API layer is responsible only for handling HTTP requests and responses, keeping controllers thin and focused.

The application layer contains the core logic of the system. It orchestrates the flow of data, applies caching for base Pokémon lookups, and handles translation fallback behavior while relying on interfaces rather than concrete implementations.

The domain layer defines the core models used across the application, keeping them independent from external concerns.

The infrastructure layer handles all interactions with external services, such as the PokéAPI and the translation APIs.

This approach keeps the codebase easy to understand, test, and extend, while making it straightforward to replace external dependencies if needed.

## API
Example base URL: `http://localhost:5000`

The API exposes two endpoints to retrieve Pokémon information:

### GET /pokemon/{name}

Returns basic information about a Pokémon, including:

* Name
* Description (standard, in English)
* Habitat
* Legendary status

This endpoint retrieves data from the PokéAPI and maps it into a simplified response model.

---

### GET /pokemon/translated/{name}

Returns the same information as the base endpoint, but with a translated description.

The translation follows these rules:

* If the Pokémon is legendary or its habitat is "cave", a Yoda-style translation is applied
* Otherwise, a Shakespeare-style translation is used
* If the translation service is unavailable, the original description is returned as a fallback

---

Both endpoints are designed to be simple and predictable:

* `200 OK` when the Pokémon is found
* `404 Not Found` when the Pokémon does not exist

Base Pokémon lookups are cached in memory for 5 minutes to reduce repeated calls to PokéAPI.
