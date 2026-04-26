using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public interface IParentFeedbackRepository
{
    Task AddAsync(ParentFeedback parentFeedback, CancellationToken cancellationToken);

    Task<ParentFeedback?> GetLatestByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken);
}
