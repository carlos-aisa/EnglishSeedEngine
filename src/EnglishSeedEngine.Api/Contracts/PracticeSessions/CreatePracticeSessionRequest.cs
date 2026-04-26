using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.PracticeSessions;

public sealed class CreatePracticeSessionRequest
{
    [Required]
    public Guid LessonId { get; init; }
}
