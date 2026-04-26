namespace EnglishSeedEngine.Api.Contracts.Students;

public sealed record StudentLevelResponse(
    Guid StudentId,
    int CorrectAnswers,
    int TotalQuestions,
    int ScorePercentage,
    string CefrLevel,
    DateTime AssessedAtUtc);
