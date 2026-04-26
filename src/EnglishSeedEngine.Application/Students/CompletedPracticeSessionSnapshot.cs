namespace EnglishSeedEngine.Application.Students;

public sealed record CompletedPracticeSessionSnapshot(
    int ScorePercentage,
    DateTime FinishedAtUtc,
    string WeakPointsJson);
