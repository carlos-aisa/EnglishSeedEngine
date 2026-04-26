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
