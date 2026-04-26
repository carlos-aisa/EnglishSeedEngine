namespace EnglishSeedEngine.Domain.Lessons;

public sealed class Lesson
{
    private Lesson()
    {
    }

    private Lesson(
        Guid id,
        Guid learningPlanId,
        int weekNumber,
        string weeklyFocus,
        string targetDifficulty,
        DateTime createdAtUtc)
    {
        Id = id;
        LearningPlanId = learningPlanId;
        WeekNumber = weekNumber;
        WeeklyFocus = weeklyFocus;
        TargetDifficulty = targetDifficulty;
        Status = "Draft";
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public Guid LearningPlanId { get; private set; }

    public int WeekNumber { get; private set; }

    public string WeeklyFocus { get; private set; } = string.Empty;

    public string TargetDifficulty { get; private set; } = string.Empty;

    public string Status { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public static Lesson Create(
        Guid learningPlanId,
        int weekNumber,
        string weeklyFocus,
        string targetDifficulty,
        DateTime createdAtUtc)
    {
        if (learningPlanId == Guid.Empty)
        {
            throw new ArgumentException("Learning plan id is required.", nameof(learningPlanId));
        }

        if (weekNumber is < 1 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(weekNumber), "Week number must be between 1 and 4.");
        }

        if (string.IsNullOrWhiteSpace(weeklyFocus))
        {
            throw new ArgumentException("Weekly focus is required.", nameof(weeklyFocus));
        }

        if (string.IsNullOrWhiteSpace(targetDifficulty))
        {
            throw new ArgumentException("Target difficulty is required.", nameof(targetDifficulty));
        }

        var normalizedCreatedAt = NormalizeUtc(createdAtUtc);

        return new Lesson(
            Guid.NewGuid(),
            learningPlanId,
            weekNumber,
            weeklyFocus.Trim(),
            targetDifficulty.Trim(),
            normalizedCreatedAt);
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
