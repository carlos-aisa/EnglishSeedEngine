# Contributing

This repository is intended to follow a simple professional workflow:

- `main` is the integration branch.
- Work happens in short-lived feature branches.
- Changes should be merged through pull requests.
- Issues represent the unit of planned work.
- Milestones group issues by phase or delivery target.

## Branching Strategy

Do not commit directly to `main` unless you are performing a one-off bootstrap or emergency fix.

Recommended branch naming:

- `feature/US-001-register-student-profile`
- `infra/INF-002-integration-harness`
- `docs/INF-005-testing-strategy`
- `bugfix/BUG-001-short-description`

## Pull Request Flow

1. Pick an issue from GitHub.
2. Create a branch from `main`.
3. Keep the scope aligned to a single issue whenever possible.
4. Open a pull request back to `main`.
5. Link the issue in the PR description using `Closes #123` or `Fixes #123`.
6. Wait for CI to pass before merging.

## Issue, PR, And Release Relationship

- An issue tracks a unit of work.
- A branch implements that work.
- A pull request reviews and integrates that work.
- A milestone groups several issues into a delivery phase.
- A release can summarize one or more completed milestones.

In practice, the main traceability chain is:

`Issue -> Branch -> Pull Request -> Merge -> Milestone/Release`

## PR Title Suggestions

Recommended PR title format:

- `[US-001] Register student profile`
- `[INF-002] Add integration test harness`

## Local Commands

Create a branch:

```bash
git checkout -b feature/US-001-register-student-profile
```

Run tests before opening a PR:

```bash
dotnet test
```

