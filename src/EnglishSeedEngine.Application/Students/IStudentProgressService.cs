namespace EnglishSeedEngine.Application.Students;

public interface IStudentProgressService
{
    Task<StudentProgressOverview?> GetOverviewAsync(Guid studentId, CancellationToken cancellationToken);
}
