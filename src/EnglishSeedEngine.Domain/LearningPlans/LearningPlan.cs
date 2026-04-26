namespace EnglishSeedEngine.Domain.LearningPlans;

public sealed class LearningPlan
{
    private LearningPlan()
    {
    }

    private LearningPlan(
        Guid id,
        Guid studentId,
        string startLevel,
        string targetLevel,
        DateTime createdAtUtc)
    {
        Id = id;
        StudentId = studentId;
        StartLevel = startLevel;
        TargetLevel = targetLevel;
        Status = "Active";
        CreatedAtUtc = createdAtUtc;
    }

    private const string ActiveStatus = "Active";
    private const string CompletedStatus = "Completed";

    public Guid Id { get; private set; }

    public Guid StudentId { get; private set; }

    public string StartLevel { get; private set; } = string.Empty;

    public string TargetLevel { get; private set; } = string.Empty;

    public string Status { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public List<LearningPlanWeeklyGoal> WeeklyGoals { get; private set; } = new();

    public static LearningPlan Create(
        Guid studentId,
        string startLevel,
        string targetLevel,
        DateTime createdAtUtc)
    {
        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id is required.", nameof(studentId));
        }

        if (string.IsNullOrWhiteSpace(startLevel))
        {
            throw new ArgumentException("Start level is required.", nameof(startLevel));
        }

        if (string.IsNullOrWhiteSpace(targetLevel))
        {
            throw new ArgumentException("Target level is required.", nameof(targetLevel));
        }

        var normalizedStartLevel = startLevel.Trim().ToUpperInvariant();
        var normalizedTargetLevel = targetLevel.Trim().ToUpperInvariant();
        var normalizedCreatedAt = NormalizeUtc(createdAtUtc);
        var planId = Guid.NewGuid();

        var plan = new LearningPlan(
            planId,
            studentId,
            normalizedStartLevel,
            normalizedTargetLevel,
            normalizedCreatedAt);

        plan.WeeklyGoals = BuildWeeklyGoals(planId, normalizedStartLevel, normalizedTargetLevel);

        return plan;
    }

    public NextLessonDraft GetNextLessonDraft(int existingLessonsCount)
    {
        if (existingLessonsCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(existingLessonsCount), "Existing lessons count cannot be negative.");
        }

        if (Status == CompletedStatus || existingLessonsCount >= WeeklyGoals.Count)
        {
            throw new InvalidOperationException("Learning plan is completed.");
        }

        var nextWeekNumber = existingLessonsCount + 1;
        var nextWeeklyGoal = WeeklyGoals.Single(x => x.WeekNumber == nextWeekNumber);
        var targetDifficulty = nextWeekNumber switch
        {
            1 => "Easy",
            2 => "Medium",
            3 => "Medium",
            4 => "Hard",
            _ => "Hard"
        };

        return new NextLessonDraft(
            nextWeekNumber,
            nextWeeklyGoal.Goal,
            targetDifficulty);
    }

    public void MarkCompleted()
    {
        Status = CompletedStatus;
    }

    private static List<LearningPlanWeeklyGoal> BuildWeeklyGoals(
        Guid learningPlanId,
        string startLevel,
        string targetLevel)
    {
        return
        [
            LearningPlanWeeklyGoal.Create(learningPlanId, 1, $"Consolidate {startLevel} foundations"),
            LearningPlanWeeklyGoal.Create(learningPlanId, 2, $"Expand grammar and vocabulary towards {targetLevel}"),
            LearningPlanWeeklyGoal.Create(learningPlanId, 3, $"Practice listening and sentence production at {targetLevel}"),
            LearningPlanWeeklyGoal.Create(learningPlanId, 4, $"Review progress and readiness check for {targetLevel}")
        ];
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
