# API Layer

## Overview

The API layer exposes the application functionality through HTTP endpoints.

---

## Endpoints

### GET /pokemon/{name}

Returns basic information about a Pokémon, including:

* Name
* Description (standard, in English)
* Habitat
* Legendary status

This endpoint retrieves data through the application layer and returns a simplified response model.

---

### GET /pokemon/translated/{name}

Returns the same information as the base endpoint, but with a translated description.

The translation logic is handled entirely in the application layer and follows these rules:

* If the Pokémon is legendary or its habitat is "cave", a Yoda-style translation is applied
* Otherwise, a Shakespeare-style translation is used
* If the translation service fails, the original description is returned

---

## Request Handling

* Input is minimally validated (e.g. empty or invalid Pokémon name)
* All requests are processed asynchronously

---

## Response Behavior

* `200 OK` – Successful response
* `400 Bad Request` – Invalid input
* `404 Not Found` – Pokémon not found
* `500 Internal Server Error` – Unexpected failure

---

## Design Principles

* Controllers are thin and delegate all logic to the application layer
* No direct interaction with external services
* Consistent and predictable HTTP responses

---

## Error Handling

Unexpected errors are handled globally through middleware, ensuring consistent error responses across the API.
