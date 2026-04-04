# Application Layer

## Overview

The Application layer contains the core logic of the system. It is responsible for orchestrating the flow of data between external services and the API layer.

## Components

### PokemonService

The `PokemonService` is responsible for retrieving Pokémon data and exposing it in a simplified format.

Responsibilities:

* Retrieve Pokémon data via the `IPokeApiClient`
* Apply caching to reduce repeated external API calls
* Provide both base and translated Pokémon information

### TranslationService

The `TranslationService` encapsulates the logic used to determine how a Pokémon description should be translated.

Responsibilities:

* Decide whether to use Yoda or Shakespeare translation
* Delegate translation to the `ITranslationApiClient`
* Apply a fallback strategy when the translation service fails

## Business Rules

The translation logic follows these rules:

* If the Pokémon is legendary or its habitat is "cave", a Yoda-style translation is applied
* Otherwise, a Shakespeare-style translation is used
* If the translation fails, the original description is returned

## Design Principles

* Services depend on abstractions (interfaces), not concrete implementations
* Business logic is kept isolated from infrastructure concerns
* Each service has a single, well-defined responsibility

## Error Handling

The Application layer is responsible for handling expected failures from external services (e.g. translation errors) and applying fallback strategies where appropriate.

Unexpected errors are propagated and handled at a higher level.

## Current Implementation

### Interfaces

```csharp
public interface IPokemonService
{
    Task<Pokemon?> GetPokemon(string name);
    Task<Pokemon?> GetTranslatedPokemon(string name);
}

public interface ITranslationService
{
    Task<string> TranslateDescription(Pokemon pokemon);
}
```

### PokemonService

The implemented `PokemonService`:

* Uses `IPokeApiClient` to retrieve the base Pokémon
* Caches successful base lookups in memory for 5 minutes
* Rejects blank Pokémon names with `ArgumentException`
* Returns `null` when the Pokémon is not found
* Implements `GetTranslatedPokemon` by reusing `GetPokemon`, then cloning the base result and replacing only the description
* Produces translated responses by combining the cached/base Pokémon with `ITranslationService`
* Clones Pokémon instances before returning them so cached state is not mutated by consumers

### TranslationService

The implemented `TranslationService`:

* Uses Yoda translation when the Pokémon is legendary or its habitat is `"cave"`
* Uses Shakespeare translation otherwise
* Returns the original description when expected upstream translation failures occur
* Returns an empty string immediately when the source description is empty

## Composition

`AddApplication()` registers:

* `IMemoryCache`
* `IPokemonService` as `PokemonService`
* `ITranslationService` as `TranslationService`
