namespace EnglishSeedEngine.Application.LearningPlans;

public sealed class MissingInitialAssessmentException : Exception
{
    public MissingInitialAssessmentException(Guid studentId)
        : base($"Student '{studentId}' must complete the initial assessment before creating a learning plan.")
    {
    }
}
