# EnglishSeed Engine

Backend-first project for adaptive English learning focused on testing quality.

## Current Scope

- Modular monolith structure (`Domain`, `Application`, `Infrastructure`, `Api`).
- Student registration API.
- Initial assessment and CEFR approximation endpoints.
- Personalized 4-week learning plan endpoints.
- Next lesson generation and lesson retrieval endpoints.
- Health endpoint.
- Integration tests with real PostgreSQL in Docker (`Testcontainers` + `Respawn`).

## Development Workflow

- `main` is the integration branch.
- New work should happen in feature branches and be merged through pull requests.
- GitHub issues are the unit of planned work.
- CI validates pushes to `main` and pull requests targeting `main`.
- Testing strategy: [docs/testing/TESTING_STRATEGY.md](docs/testing/TESTING_STRATEGY.md)

See [CONTRIBUTING.md](CONTRIBUTING.md) for the branch naming convention and PR flow.

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
