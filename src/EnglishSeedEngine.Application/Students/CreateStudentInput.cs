namespace EnglishSeedEngine.Application.Students;

public sealed record CreateStudentInput(
    string FullName,
    int Age,
    string TutorEmail,
    string TargetLevel);

