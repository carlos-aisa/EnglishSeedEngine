using EnglishSeedEngine.Domain.LearningPlans;

namespace EnglishSeedEngine.Application.LearningPlans;

public interface ILearningPlanService
{
    Task<LearningPlan?> CreateForStudentAsync(Guid studentId, CancellationToken cancellationToken);

    Task<LearningPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
