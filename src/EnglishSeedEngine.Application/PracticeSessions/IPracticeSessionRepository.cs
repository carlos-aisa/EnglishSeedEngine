using EnglishSeedEngine.Domain.PracticeSessions;

namespace EnglishSeedEngine.Application.PracticeSessions;

public interface IPracticeSessionRepository
{
    Task AddAsync(PracticeSession practiceSession, CancellationToken cancellationToken);

    Task<PracticeSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(PracticeSession practiceSession, CancellationToken cancellationToken);
}
