using System.ComponentModel.DataAnnotations;

namespace EnglishSeedEngine.Api.Contracts.Students;

public sealed class CreateStudentRequest
{
    [Required]
    [MinLength(2)]
    public string FullName { get; init; } = string.Empty;

    [Range(5, 18)]
    public int Age { get; init; }

    [Required]
    [EmailAddress]
    public string TutorEmail { get; init; } = string.Empty;

    [Required]
    [RegularExpression("^(A1|A2|B1|B2|C1|C2)$")]
    public string TargetLevel { get; init; } = string.Empty;
}

