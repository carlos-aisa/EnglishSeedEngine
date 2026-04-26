using EnglishSeedEngine.Api.Contracts.Students;

namespace EnglishSeedEngine.IntegrationTests.Students.TestData;

public sealed class CreateStudentRequestBuilder
{
    private string _fullName = "Alice Carter";
    private int _age = 11;
    private string _tutorEmail = "parent.alice@example.com";
    private string _targetLevel = "A2";

    public CreateStudentRequestBuilder WithInvalidAge()
    {
        _age = 4;
        return this;
    }

    public CreateStudentRequestBuilder WithInvalidFullName()
    {
        _fullName = string.Empty;
        return this;
    }

    public CreateStudentRequestBuilder WithInvalidTutorEmail()
    {
        _tutorEmail = "not-an-email";
        return this;
    }

    public CreateStudentRequestBuilder WithInvalidTargetLevel()
    {
        _targetLevel = "Z9";
        return this;
    }

    public CreateStudentRequest Build() => new()
    {
        FullName = _fullName,
        Age = _age,
        TutorEmail = _tutorEmail,
        TargetLevel = _targetLevel
    };
}
