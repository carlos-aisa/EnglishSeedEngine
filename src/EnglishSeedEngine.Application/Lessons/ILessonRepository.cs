using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public interface ILessonRepository
{
    Task AddAsync(Lesson lesson, CancellationToken cancellationToken);

    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<int> CountByLearningPlanIdAsync(Guid learningPlanId, CancellationToken cancellationToken);
}
