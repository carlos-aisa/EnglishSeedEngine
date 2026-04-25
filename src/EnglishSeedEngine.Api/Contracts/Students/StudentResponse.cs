namespace EnglishSeedEngine.Api.Contracts.Students;

public sealed record StudentResponse(
    Guid Id,
    string FullName,
    int Age,
    string TutorEmail,
    string TargetLevel,
    DateTime CreatedAtUtc);

