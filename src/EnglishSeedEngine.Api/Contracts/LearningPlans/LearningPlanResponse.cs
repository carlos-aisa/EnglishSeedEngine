namespace EnglishSeedEngine.Api.Contracts.LearningPlans;

public sealed record LearningPlanResponse(
    Guid Id,
    Guid StudentId,
    string StartLevel,
    string TargetLevel,
    string Status,
    DateTime CreatedAtUtc,
    IReadOnlyCollection<LearningPlanWeeklyGoalResponse> WeeklyGoals);
