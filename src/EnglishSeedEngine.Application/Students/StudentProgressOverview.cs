namespace EnglishSeedEngine.Application.Students;

public sealed record StudentProgressOverview(
    Guid StudentId,
    string CurrentLevel,
    int CompletedSessions,
    int? LastFiveAverageScore,
    string RecommendedFocus,
    bool HasSessions);
