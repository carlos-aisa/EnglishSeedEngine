namespace EnglishSeedEngine.Domain.PracticeSessions;

public sealed record PracticeSessionResult(
    Guid PracticeSessionId,
    int TotalExercises,
    int AnsweredExercises,
    int CorrectAnswers,
    int ScorePercentage,
    IReadOnlyCollection<string> WeakPoints);
