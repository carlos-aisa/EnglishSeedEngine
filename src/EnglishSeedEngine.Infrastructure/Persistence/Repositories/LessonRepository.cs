using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class LessonRepository : ILessonRepository
{
    private readonly AppDbContext _dbContext;

    public LessonRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        await _dbContext.Lessons.AddAsync(lesson, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.Lessons
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<int> CountByLearningPlanIdAsync(Guid learningPlanId, CancellationToken cancellationToken)
    {
        return _dbContext.Lessons
            .AsNoTracking()
            .CountAsync(x => x.LearningPlanId == learningPlanId, cancellationToken);
    }
}
