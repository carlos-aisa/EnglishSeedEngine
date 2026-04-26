namespace EnglishSeedEngine.Application.Students;

public interface IStudentProgressRepository
{
    Task<IReadOnlyCollection<CompletedPracticeSessionSnapshot>> GetCompletedSessionsByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken);
}
