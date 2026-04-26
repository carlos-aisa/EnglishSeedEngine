namespace EnglishSeedEngine.Application.PracticeSessions;

public sealed class PracticeSessionNotFinishedException : Exception
{
    public PracticeSessionNotFinishedException(Guid practiceSessionId)
        : base($"Practice session {practiceSessionId} is not finished yet.")
    {
    }
}
