using EnglishSeedEngine.Domain.LearningPlans;

namespace EnglishSeedEngine.Application.LearningPlans;

public interface ILearningPlanRepository
{
    Task AddAsync(LearningPlan learningPlan, CancellationToken cancellationToken);

    Task<LearningPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task UpdateAsync(LearningPlan learningPlan, CancellationToken cancellationToken);
}
