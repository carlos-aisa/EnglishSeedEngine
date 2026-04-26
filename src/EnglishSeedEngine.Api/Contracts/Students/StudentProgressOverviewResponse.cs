namespace EnglishSeedEngine.Api.Contracts.Students;

public sealed record StudentProgressOverviewResponse(
    Guid StudentId,
    string CurrentLevel,
    int CompletedSessions,
    int? LastFiveAverageScore,
    string RecommendedFocus,
    bool HasSessions);
