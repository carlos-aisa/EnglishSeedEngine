using EnglishSeedEngine.Domain.Lessons;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Lessons;

public sealed class ParentFeedbackTests
{
    [Theory]
    [InlineData("too_easy", 1)]
    [InlineData("adequate", 0)]
    [InlineData("too_hard", -1)]
    public void Create_WithValidRating_MapsDifficultyDelta(string rating, int expectedDelta)
    {
        var feedback = ParentFeedback.Create(Guid.NewGuid(), rating, DateTime.UtcNow);

        feedback.Rating.Should().Be(rating);
        feedback.DifficultyDelta.Should().Be(expectedDelta);
    }

    [Fact]
    public void Create_WithInvalidRating_ThrowsArgumentException()
    {
        var action = () => ParentFeedback.Create(Guid.NewGuid(), "invalid", DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }
}
