using EnglishSeedEngine.Domain.Students;

namespace EnglishSeedEngine.Application.Students;

public interface IStudentService
{
    Task<Student> CreateAsync(CreateStudentInput input, CancellationToken cancellationToken);

    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}

