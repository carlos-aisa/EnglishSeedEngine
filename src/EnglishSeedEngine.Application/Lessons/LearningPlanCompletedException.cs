namespace EnglishSeedEngine.Application.Lessons;

public sealed class LearningPlanCompletedException : Exception
{
    public LearningPlanCompletedException(Guid learningPlanId)
        : base($"Learning plan '{learningPlanId}' is already completed.")
    {
    }
}
