using EnglishSeedEngine.Domain.Students;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Students;

public sealed class StudentTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidFullName_ThrowsArgumentException(string invalidFullName)
    {
        var action = () => Student.Create(
            invalidFullName,
            11,
            "parent.alice@example.com",
            "A2",
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(4)]
    [InlineData(19)]
    public void Create_WithInvalidAge_ThrowsArgumentOutOfRangeException(int invalidAge)
    {
        var action = () => Student.Create(
            "Alice Carter",
            invalidAge,
            "parent.alice@example.com",
            "A2",
            DateTime.UtcNow);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTutorEmail_ThrowsArgumentException(string invalidTutorEmail)
    {
        var action = () => Student.Create(
            "Alice Carter",
            11,
            invalidTutorEmail,
            "A2",
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTargetLevel_ThrowsArgumentException(string invalidTargetLevel)
    {
        var action = () => Student.Create(
            "Alice Carter",
            11,
            "parent.alice@example.com",
            invalidTargetLevel,
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithValidData_ReturnsStudent()
    {
        var createdStudent = Student.Create(
            "Alice Carter",
            11,
            "parent.alice@example.com",
            "A2",
            DateTime.UtcNow);

        createdStudent.FullName.Should().Be("Alice Carter");
        createdStudent.Age.Should().Be(11);
        createdStudent.TutorEmail.Should().Be("parent.alice@example.com");
        createdStudent.TargetLevel.Should().Be("A2");
    }
}
