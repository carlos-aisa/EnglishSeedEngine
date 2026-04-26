namespace EnglishSeedEngine.Domain.LearningPlans;

public sealed class LearningPlanWeeklyGoal
{
    private LearningPlanWeeklyGoal()
    {
    }

    private LearningPlanWeeklyGoal(Guid id, Guid learningPlanId, int weekNumber, string goal)
    {
        Id = id;
        LearningPlanId = learningPlanId;
        WeekNumber = weekNumber;
        Goal = goal;
    }

    public Guid Id { get; private set; }

    public Guid LearningPlanId { get; private set; }

    public int WeekNumber { get; private set; }

    public string Goal { get; private set; } = string.Empty;

    public static LearningPlanWeeklyGoal Create(Guid learningPlanId, int weekNumber, string goal)
    {
        if (learningPlanId == Guid.Empty)
        {
            throw new ArgumentException("Learning plan id is required.", nameof(learningPlanId));
        }

        if (weekNumber is < 1 or > 4)
        {
            throw new ArgumentOutOfRangeException(nameof(weekNumber), "Week number must be between 1 and 4.");
        }

        if (string.IsNullOrWhiteSpace(goal))
        {
            throw new ArgumentException("Weekly goal is required.", nameof(goal));
        }

        return new LearningPlanWeeklyGoal(
            Guid.NewGuid(),
            learningPlanId,
            weekNumber,
            goal.Trim());
    }
}
