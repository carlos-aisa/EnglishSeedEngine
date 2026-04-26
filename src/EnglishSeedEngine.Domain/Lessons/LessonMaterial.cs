using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EnglishSeedEngine.Domain.Lessons;

public sealed class LessonMaterial
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly string[] RequiredExerciseTypes = ["cloze", "translation", "dictation"];

    private LessonMaterial()
    {
    }

    private LessonMaterial(
        Guid id,
        Guid lessonId,
        int version,
        string vocabularyJson,
        string phrasesJson,
        string exercisesJson,
        DateTime generatedAtUtc)
    {
        Id = id;
        LessonId = lessonId;
        Version = version;
        VocabularyJson = vocabularyJson;
        PhrasesJson = phrasesJson;
        ExercisesJson = exercisesJson;
        GeneratedAtUtc = generatedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid LessonId { get; private set; }

    public int Version { get; private set; }

    public string VocabularyJson { get; private set; } = "[]";

    public string PhrasesJson { get; private set; } = "[]";

    public string ExercisesJson { get; private set; } = "[]";

    public DateTime GeneratedAtUtc { get; private set; }

    [NotMapped]
    public IReadOnlyCollection<string> Vocabulary => DeserializeStringCollection(VocabularyJson);

    [NotMapped]
    public IReadOnlyCollection<string> Phrases => DeserializeStringCollection(PhrasesJson);

    [NotMapped]
    public IReadOnlyCollection<LessonMaterialExercise> Exercises => DeserializeExerciseCollection(ExercisesJson);

    public static LessonMaterial Create(
        Guid lessonId,
        int version,
        IReadOnlyCollection<string> vocabulary,
        IReadOnlyCollection<string> phrases,
        IReadOnlyCollection<LessonMaterialExercise> exercises,
        DateTime generatedAtUtc)
    {
        ArgumentNullException.ThrowIfNull(vocabulary);
        ArgumentNullException.ThrowIfNull(phrases);
        ArgumentNullException.ThrowIfNull(exercises);

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson id is required.", nameof(lessonId));
        }

        if (version < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(version), "Version must be greater than zero.");
        }

        var normalizedVocabulary = NormalizeValues(vocabulary, nameof(vocabulary), "Vocabulary");
        var normalizedPhrases = NormalizeValues(phrases, nameof(phrases), "Phrases");
        var normalizedExercises = NormalizeExercises(exercises);
        ValidateRequiredExerciseTypes(normalizedExercises);

        var normalizedGeneratedAt = NormalizeUtc(generatedAtUtc);

        return new LessonMaterial(
            Guid.NewGuid(),
            lessonId,
            version,
            JsonSerializer.Serialize(normalizedVocabulary, JsonSerializerOptions),
            JsonSerializer.Serialize(normalizedPhrases, JsonSerializerOptions),
            JsonSerializer.Serialize(normalizedExercises, JsonSerializerOptions),
            normalizedGeneratedAt);
    }

    private static string[] NormalizeValues(
        IReadOnlyCollection<string> values,
        string paramName,
        string label)
    {
        if (values.Count == 0)
        {
            throw new ArgumentException($"{label} cannot be empty.", paramName);
        }

        var normalized = values
            .Select(x => x?.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x!)
            .ToArray();

        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{label} cannot be empty.", paramName);
        }

        return normalized;
    }

    private static LessonMaterialExercise[] NormalizeExercises(IReadOnlyCollection<LessonMaterialExercise> exercises)
    {
        if (exercises.Count == 0)
        {
            throw new ArgumentException("Exercises cannot be empty.", nameof(exercises));
        }

        var normalized = new List<LessonMaterialExercise>(exercises.Count);

        foreach (var exercise in exercises)
        {
            if (exercise is null)
            {
                throw new ArgumentException("Exercises cannot contain null elements.", nameof(exercises));
            }

            var normalizedType = exercise.Type.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedType))
            {
                throw new ArgumentException("Exercise type is required.", nameof(exercises));
            }

            var prompt = exercise.Prompt.Trim();
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Exercise prompt is required.", nameof(exercises));
            }

            var expectedAnswer = exercise.ExpectedAnswer.Trim();
            if (string.IsNullOrWhiteSpace(expectedAnswer))
            {
                throw new ArgumentException("Exercise expected answer is required.", nameof(exercises));
            }

            normalized.Add(new LessonMaterialExercise(normalizedType, prompt, expectedAnswer));
        }

        return normalized.ToArray();
    }

    private static void ValidateRequiredExerciseTypes(IReadOnlyCollection<LessonMaterialExercise> exercises)
    {
        var normalizedTypes = exercises
            .Select(x => x.Type)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTypes = RequiredExerciseTypes
            .Where(x => !normalizedTypes.Contains(x))
            .ToArray();

        if (missingTypes.Length > 0)
        {
            throw new ArgumentException(
                $"Exercises must include required types: {string.Join(", ", RequiredExerciseTypes)}.",
                nameof(exercises));
        }
    }

    private static IReadOnlyCollection<string> DeserializeStringCollection(string json)
    {
        return JsonSerializer.Deserialize<string[]>(json, JsonSerializerOptions) ?? [];
    }

    private static IReadOnlyCollection<LessonMaterialExercise> DeserializeExerciseCollection(string json)
    {
        return JsonSerializer.Deserialize<LessonMaterialExercise[]>(json, JsonSerializerOptions) ?? [];
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        var utcValue = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();

        var truncatedTicks = utcValue.Ticks - (utcValue.Ticks % TimeSpan.TicksPerMicrosecond);
        return new DateTime(truncatedTicks, DateTimeKind.Utc);
    }
}
