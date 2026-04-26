namespace EnglishSeedEngine.Domain.LearningPlans;

public sealed record NextLessonDraft(
    int WeekNumber,
    string WeeklyFocus,
    string TargetDifficulty);
