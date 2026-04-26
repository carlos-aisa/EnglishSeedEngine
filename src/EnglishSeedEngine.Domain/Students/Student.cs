namespace EnglishSeedEngine.Domain.Students;

public sealed class Student
{
    private Student()
    {
    }

    private Student(Guid id, string fullName, int age, string tutorEmail, string targetLevel, DateTime createdAtUtc)
    {
        Id = id;
        FullName = fullName;
        Age = age;
        TutorEmail = tutorEmail;
        TargetLevel = targetLevel;
        CreatedAtUtc = createdAtUtc;
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public int Age { get; private set; }

    public string TutorEmail { get; private set; } = string.Empty;

    public string TargetLevel { get; private set; } = string.Empty;

    public DateTime CreatedAtUtc { get; private set; }

    public int? InitialAssessmentCorrectAnswers { get; private set; }

    public int? InitialAssessmentTotalQuestions { get; private set; }

    public int? InitialAssessmentScorePercentage { get; private set; }

    public string? InitialAssessmentLevel { get; private set; }

    public DateTime? InitialAssessmentCompletedAtUtc { get; private set; }

    public bool HasInitialAssessment => InitialAssessmentLevel is not null;

    public static Student Create(string fullName, int age, string tutorEmail, string targetLevel, DateTime createdAtUtc)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Full name is required.", nameof(fullName));
        }

        if (age is < 5 or > 18)
        {
            throw new ArgumentOutOfRangeException(nameof(age), "Age must be between 5 and 18.");
        }

        if (string.IsNullOrWhiteSpace(tutorEmail))
        {
            throw new ArgumentException("Tutor email is required.", nameof(tutorEmail));
        }

        if (string.IsNullOrWhiteSpace(targetLevel))
        {
            throw new ArgumentException("Target level is required.", nameof(targetLevel));
        }

        return new Student(
            Guid.NewGuid(),
            fullName.Trim(),
            age,
            tutorEmail.Trim().ToLowerInvariant(),
            targetLevel.Trim().ToUpperInvariant(),
            createdAtUtc);
    }

    public void RecordInitialAssessment(
        int correctAnswers,
        int totalQuestions,
        string level,
        DateTime completedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(level))
        {
            throw new ArgumentException("Assessment level is required.", nameof(level));
        }

        InitialAssessmentCorrectAnswers = correctAnswers;
        InitialAssessmentTotalQuestions = totalQuestions;
        InitialAssessmentScorePercentage = InitialAssessmentScoring.CalculateScorePercentage(correctAnswers, totalQuestions);
        InitialAssessmentLevel = level.Trim().ToUpperInvariant();
        InitialAssessmentCompletedAtUtc = completedAtUtc;
    }
}
