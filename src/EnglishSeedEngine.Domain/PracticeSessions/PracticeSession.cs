using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Domain.PracticeSessions;

public sealed class PracticeSession
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

    private PracticeSession()
    {
    }

    private PracticeSession(
        Guid id,
        Guid lessonId,
        string status,
        string exercisesJson,
        int? scorePercentage,
        string weakPointsJson,
        DateTime createdAtUtc,
        DateTime? finishedAtUtc)
    {
        Id = id;
        LessonId = lessonId;
        Status = status;
        ExercisesJson = exercisesJson;
        ScorePercentage = scorePercentage;
        WeakPointsJson = weakPointsJson;
        CreatedAtUtc = createdAtUtc;
        FinishedAtUtc = finishedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid LessonId { get; private set; }

    public string Status { get; private set; } = string.Empty;

    public string ExercisesJson { get; private set; } = "[]";

    public int? ScorePercentage { get; private set; }

    public string WeakPointsJson { get; private set; } = "[]";

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? FinishedAtUtc { get; private set; }

    [NotMapped]
    public IReadOnlyCollection<PracticeSessionExercise> Exercises => DeserializeExercises(ExercisesJson);

    [NotMapped]
    public IReadOnlyCollection<string> WeakPoints => DeserializeWeakPoints(WeakPointsJson);

    [NotMapped]
    public bool IsFinished => Status.Equals(Statuses.Finished, StringComparison.OrdinalIgnoreCase);

    public static PracticeSession Start(
        Guid lessonId,
        IReadOnlyCollection<LessonMaterialExercise> exercises,
        DateTime createdAtUtc)
    {
        ArgumentNullException.ThrowIfNull(exercises);

        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson id is required.", nameof(lessonId));
        }

        if (exercises.Count == 0)
        {
            throw new ArgumentException("Exercises are required to start a session.", nameof(exercises));
        }

        var normalizedExercises = new List<PracticeSessionExercise>(exercises.Count);
        var index = 1;

        foreach (var exercise in exercises)
        {
            if (exercise is null)
            {
                throw new ArgumentException("Exercises cannot contain null elements.", nameof(exercises));
            }

            var type = exercise.Type.Trim().ToLowerInvariant();
            var prompt = exercise.Prompt.Trim();
            var expectedAnswer = exercise.ExpectedAnswer.Trim();

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Exercise type is required.", nameof(exercises));
            }

            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Exercise prompt is required.", nameof(exercises));
            }

            if (string.IsNullOrWhiteSpace(expectedAnswer))
            {
                throw new ArgumentException("Exercise expected answer is required.", nameof(exercises));
            }

            normalizedExercises.Add(new PracticeSessionExercise(index, type, prompt, expectedAnswer, SubmittedAnswer: null));
            index++;
        }

        var normalizedCreatedAt = NormalizeUtc(createdAtUtc);

        return new PracticeSession(
            Guid.NewGuid(),
            lessonId,
            Statuses.InProgress,
            JsonSerializer.Serialize(normalizedExercises, JsonSerializerOptions),
            scorePercentage: null,
            weakPointsJson: "[]",
            normalizedCreatedAt,
            finishedAtUtc: null);
    }

    public void SubmitAnswers(IReadOnlyCollection<PracticeSessionAnswer> answers)
    {
        ArgumentNullException.ThrowIfNull(answers);

        EnsureInProgress();

        var exercises = DeserializeExercises(ExercisesJson).ToList();
        foreach (var answer in answers)
        {
            if (answer is null)
            {
                throw new ArgumentException("Answers cannot contain null elements.", nameof(answers));
            }

            var submittedAnswer = answer.Answer.Trim();
            if (string.IsNullOrWhiteSpace(submittedAnswer))
            {
                throw new ArgumentException("Submitted answer cannot be empty.", nameof(answers));
            }

            var exerciseIndex = answer.ExerciseIndex;
            var exercise = exercises.FirstOrDefault(x => x.ExerciseIndex == exerciseIndex);
            if (exercise is null)
            {
                throw new ArgumentOutOfRangeException(nameof(answers), $"Exercise index {exerciseIndex} does not exist.");
            }

            var updated = exercise with { SubmittedAnswer = submittedAnswer };
            var currentPosition = exercises.FindIndex(x => x.ExerciseIndex == exerciseIndex);
            exercises[currentPosition] = updated;
        }

        ExercisesJson = JsonSerializer.Serialize(exercises, JsonSerializerOptions);
    }

    public PracticeSessionResult Finish(DateTime finishedAtUtc)
    {
        EnsureInProgress();

        var exercises = DeserializeExercises(ExercisesJson);
        var correctAnswers = exercises.Count(IsCorrect);
        var answeredExercises = exercises.Count(x => !string.IsNullOrWhiteSpace(x.SubmittedAnswer));
        var totalExercises = exercises.Count;

        var scorePercentage = totalExercises == 0
            ? 0
            : (int)Math.Round(correctAnswers * 100d / totalExercises, MidpointRounding.AwayFromZero);

        var weakPoints = exercises
            .Where(x => !IsCorrect(x))
            .GroupBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.Key)
            .ToArray();

        Status = Statuses.Finished;
        ScorePercentage = scorePercentage;
        WeakPointsJson = JsonSerializer.Serialize(weakPoints, JsonSerializerOptions);
        FinishedAtUtc = NormalizeUtc(finishedAtUtc);

        return new PracticeSessionResult(
            Id,
            totalExercises,
            answeredExercises,
            correctAnswers,
            scorePercentage,
            weakPoints);
    }

    public PracticeSessionResult GetResult()
    {
        if (!IsFinished || ScorePercentage is null)
        {
            throw new InvalidOperationException("Practice session is not finished yet.");
        }

        var exercises = DeserializeExercises(ExercisesJson);
        return new PracticeSessionResult(
            Id,
            exercises.Count,
            exercises.Count(x => !string.IsNullOrWhiteSpace(x.SubmittedAnswer)),
            exercises.Count(IsCorrect),
            ScorePercentage.Value,
            DeserializeWeakPoints(WeakPointsJson));
    }

    private static bool IsCorrect(PracticeSessionExercise exercise)
    {
        if (string.IsNullOrWhiteSpace(exercise.SubmittedAnswer))
        {
            return false;
        }

        return string.Equals(
            exercise.SubmittedAnswer.Trim(),
            exercise.ExpectedAnswer.Trim(),
            StringComparison.OrdinalIgnoreCase);
    }

    private void EnsureInProgress()
    {
        if (!Status.Equals(Statuses.InProgress, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Practice session is already finished.");
        }
    }

    private static IReadOnlyCollection<PracticeSessionExercise> DeserializeExercises(string json)
    {
        return JsonSerializer.Deserialize<PracticeSessionExercise[]>(json, JsonSerializerOptions) ?? [];
    }

    private static IReadOnlyCollection<string> DeserializeWeakPoints(string json)
    {
        return JsonSerializer.Deserialize<string[]>(json, JsonSerializerOptions) ?? [];
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        var utcValue = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();

        var truncatedTicks = utcValue.Ticks - (utcValue.Ticks % TimeSpan.TicksPerMicrosecond);
        return new DateTime(truncatedTicks, DateTimeKind.Utc);
    }

    public static class Statuses
    {
        public const string InProgress = "InProgress";
        public const string Finished = "Finished";
    }
}
