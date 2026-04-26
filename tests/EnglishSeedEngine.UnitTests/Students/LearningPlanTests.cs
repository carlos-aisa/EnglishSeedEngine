using EnglishSeedEngine.Domain.LearningPlans;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Students;

public sealed class LearningPlanTests
{
    [Fact]
    public void Create_GeneratesFourWeeklyGoals()
    {
        var plan = LearningPlan.Create(
            Guid.NewGuid(),
            "A2",
            "B1",
            DateTime.UtcNow);

        plan.WeeklyGoals.Should().HaveCount(4);
        plan.WeeklyGoals.Select(x => x.WeekNumber).Should().Equal(1, 2, 3, 4);
    }

    [Fact]
    public void Create_UsesStartAndTargetLevelsInWeeklyGoals()
    {
        var plan = LearningPlan.Create(
            Guid.NewGuid(),
            "A2",
            "B1",
            DateTime.UtcNow);

        plan.WeeklyGoals.Should().ContainSingle(x => x.WeekNumber == 1 && x.Goal.Contains("A2"));
        plan.WeeklyGoals.Should().ContainSingle(x => x.WeekNumber == 2 && x.Goal.Contains("B1"));
        plan.WeeklyGoals.Should().ContainSingle(x => x.WeekNumber == 4 && x.Goal.Contains("B1"));
    }

    [Theory]
    [InlineData(0, 1, "Easy")]
    [InlineData(1, 2, "Medium")]
    [InlineData(2, 3, "Medium")]
    [InlineData(3, 4, "Hard")]
    public void GetNextLessonDraft_ReturnsExpectedWeekAndDifficulty(
        int existingLessons,
        int expectedWeek,
        string expectedDifficulty)
    {
        var plan = LearningPlan.Create(
            Guid.NewGuid(),
            "A2",
            "B1",
            DateTime.UtcNow);

        var draft = plan.GetNextLessonDraft(existingLessons);

        draft.WeekNumber.Should().Be(expectedWeek);
        draft.TargetDifficulty.Should().Be(expectedDifficulty);
        draft.WeeklyFocus.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void GetNextLessonDraft_WhenPlanIsCompleted_ThrowsInvalidOperationException()
    {
        var plan = LearningPlan.Create(
            Guid.NewGuid(),
            "A2",
            "B1",
            DateTime.UtcNow);
        plan.MarkCompleted();

        var action = () => plan.GetNextLessonDraft(0);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void GetNextLessonDraft_WhenAllLessonsAlreadyGenerated_ThrowsInvalidOperationException()
    {
        var plan = LearningPlan.Create(
            Guid.NewGuid(),
            "A2",
            "B1",
            DateTime.UtcNow);

        var action = () => plan.GetNextLessonDraft(4);

        action.Should().Throw<InvalidOperationException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidStartLevel_ThrowsArgumentException(string invalidStartLevel)
    {
        var action = () => LearningPlan.Create(
            Guid.NewGuid(),
            invalidStartLevel,
            "B1",
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }
}
