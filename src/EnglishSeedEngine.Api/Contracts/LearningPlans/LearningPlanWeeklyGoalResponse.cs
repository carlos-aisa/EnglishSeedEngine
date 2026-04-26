namespace EnglishSeedEngine.Api.Contracts.LearningPlans;

public sealed record LearningPlanWeeklyGoalResponse(
    int WeekNumber,
    string Goal);
