# Testing

## Overview

The testing strategy focuses on covering the most relevant and valuable parts of the application.

The goal is to validate core business logic, ensure correct behavior under edge cases, and verify resilience when interacting with external services.

---

## Testing Approach

Tests are primarily focused on the Application layer, where the core logic resides.

External dependencies such as API clients are mocked with `Moq` so tests remain fast, deterministic, and isolated.

---

## What is Tested

### Translation Logic

* Correct selection between Yoda and Shakespeare translation
* Behavior when the Pokémon is legendary
* Behavior when the habitat is "cave"
* Empty descriptions short-circuit without calling the translation client

### Fallback Behavior

* If the translation API fails, the original description is returned
* Ensures resilience against external service failures

### Pokémon Retrieval

* Returning Pokémon data retrieved from the application-facing API client
* Returning `null` when the Pokémon is not found
* Producing a translated Pokémon by preserving the base data and replacing only the description

### Caching Behavior

* Repeated requests for the same Pokémon do not trigger additional external API calls
* Ensures caching is correctly applied at the service level
* Returned models are clones, so callers cannot mutate the cached instance

## What is Not Tested

* External API clients (PokéAPI, translation APIs)
* Framework-specific behavior (ASP.NET routing, controllers)
* DTO-to-domain mapping performed inside infrastructure clients

These are considered outside the scope of unit testing.

## Design Principles

* Tests are focused on behavior, not implementation details
* MSTest is used as the test framework
* `Moq` is used to isolate the system under test and verify expected collaborator calls
* Edge cases and failure scenarios are explicitly validated
