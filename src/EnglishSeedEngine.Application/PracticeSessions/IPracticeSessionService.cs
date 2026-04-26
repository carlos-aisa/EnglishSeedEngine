using EnglishSeedEngine.Domain.PracticeSessions;

namespace EnglishSeedEngine.Application.PracticeSessions;

public interface IPracticeSessionService
{
    Task<PracticeSession?> GetByIdAsync(Guid practiceSessionId, CancellationToken cancellationToken);

    Task<PracticeSession?> StartAsync(Guid lessonId, CancellationToken cancellationToken);

    Task<PracticeSession?> SubmitAnswersAsync(
        Guid practiceSessionId,
        IReadOnlyCollection<PracticeSessionAnswer> answers,
        CancellationToken cancellationToken);

    Task<PracticeSession?> FinishAsync(Guid practiceSessionId, CancellationToken cancellationToken);

    Task<PracticeSessionResult?> GetResultAsync(Guid practiceSessionId, CancellationToken cancellationToken);
}
