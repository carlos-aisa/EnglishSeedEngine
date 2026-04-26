namespace EnglishSeedEngine.Api.Contracts.Lessons;

public sealed record LessonResponse(
    Guid Id,
    Guid LearningPlanId,
    int WeekNumber,
    string WeeklyFocus,
    string TargetDifficulty,
    string Status,
    DateTime CreatedAtUtc);
