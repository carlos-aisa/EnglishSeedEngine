using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.Lessons;

public sealed class SubmitParentFeedbackRequest
{
    [Required]
    [RegularExpression("^(too_easy|adequate|too_hard)$")]
    public string Rating { get; init; } = string.Empty;
}
