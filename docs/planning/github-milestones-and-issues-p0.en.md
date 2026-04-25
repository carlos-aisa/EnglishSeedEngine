# EnglishSeed Engine - P0 GitHub Milestones + Issues (English)

This document gives you a P0 backlog ready to copy into GitHub, focused on testing and quality.

## Suggested Milestones

1. `P0-M1 Testing Foundation`  
   Target date: `2026-05-10`  
   Goal: architecture baseline + integration testing harness + first core flows.
2. `P0-M2 Core Learning Flows`  
   Target date: `2026-05-24`  
   Goal: learning plans, lessons, materials, and practice sessions with strong API quality.
3. `P0-M3 Delivery Hardening`  
   Target date: `2026-06-07`  
   Goal: parent feedback, progress overview, Docker, and CI.

## Recommended Labels

`type:feature`, `type:infra`, `type:testing`, `priority:p0`, `area:api`, `area:domain`, `area:data`, `area:quality`, `area:devops`

## Shared Definition of Done (copy into each issue)

```md
## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

## P0 Issues (copy and paste)

### 1) INF-001 - Bootstrap modular backend solution

- **Title:** `INF-001 Bootstrap solution and modular backend skeleton`
- **Labels:** `type:infra`, `priority:p0`, `area:api`, `area:domain`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Context
We need a clean, maintainable baseline to work testing-first with low friction.

## Scope
- Create .NET 8 solution:
  - src/EnglishSeedEngine.Api
  - src/EnglishSeedEngine.Application
  - src/EnglishSeedEngine.Domain
  - src/EnglishSeedEngine.Infrastructure
  - tests/EnglishSeedEngine.UnitTests
  - tests/EnglishSeedEngine.IntegrationTests
- Configure layer dependencies.
- Configure EF Core with initial DbContext.
- Add basic health endpoint (`/health`).

## Acceptance Criteria
- Solution builds (`dotnet build`).
- API starts and responds on `/health`.
- Architecture baseline documented in technical README.

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 2) INF-002 - Integration testing harness

- **Title:** `INF-002 Integration test harness with Testcontainers + Respawn`
- **Labels:** `type:testing`, `priority:p0`, `area:quality`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## Context
The top project priority is Testing and Quality.

## Scope
- Configure `WebApplicationFactory`.
- Configure `Testcontainers` with real PostgreSQL.
- Configure `Respawn` to reset DB state between tests.
- Create base class for API integration tests.
- Add naming and test-structure conventions.

## Acceptance Criteria
- `dotnet test` can run locally with a real DB container.
- Tests are isolated and repeatable.
- Base suite runs in under 60 seconds in local environment.

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 3) US-001 - Register student profile

- **Title:** `US-001 Register student profile`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:data`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## User Story
As a parent, I want to register a student so I can start a learning plan.

## Endpoint
- POST /students
- GET /students/{id}

## Acceptance Criteria
- Creates student with name, age, and target level.
- Validates invalid payload (400).
- Detects duplicate tutor email (409).

## Integration Tests
- CreateStudent_Returns201
- CreateStudent_InvalidPayload_Returns400
- CreateStudent_DuplicateTutorEmail_Returns409

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 4) US-002 - Initial assessment and CEFR approximation

- **Title:** `US-002 Initial assessment and CEFR approximation`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M1 Testing Foundation`

```md
## User Story
As the system, I want to evaluate the student's initial level to personalize learning.

## Endpoint
- POST /students/{id}/assessments/initial
- GET /students/{id}/level

## Acceptance Criteria
- Persists assessment result.
- Assigns approximate CEFR level (A1/A2/B1).
- Returns 404 for non-existing student.

## Integration Tests
- SubmitInitialAssessment_AssignsLevel
- SubmitInitialAssessment_StudentNotFound_Returns404
- SubmitInitialAssessment_InvalidAnswers_Returns400

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 5) US-003 - 4-week personalized learning plan

- **Title:** `US-003 Create 4-week personalized learning plan`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## User Story
As a parent, I want a personalized 4-week learning plan.

## Endpoint
- POST /students/{id}/learning-plans
- GET /learning-plans/{id}

## Acceptance Criteria
- Creates weekly goals based on current level and target.
- Blocks plan creation without initial assessment (422).
- Returns current plan status.

## Integration Tests
- CreatePlan_FromAssessment_Returns201
- CreatePlan_WithoutAssessment_Returns422
- GetPlan_NotFound_Returns404

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 6) US-004 - Generate next lesson from active plan

- **Title:** `US-004 Generate next lesson from active plan`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## User Story
As the system, I want to create the next lesson from the active plan.

## Endpoint
- POST /learning-plans/{id}/lessons:next
- GET /lessons/{id}

## Acceptance Criteria
- Creates lesson with target difficulty and weekly focus.
- Does not create lessons when plan is completed (409).
- Returns 404 when plan is missing.

