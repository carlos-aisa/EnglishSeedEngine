using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.Students;

public sealed class SubmitInitialAssessmentRequest : IValidatableObject
{
    [Range(0, int.MaxValue)]
    public int CorrectAnswers { get; init; }

    [Range(1, int.MaxValue)]
    public int TotalQuestions { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CorrectAnswers > TotalQuestions)
        {
            yield return new ValidationResult(
                "Correct answers cannot exceed total questions.",
                [nameof(CorrectAnswers), nameof(TotalQuestions)]);
        }
    }
}
