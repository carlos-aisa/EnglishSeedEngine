using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class ParentFeedbackRepository : IParentFeedbackRepository
{
    private readonly AppDbContext _dbContext;

    public ParentFeedbackRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ParentFeedback parentFeedback, CancellationToken cancellationToken)
    {
        await _dbContext.ParentFeedbacks.AddAsync(parentFeedback, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<ParentFeedback?> GetLatestByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return _dbContext.ParentFeedbacks
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
