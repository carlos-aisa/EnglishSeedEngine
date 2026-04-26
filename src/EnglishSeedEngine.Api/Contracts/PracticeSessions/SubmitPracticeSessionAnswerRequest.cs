using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.PracticeSessions;

public sealed class SubmitPracticeSessionAnswerRequest
{
    [Range(1, int.MaxValue)]
    public int ExerciseIndex { get; init; }

    [Required]
    [MinLength(1)]
    public string Answer { get; init; } = string.Empty;
}
