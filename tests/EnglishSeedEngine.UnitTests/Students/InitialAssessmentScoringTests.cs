using EnglishSeedEngine.Domain.Students;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Students;

public sealed class InitialAssessmentScoringTests
{
    [Theory]
    [InlineData(1, 3, 33)]
    [InlineData(2, 3, 67)]
    [InlineData(8, 10, 80)]
    public void CalculateScorePercentage_ReturnsRoundedScore(
        int correctAnswers,
        int totalQuestions,
        int expectedScorePercentage)
    {
        var score = InitialAssessmentScoring.CalculateScorePercentage(correctAnswers, totalQuestions);

        score.Should().Be(expectedScorePercentage);
    }

    [Theory]
    [InlineData(3, 10, "A1")]
    [InlineData(4, 10, "A2")]
    [InlineData(7, 10, "A2")]
    [InlineData(8, 10, "B1")]
    public void DetermineLevel_ReturnsExpectedCefrBand(
        int correctAnswers,
        int totalQuestions,
        string expectedLevel)
    {
        var level = InitialAssessmentScoring.DetermineLevel(correctAnswers, totalQuestions);

        level.Should().Be(expectedLevel);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(1, 0)]
    [InlineData(11, 10)]
    public void CalculateScorePercentage_WithInvalidAnswers_ThrowsArgumentOutOfRangeException(
        int correctAnswers,
        int totalQuestions)
    {
        var action = () => InitialAssessmentScoring.CalculateScorePercentage(correctAnswers, totalQuestions);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
