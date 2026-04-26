namespace EnglishSeedEngine.Domain.Lessons;

public sealed record LessonMaterialExercise(
    string Type,
    string Prompt,
    string ExpectedAnswer);
