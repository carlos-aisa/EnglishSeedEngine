using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.PracticeSessions;

public sealed class SubmitPracticeSessionAnswersRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<SubmitPracticeSessionAnswerRequest> Answers { get; init; } = [];
}
