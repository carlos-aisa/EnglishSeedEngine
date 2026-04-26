namespace EnglishSeedEngine.Application.Students;

public sealed record SubmitInitialAssessmentInput(
    int CorrectAnswers,
    int TotalQuestions);
