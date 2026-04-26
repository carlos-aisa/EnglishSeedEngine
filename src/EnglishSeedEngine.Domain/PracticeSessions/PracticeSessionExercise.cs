namespace EnglishSeedEngine.Domain.PracticeSessions;

public sealed record PracticeSessionExercise(
    int ExerciseIndex,
    string Type,
    string Prompt,
    string ExpectedAnswer,
    string? SubmittedAnswer);
