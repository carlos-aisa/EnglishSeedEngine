using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public interface ILessonService
{
    Task<Lesson?> CreateNextAsync(Guid learningPlanId, CancellationToken cancellationToken);

    Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
}
