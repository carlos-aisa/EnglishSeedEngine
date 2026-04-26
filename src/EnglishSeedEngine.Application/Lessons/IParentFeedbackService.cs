using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public interface IParentFeedbackService
{
    Task<ParentFeedback?> SubmitAsync(Guid lessonId, string rating, CancellationToken cancellationToken);
}
