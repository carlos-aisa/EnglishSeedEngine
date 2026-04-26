using EnglishSeedEngine.Application.LearningPlans;
using EnglishSeedEngine.Domain.LearningPlans;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class LearningPlanRepository : ILearningPlanRepository
{
    private readonly AppDbContext _dbContext;

    public LearningPlanRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LearningPlan learningPlan, CancellationToken cancellationToken)
    {
        await _dbContext.LearningPlans.AddAsync(learningPlan, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<LearningPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.LearningPlans
            .AsNoTracking()
            .Include(x => x.WeeklyGoals)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(LearningPlan learningPlan, CancellationToken cancellationToken)
    {
        _dbContext.Attach(learningPlan);
        _dbContext.Entry(learningPlan).Property(x => x.Status).IsModified = true;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
