namespace EnglishSeedEngine.Application.LessonMaterials;

public sealed record GeneratedLessonMaterials(
    IReadOnlyCollection<string> Vocabulary,
    IReadOnlyCollection<string> Phrases,
    IReadOnlyCollection<GeneratedLessonExercise> Exercises);

public sealed record GeneratedLessonExercise(
    string Type,
    string Prompt,
    string ExpectedAnswer);