## Integration Tests
- CreateNextLesson_Returns201
- CreateNextLesson_PlanCompleted_Returns409
- CreateNextLesson_PlanNotFound_Returns404

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 7) US-005 - Generate lesson materials and exercises

- **Title:** `US-005 Generate lesson materials and exercises`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:quality`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## User Story
As a student, I want lesson materials and exercises to practice.

## Endpoint
- POST /lessons/{id}/materials:generate
- GET /lessons/{id}/materials

## Acceptance Criteria
- Generates vocabulary, phrases, and exercises (`cloze`, `translation`, `dictation`).
- Versions and persists generated materials.
- Handles AI provider failures with controlled 503 response.

## Integration Tests
- GenerateMaterials_Returns201
- GenerateMaterials_LessonNotFound_Returns404
- GenerateMaterials_AiProviderFailure_Returns503

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 8) US-006 - Practice session scoring and weak points

- **Title:** `US-006 Practice session scoring and weak points`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M2 Core Learning Flows`

```md
## User Story
As a student, I want to submit answers and receive actionable results.

## Endpoint
- POST /practice-sessions
- POST /practice-sessions/{id}/answers
- POST /practice-sessions/{id}:finish
- GET /practice-sessions/{id}/result

## Acceptance Criteria
- Persists submitted answers.
- Computes score and common weak points.
- Blocks answer submission after session is finished (409).

## Integration Tests
- StartSession_Returns201
- SubmitAnswers_AfterFinish_Returns409
- FinishSession_ComputesScoreAndWeakPoints

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 9) US-007 - Parent feedback and lesson difficulty tuning

- **Title:** `US-007 Parent feedback and lesson difficulty tuning`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:domain`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## User Story
As a parent, I want to rate lesson difficulty so the next lessons adapt.

## Endpoint
- POST /lessons/{id}/parent-feedback

## Acceptance Criteria
- Accepts `too_easy`, `adequate`, `too_hard`.
- Persists feedback.
- Adjusts next lesson difficulty (+1 / 0 / -1).

## Integration Tests
- SubmitFeedback_AdjustsNextLessonDifficulty
- SubmitFeedback_InvalidValue_Returns400
- SubmitFeedback_LessonNotFound_Returns404

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 10) US-008 - Student progress overview endpoint

- **Title:** `US-008 Student progress overview endpoint`
- **Labels:** `type:feature`, `priority:p0`, `area:api`, `area:data`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## User Story
As a parent, I want a student progress summary to make better learning decisions.

## Endpoint
- GET /students/{id}/progress/overview

## Acceptance Criteria
- Returns current level, completed sessions, last-5 average, and recommended focus.
- Supports empty-state when no sessions exist.
- Returns 404 for non-existing student.

## Integration Tests
- GetOverview_ReturnsAggregatedMetrics
- GetOverview_StudentNotFound_Returns404
- GetOverview_NoSessions_ReturnsEmptyState

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 11) INF-003 - Dockerized local environment

- **Title:** `INF-003 Dockerized local environment (API + PostgreSQL)`
- **Labels:** `type:infra`, `priority:p0`, `area:devops`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Context
We need a reproducible local environment for onboarding and demos.

## Scope
- API Dockerfile.
- docker-compose with API + PostgreSQL.
- Environment variables and data volumes.

## Acceptance Criteria
- `docker compose up` runs a working local API.
- README includes clear startup instructions.

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 12) INF-004 - CI baseline

- **Title:** `INF-004 CI baseline: build + unit + integration tests`
- **Labels:** `type:infra`, `type:testing`, `priority:p0`, `area:devops`, `area:quality`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Context
Every change must be automatically validated to protect quality.

## Scope
- GitHub Actions pipeline:
  - restore/build
  - unit tests
  - integration tests with containers
- NuGet package cache.
- Required status checks on PRs.

## Acceptance Criteria
- Green pipeline on main branch.
- Pipeline fails if any test fails.
- Reasonable pipeline runtime (<10 minutes initial goal).

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

### 13) INF-005 - Technical portfolio documentation

- **Title:** `INF-005 Technical README and testing strategy`
- **Labels:** `type:infra`, `priority:p0`, `area:quality`
- **Milestone:** `P0-M3 Delivery Hardening`

```md
## Context
The project must communicate professional technical value on GitHub.

## Scope
- Main README with:
  - problem and value proposition
  - architecture
  - local run instructions
  - roadmap
- `docs/testing/TESTING_STRATEGY.md` including:
  - project-specific test pyramid
  - what belongs in unit, integration, and API contract tests
  - test doubles approach

## Acceptance Criteria
- Anyone can run the project and tests following the README.
- The testing-first strategy is explicit and understandable.

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
```

## Recommended Execution Order

1. INF-001
2. INF-002
3. US-001
4. US-002
5. US-003
6. US-004
7. US-005
8. US-006
9. US-007
10. US-008
11. INF-003
12. INF-004
13. INF-005

