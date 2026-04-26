namespace EnglishSeedEngine.Application.PracticeSessions;

public sealed class PracticeSessionFinishedException : Exception
{
    public PracticeSessionFinishedException(Guid practiceSessionId)
        : base($"Practice session {practiceSessionId} is already finished.")
    {
    }
}
