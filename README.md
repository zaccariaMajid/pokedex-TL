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
- Basic error handling is applied for external API failures

## Testing

Unit tests focus on:
- Translation decision logic (Yoda vs Shakespeare)
- Fallback behavior when translation fails

## Performance considerations

A simple in-memory cache is used to reduce calls to external APIs and improve response time.

In a production environment, a distributed cache (e.g. Redis) would be more appropriate.

## Possible improvements

- Introduce resilience policies (retry, circuit breaker)
- Add integration tests
- Improve observability with structured logging

## Project Structure

The project is organized as follows:

```
/src
  /Pokedex.Api
    Controllers/
      PokemonController.cs

  /Pokedex.Application
    Interfaces/
      IPokemonService.cs
      ITranslationService.cs

    Services/
      PokemonService.cs
      TranslationService.cs

  /Pokedex.Domain
    Models/
      Pokemon.cs

  /Pokedex.Infrastructure
    Clients/
      PokeApiClient.cs
      TranslationApiClient.cs

/tests
  /Pokedex.Tests
```

The project is structured following Clean Architecture principles, with a clear separation of responsibilities between layers.

The API layer is responsible only for handling HTTP requests and responses, keeping controllers thin and focused.

The application layer contains the core logic of the system. It orchestrates the flow of data and defines the main use cases, relying on interfaces rather than concrete implementations.

The domain layer defines the core models used across the application, keeping them independent from external concerns.

The infrastructure layer handles all interactions with external services, such as the PokéAPI and the translation APIs.

This approach keeps the codebase easy to understand, test, and extend, while making it straightforward to replace external dependencies if needed.
