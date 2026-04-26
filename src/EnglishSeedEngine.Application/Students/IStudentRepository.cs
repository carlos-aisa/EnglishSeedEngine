using EnglishSeedEngine.Domain.Students;

namespace EnglishSeedEngine.Application.Students;

public interface IStudentRepository
{
    Task AddAsync(Student student, CancellationToken cancellationToken);

    Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(Student student, CancellationToken cancellationToken);

    Task<bool> TutorEmailExistsAsync(string tutorEmail, CancellationToken cancellationToken);
}
