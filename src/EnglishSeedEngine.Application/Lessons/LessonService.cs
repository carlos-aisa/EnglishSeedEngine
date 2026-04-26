using EnglishSeedEngine.Application.LearningPlans;
using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public sealed class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILearningPlanRepository _learningPlanRepository;
    private readonly TimeProvider _timeProvider;

    public LessonService(
        ILessonRepository lessonRepository,
        ILearningPlanRepository learningPlanRepository,
        TimeProvider timeProvider)
    {
        _lessonRepository = lessonRepository;
        _learningPlanRepository = learningPlanRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Lesson?> CreateNextAsync(Guid learningPlanId, CancellationToken cancellationToken)
    {
        var learningPlan = await _learningPlanRepository.GetByIdAsync(learningPlanId, cancellationToken);

        if (learningPlan is null)
        {
            return null;
        }

        var existingLessons = await _lessonRepository.CountByLearningPlanIdAsync(learningPlanId, cancellationToken);

        try
        {
            var draft = learningPlan.GetNextLessonDraft(existingLessons);

            var lesson = Lesson.Create(
                learningPlan.Id,
                draft.WeekNumber,
                draft.WeeklyFocus,
                draft.TargetDifficulty,
                _timeProvider.GetUtcNow().UtcDateTime);

            await _lessonRepository.AddAsync(lesson, cancellationToken);

            var totalLessons = existingLessons + 1;
            if (totalLessons >= learningPlan.WeeklyGoals.Count)
            {
                learningPlan.MarkCompleted();
                await _learningPlanRepository.UpdateAsync(learningPlan, cancellationToken);
            }

            return lesson;
        }
        catch (InvalidOperationException)
        {
            throw new LearningPlanCompletedException(learningPlanId);
        }
    }

    public Task<Lesson?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _lessonRepository.GetByIdAsync(id, cancellationToken);
    }
}
