# Infrastructure Layer

## Overview

The infrastructure layer is responsible for outbound HTTP integrations. In the current solution, it contains typed clients for:

- PokéAPI, used to retrieve Pokemon species data
- FunTranslations, used to translate Pokemon descriptions

The abstractions for these integrations live in `Pokedex.Application.Interfaces`, while the concrete implementations live in `Pokedex.Infrastructure.Integrations`.

## Current structure

```text
src/Pokedex.Infrastructure
  DependencyInjection.cs
  Integrations/
    FunTranslations/
      TranslationApiClient.cs
      TranslationRequest.cs
      Translation.cs
      TranslationContents.cs
    PokeApi/
      PokeApiClient.cs
      PokeApiSpecies.cs
      FlavorTextEntry.cs
      Habitat.cs
      Language.cs
```

## Dependency flow

- `Pokedex.Infrastructure` depends on `Pokedex.Application` for integration interfaces
- `Pokedex.Infrastructure` depends on `Pokedex.Domain` for the `Pokemon` model
- Higher layers should depend on the interfaces, not on the concrete clients

## HTTP client registration

`DependencyInjection.AddInfrastructure()` registers both integrations as typed `HttpClient` clients with:

- a base address
- a 5 second timeout

This keeps transport configuration close to the concrete integration classes.

## PokeApi integration

### Abstraction

```csharp
public interface IPokeApiClient
{
    Task<Pokemon?> GetPokemon(string name);
}
```

### Responsibility

- Request `pokemon-species/{name}` from PokéAPI
- Deserialize the upstream payload into infrastructure DTOs
- Map the payload into the domain `Pokemon` model

### Mapping rules

- `Name` comes from the upstream `name`
- `Description` uses the first English `flavor_text_entries` value
- Description text is normalized to remove `\n` and `\f`
- `Habitat` defaults to `"unknown"` when the upstream payload does not include one
- `IsLegendary` maps directly from `is_legendary`

### Error handling

- `404` returns `null`
- Other non-success responses throw via `EnsureSuccessStatusCode()`
- A deserialized `null` payload throws `InvalidOperationException`

## FunTranslations integration

### Abstraction

```csharp
public interface ITranslationApiClient
{
    Task<string> TranslateYoda(string text);
    Task<string> TranslateShakespeare(string text);
}
```

### Responsibility

- Send text to the Yoda or Shakespeare translation endpoint
- Deserialize the translation payload
- Return the translated string

### Endpoints

- `yoda`
- `shakespeare`

Both are configured under the shared base URL:
`https://api.funtranslations.mercxry.me/v1/translate/`

### Error handling

- Non-success responses throw via `EnsureSuccessStatusCode()`
- Missing or empty translated text throws `InvalidOperationException`

## Constraints

This layer should contain:

- HTTP-specific concerns
- external DTOs
- mapping from external payloads into domain types
- DI registration for integration clients

This layer should not contain:

- API endpoint logic
- orchestration between use cases
- business rules such as deciding when Yoda vs Shakespeare should be used
- caching policies owned by the application layer
