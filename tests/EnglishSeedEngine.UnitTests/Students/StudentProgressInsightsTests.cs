using EnglishSeedEngine.Domain.Students;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Students;

public sealed class StudentProgressInsightsTests
{
    [Fact]
    public void CalculateLastFiveAverage_WithScores_ReturnsRoundedAverage()
    {
        var result = StudentProgressInsights.CalculateLastFiveAverage([67, 67, 67, 33, 100]);

        result.Should().Be(67);
    }

    [Fact]
    public void CalculateLastFiveAverage_WithoutScores_ReturnsNull()
    {
        var result = StudentProgressInsights.CalculateLastFiveAverage([]);

        result.Should().BeNull();
    }

    [Fact]
    public void CalculateRecommendedFocus_WithRepeatedWeakPoint_ReturnsMappedFocus()
    {
        var result = StudentProgressInsights.CalculateRecommendedFocus(["translation", "dictation", "translation"]);

        result.Should().Be("Translation accuracy");
    }
}
