namespace EnglishSeedEngine.Domain.Lessons;

public sealed class ParentFeedback
{
    private ParentFeedback()
    {
    }

    private ParentFeedback(
        Guid id,
        Guid lessonId,
        string rating,
        int difficultyDelta,
        DateTime submittedAtUtc)
    {
        Id = id;
        LessonId = lessonId;
        Rating = rating;
        DifficultyDelta = difficultyDelta;
        SubmittedAtUtc = submittedAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid LessonId { get; private set; }

    public string Rating { get; private set; } = string.Empty;

    public int DifficultyDelta { get; private set; }

    public DateTime SubmittedAtUtc { get; private set; }

    public static ParentFeedback Create(Guid lessonId, string rating, DateTime submittedAtUtc)
    {
        if (lessonId == Guid.Empty)
        {
            throw new ArgumentException("Lesson id is required.", nameof(lessonId));
        }

        if (string.IsNullOrWhiteSpace(rating))
        {
            throw new ArgumentException("Rating is required.", nameof(rating));
        }

        var normalizedRating = rating.Trim().ToLowerInvariant();
        var delta = normalizedRating switch
        {
            Ratings.TooEasy => 1,
            Ratings.Adequate => 0,
            Ratings.TooHard => -1,
            _ => throw new ArgumentException(
                $"Rating must be one of: {Ratings.TooEasy}, {Ratings.Adequate}, {Ratings.TooHard}.",
                nameof(rating))
        };

        var normalizedSubmittedAt = NormalizeUtc(submittedAtUtc);

        return new ParentFeedback(
            Guid.NewGuid(),
            lessonId,
            normalizedRating,
            delta,
            normalizedSubmittedAt);
    }

    private static DateTime NormalizeUtc(DateTime value)
    {
        var utcValue = value.Kind == DateTimeKind.Utc
            ? value
            : value.ToUniversalTime();

        var truncatedTicks = utcValue.Ticks - (utcValue.Ticks % TimeSpan.TicksPerMicrosecond);
        return new DateTime(truncatedTicks, DateTimeKind.Utc);
    }

    public static class Ratings
    {
        public const string TooEasy = "too_easy";
        public const string Adequate = "adequate";
        public const string TooHard = "too_hard";
    }
}
