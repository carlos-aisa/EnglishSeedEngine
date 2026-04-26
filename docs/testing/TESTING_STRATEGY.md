# Testing Strategy - EnglishSeed Engine

This document defines the testing approach for the repository. The goal is to keep the test suite useful, fast, and easy to understand as the project grows.

## Goals

- Protect the business rules first.
- Verify the HTTP/API contract where it adds real value.
- Use real infrastructure for the flows that matter.
- Keep the suite fast enough to run often.
- Avoid overtesting combinations that do not add signal.

## Test Layers

### 1. Unit tests

Use unit tests for pure domain rules and small application decisions that can be verified without HTTP, database, or external services.

Typical examples:

- `Student.Create()` validation rules.
- Scoring rules.
- Difficulty adjustment logic.
- Small mapping or normalization logic.

Characteristics:

- Fast.
- No network.
- No database.
- No container.
- No `WebApplicationFactory`.

Preferred tools:

- `xUnit`
- `FluentAssertions`
- `Theory` for multiple values
- `Fact` for a single scenario

### 2. Integration tests

Use integration tests when the behavior depends on multiple real pieces working together.

In this repository, the default integration test is an **API integration test**:

- real ASP.NET Core API host
- real application layer
- real EF Core persistence
- real PostgreSQL in Docker
- shared test fixture with `WebApplicationFactory`

Typical examples:

- `POST /students` creates and persists a student.
- `GET /students/{id}` returns the stored student.
- Duplicate tutor email returns `409 Conflict`.
- Invalid payload returns `400 BadRequest`.

Characteristics:

- HTTP requests are real.
- Database is real.
- Repository and EF Core configuration are exercised.
- State is reset between tests.

Preferred tools:

- `Microsoft.AspNetCore.Mvc.Testing`
- `Testcontainers.PostgreSql`
- `Respawn`
- `xUnit collection fixtures`

### 3. API tests

For this repository, API tests are not a separate top-level category.

Instead, they are a subset of integration tests that focus on the HTTP contract:

- status codes
- response bodies
- validation errors
- routing
- serialization

If a test hits the real API host and real dependencies, it is an integration test and also an API test in the practical sense.

## What Goes Where

### Put in unit tests

- Domain invariants.
- Small business rules.
- Edge cases that do not need HTTP or database.
- Deterministic behavior.

### Put in integration tests

- End-to-end HTTP flows.
- Persistence behavior.
- Validation at the API boundary.
- Repository + EF Core behavior.
- Cross-component behavior.

### Put outside tests for now

- Full UI flows.
- Microservice orchestration.
- Message broker pipelines.
- Cloud deployment specifics.
- Infrastructure details that are already covered indirectly by higher-level tests.

## Test Doubles

Use test doubles when a dependency is external, slow, unreliable, or expensive to use directly in a test.

Good candidates:

- AI generation client.
- Clock/time provider.
- External auth provider.
- Email/SMS notification provider.

Do not mock the database or the repository in integration tests unless you are intentionally testing a narrower seam.

## Data And Input Patterns

### Use `Fact` when

- there is one clear scenario.
- the case is simple.
- the test is already expressive enough on its own.

### Use `Theory` when

- you want to cover several values for the same rule.
- you are testing borders or validation rules.
- the test body should stay identical across cases.

### Use builders when

- the input object has many properties.
- you want to vary only one field per case.
- copying full objects starts to hurt readability.

Example guideline:

- small scalar values -> `InlineData`
- medium-sized object variations -> `MemberData`
- large test objects -> builder/factory

## Naming Conventions

- Unit test: `Method_Scenario_ExpectedResult`
- Integration test: `Action_Condition_ExpectedResult`

Examples:

- `Create_WithInvalidAge_ThrowsArgumentOutOfRangeException`
- `CreateStudent_WithValidPayload_Returns201Created`
- `CreateStudent_WithInvalidPayload_Returns400BadRequest`

## Practical Rule Of Thumb

If you are asking "should this be a unit test or an integration test?", use this order:

1. Can I verify the rule without HTTP or database?
2. If yes, make it a unit test.
3. If the behavior depends on API routing, serialization, persistence, or a real host, make it an integration test.
4. If the test is only checking the HTTP contract of a real endpoint, treat it as an API integration test.

## Current Repository Baseline

The current baseline follows this model:

- `StudentTests.cs` contains domain unit tests.
- `StudentsEndpointsTests.cs` contains API integration tests.
- `ApiIntegrationTestFixture` manages shared infrastructure for the integration suite.
- `WebApplicationFactory + Testcontainers + Respawn` are the default integration test harness.

## Related Docs

- [Integration testing template](integration-test-template-dotnet.md)
- [Contributing guide](../../CONTRIBUTING.md)
