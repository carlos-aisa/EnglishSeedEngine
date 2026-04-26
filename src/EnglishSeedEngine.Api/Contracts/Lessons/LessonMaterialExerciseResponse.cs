namespace EnglishSeedEngine.Api.Contracts.Lessons;

public sealed record LessonMaterialExerciseResponse(
    string Type,
    string Prompt,
    string ExpectedAnswer);
