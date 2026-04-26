namespace EnglishSeedEngine.Api.Contracts.PracticeSessions;

public sealed record PracticeSessionResponse(
    Guid Id,
    Guid LessonId,
    string Status,
    int TotalExercises,
    DateTime CreatedAtUtc,
    DateTime? FinishedAtUtc);
