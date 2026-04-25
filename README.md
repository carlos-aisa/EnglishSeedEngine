# EnglishSeed Engine

Backend-first project for adaptive English learning focused on testing quality.

## Current Scope

- Modular monolith structure (`Domain`, `Application`, `Infrastructure`, `Api`).
- Student registration API.
- Health endpoint.
- Integration tests with real PostgreSQL in Docker (`Testcontainers` + `Respawn`).

## Run API

```bash
dotnet run --project src/EnglishSeedEngine.Api
```

## Run Tests

```bash
dotnet test
```

## Solution Layout

```text
src/
  EnglishSeedEngine.Domain/
  EnglishSeedEngine.Application/
  EnglishSeedEngine.Infrastructure/
  EnglishSeedEngine.Api/
tests/
  EnglishSeedEngine.UnitTests/
  EnglishSeedEngine.IntegrationTests/
```

