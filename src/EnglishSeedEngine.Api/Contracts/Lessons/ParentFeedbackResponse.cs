namespace EnglishSeedEngine.Api.Contracts.Lessons;

public sealed record ParentFeedbackResponse(
    Guid Id,
    Guid LessonId,
    string Rating,
    int DifficultyDelta,
    DateTime SubmittedAtUtc);
