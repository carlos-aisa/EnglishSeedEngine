param(
    [string]$Owner,
    [string]$Repo,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

function Assert-GitHubCli {
    $null = gh --version
}

function Resolve-RepositorySlug {
    param(
        [string]$OwnerParam,
        [string]$RepoParam
    )

    if (-not [string]::IsNullOrWhiteSpace($OwnerParam) -and -not [string]::IsNullOrWhiteSpace($RepoParam)) {
        return "$OwnerParam/$RepoParam"
    }

    $nameWithOwner = gh repo view --json nameWithOwner -q .nameWithOwner 2>$null
    if (-not [string]::IsNullOrWhiteSpace($nameWithOwner)) {
        return $nameWithOwner.Trim()
    }

    $remoteUrl = git remote get-url origin 2>$null
    if (-not [string]::IsNullOrWhiteSpace($remoteUrl)) {
        $match = [regex]::Match($remoteUrl.Trim(), "github\.com[:/](?<owner>[^/]+)/(?<repo>[^/.]+)(\.git)?$")
        if ($match.Success) {
            return "$($match.Groups['owner'].Value)/$($match.Groups['repo'].Value)"
        }
    }

    throw "Could not resolve repository. Provide -Owner and -Repo explicitly."
}

function Assert-RepositoryExists {
    param(
        [string]$RepositorySlug
    )

    gh api "repos/$RepositorySlug" --silent 2>$null
}

function Get-MilestoneTitleSet {
    param(
        [string]$RepositorySlug
    )

    $milestonesJson = gh api "repos/$RepositorySlug/milestones?state=all&per_page=100"
    $milestones = $milestonesJson | ConvertFrom-Json
    $titleSet = New-Object System.Collections.Generic.HashSet[string]([System.StringComparer]::OrdinalIgnoreCase)

    foreach ($milestone in $milestones) {
        [void]$titleSet.Add([string]$milestone.title)
    }

    return $titleSet
}

function Get-ExistingIssueMap {
    param(
        [string]$RepositorySlug
    )

    $issuesJson = gh issue list --repo $RepositorySlug --state all --limit 200 --json number,title
    $issues = $issuesJson | ConvertFrom-Json
    $map = @{}

    foreach ($issue in $issues) {
        $map[[string]$issue.title] = [int]$issue.number
    }

    return $map
}

function Invoke-IssueCreate {
    param(
        [string]$RepositorySlug,
        [pscustomobject]$Issue,
        [switch]$DryRunMode
    )

    $args = @(
        "issue", "create",
        "--repo", $RepositorySlug,
        "--title", $Issue.Title,
        "--body", $Issue.Body,
        "--milestone", $Issue.Milestone
    )

    foreach ($label in $Issue.Labels) {
        $args += @("--label", $label)
    }

    if ($DryRunMode) {
        Write-Host "[DRY-RUN] Would create issue: $($Issue.Title)"
        return
    }

    $url = gh @args
    Write-Host "Created issue: $($Issue.Title) -> $url"
}

$definitionOfDone = @'

## Definition of Done
- Endpoint or module documented in OpenAPI or technical README.
- EF Core migrations are up to date and applied in test environment.
- At least 1 happy-path integration test.
- At least 2 negative integration tests where applicable.
- Critical domain rules covered by unit tests.
- Structured logs for use-case start/end and errors.
- CI is green.
'@

$issues = @(
    [pscustomobject]@{
        Title = "INF-001 Bootstrap solution and modular backend skeleton"
        Milestone = "P0-M1 Testing Foundation"
        Labels = @("type:infra", "priority:p0", "area:api", "area:domain", "area:data")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "INF-002 Integration test harness with Testcontainers + Respawn"
        Milestone = "P0-M1 Testing Foundation"
        Labels = @("type:testing", "priority:p0", "area:quality", "area:data")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-001 Register student profile"
        Milestone = "P0-M1 Testing Foundation"
        Labels = @("type:feature", "priority:p0", "area:api", "area:data")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-002 Initial assessment and CEFR approximation"
        Milestone = "P0-M1 Testing Foundation"
        Labels = @("type:feature", "priority:p0", "area:api", "area:domain")
        Body = @'
## User Story
As the system, I want to evaluate the student''s initial level to personalize learning.

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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-003 Create 4-week personalized learning plan"
        Milestone = "P0-M2 Core Learning Flows"
        Labels = @("type:feature", "priority:p0", "area:api", "area:domain")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-004 Generate next lesson from active plan"
        Milestone = "P0-M2 Core Learning Flows"
        Labels = @("type:feature", "priority:p0", "area:api", "area:domain")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-005 Generate lesson materials and exercises"
        Milestone = "P0-M2 Core Learning Flows"
        Labels = @("type:feature", "priority:p0", "area:api", "area:quality")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-006 Practice session scoring and weak points"
        Milestone = "P0-M2 Core Learning Flows"
        Labels = @("type:feature", "priority:p0", "area:api", "area:domain")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-007 Parent feedback and lesson difficulty tuning"
        Milestone = "P0-M3 Delivery Hardening"
        Labels = @("type:feature", "priority:p0", "area:api", "area:domain")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "US-008 Student progress overview endpoint"
        Milestone = "P0-M3 Delivery Hardening"
        Labels = @("type:feature", "priority:p0", "area:api", "area:data")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "INF-003 Dockerized local environment (API + PostgreSQL)"
        Milestone = "P0-M3 Delivery Hardening"
        Labels = @("type:infra", "priority:p0", "area:devops")
        Body = @'
## Context
We need a reproducible local environment for onboarding and demos.

## Scope
- API Dockerfile.
- docker-compose with API + PostgreSQL.
- Environment variables and data volumes.

## Acceptance Criteria
- `docker compose up` runs a working local API.
- README includes clear startup instructions.
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "INF-004 CI baseline: build + unit + integration tests"
        Milestone = "P0-M3 Delivery Hardening"
        Labels = @("type:infra", "type:testing", "priority:p0", "area:devops", "area:quality")
        Body = @'
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
'@ + $definitionOfDone
    }
    [pscustomobject]@{
        Title = "INF-005 Technical README and testing strategy"
        Milestone = "P0-M3 Delivery Hardening"
        Labels = @("type:infra", "priority:p0", "area:quality")
        Body = @'
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
'@ + $definitionOfDone
    }
)

Assert-GitHubCli
$repositorySlug = Resolve-RepositorySlug -OwnerParam $Owner -RepoParam $Repo
Assert-RepositoryExists -RepositorySlug $repositorySlug

$milestoneSet = Get-MilestoneTitleSet -RepositorySlug $repositorySlug
$existingIssueMap = Get-ExistingIssueMap -RepositorySlug $repositorySlug

Write-Host "Target repository: $repositorySlug"
Write-Host "Planned issues: $($issues.Count)"

foreach ($issue in $issues) {
    if (-not $milestoneSet.Contains($issue.Milestone)) {
        throw "Milestone '$($issue.Milestone)' does not exist. Run gh-bootstrap-template.ps1 first."
    }

    if ($existingIssueMap.ContainsKey($issue.Title)) {
        $issueNumber = $existingIssueMap[$issue.Title]
        Write-Host "Skipped existing issue: $($issue.Title) (#$issueNumber)"
        continue
    }

    Invoke-IssueCreate -RepositorySlug $repositorySlug -Issue $issue -DryRunMode:$DryRun
}

Write-Host "Done."

