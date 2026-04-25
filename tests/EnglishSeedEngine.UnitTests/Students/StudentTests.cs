using EnglishSeedEngine.Domain.Students;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Students;

public sealed class StudentTests
{
    [Fact]
    public void Create_WithInvalidAge_ThrowsArgumentOutOfRangeException()
    {
        var action = () => Student.Create(
            "Alice Carter",
            3,
            "parent.alice@example.com",
            "A2",
            DateTime.UtcNow);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}

