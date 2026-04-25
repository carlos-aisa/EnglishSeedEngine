namespace EnglishSeedEngine.Application.Students;

public sealed class DuplicateTutorEmailException : Exception
{
    public DuplicateTutorEmailException(string tutorEmail)
        : base($"A student with tutor email '{tutorEmail}' already exists.")
    {
        TutorEmail = tutorEmail;
    }

    public string TutorEmail { get; }
}

