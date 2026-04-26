namespace EnglishSeedEngine.Api.Contracts.PracticeSessions;

public sealed record PracticeSessionResultResponse(
    Guid PracticeSessionId,
    string Status,
    int TotalExercises,
    int AnsweredExercises,
    int CorrectAnswers,
    int ScorePercentage,
    IReadOnlyCollection<string> WeakPoints,
    DateTime? FinishedAtUtc);
