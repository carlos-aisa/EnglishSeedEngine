namespace EnglishSeedEngine.Api.Contracts.Lessons;

public sealed record LessonMaterialResponse(
    Guid Id,
    Guid LessonId,
    int Version,
    DateTime GeneratedAtUtc,
    IReadOnlyCollection<string> Vocabulary,
    IReadOnlyCollection<string> Phrases,
    IReadOnlyCollection<LessonMaterialExerciseResponse> Exercises);
