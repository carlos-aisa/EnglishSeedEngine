using EnglishSeedEngine.Application.Students;
using EnglishSeedEngine.Domain.Students;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class StudentRepository : IStudentRepository
{
    private readonly AppDbContext _dbContext;

    public StudentRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Student student, CancellationToken cancellationToken)
    {
        await _dbContext.Students.AddAsync(student, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Student?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> TutorEmailExistsAsync(string tutorEmail, CancellationToken cancellationToken)
    {
        return _dbContext.Students
            .AsNoTracking()
            .AnyAsync(x => x.TutorEmail == tutorEmail, cancellationToken);
    }
}

