using EnglishSeedEngine.Domain.Lessons;
using Microsoft.Extensions.Logging;

namespace EnglishSeedEngine.Application.Lessons;

public sealed class ParentFeedbackService : IParentFeedbackService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IParentFeedbackRepository _parentFeedbackRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ParentFeedbackService> _logger;

    public ParentFeedbackService(
        ILessonRepository lessonRepository,
        IParentFeedbackRepository parentFeedbackRepository,
        TimeProvider timeProvider,
        ILogger<ParentFeedbackService> logger)
    {
        _lessonRepository = lessonRepository;
        _parentFeedbackRepository = parentFeedbackRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ParentFeedback?> SubmitAsync(Guid lessonId, string rating, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parent feedback submission started for lesson {LessonId}", lessonId);

        var lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null)
        {
            _logger.LogWarning("Parent feedback submission skipped because lesson {LessonId} was not found", lessonId);
            return null;
        }

        var parentFeedback = ParentFeedback.Create(
            lessonId,
            rating,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _parentFeedbackRepository.AddAsync(parentFeedback, cancellationToken);

        _logger.LogInformation(
            "Parent feedback submission completed for lesson {LessonId} with rating {Rating}",
            lessonId,
            parentFeedback.Rating);

        return parentFeedback;
    }
}
