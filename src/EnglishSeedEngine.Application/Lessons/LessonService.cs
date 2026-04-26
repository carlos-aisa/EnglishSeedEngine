using EnglishSeedEngine.Application.LearningPlans;
using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.Lessons;

public sealed class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IParentFeedbackRepository _parentFeedbackRepository;
    private readonly ILearningPlanRepository _learningPlanRepository;
    private readonly TimeProvider _timeProvider;

    public LessonService(
        ILessonRepository lessonRepository,
        IParentFeedbackRepository parentFeedbackRepository,
        ILearningPlanRepository learningPlanRepository,
        TimeProvider timeProvider)
    {
        _lessonRepository = lessonRepository;
        _parentFeedbackRepository = parentFeedbackRepository;
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
            var adjustedDifficulty = await ResolveAdjustedDifficultyAsync(
                learningPlan.Id,
                existingLessons,
                draft.TargetDifficulty,
                cancellationToken);

            var lesson = Lesson.Create(
                learningPlan.Id,
                draft.WeekNumber,
                draft.WeeklyFocus,
                adjustedDifficulty,
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

    private async Task<string> ResolveAdjustedDifficultyAsync(
        Guid learningPlanId,
        int existingLessons,
        string baseDifficulty,
        CancellationToken cancellationToken)
    {
        if (existingLessons == 0)
        {
            return baseDifficulty;
        }

        var latestLesson = await _lessonRepository.GetLatestByLearningPlanIdAsync(learningPlanId, cancellationToken);
        if (latestLesson is null)
        {
            return baseDifficulty;
        }

        var latestFeedback = await _parentFeedbackRepository.GetLatestByLessonIdAsync(latestLesson.Id, cancellationToken);
        if (latestFeedback is null)
        {
            return baseDifficulty;
        }

        return AdjustDifficulty(baseDifficulty, latestFeedback.DifficultyDelta);
    }

    private static string AdjustDifficulty(string baseDifficulty, int difficultyDelta)
    {
        var baseLevel = baseDifficulty switch
        {
            "Easy" => 1,
            "Medium" => 2,
            "Hard" => 3,
            _ => 2
        };

        var adjustedLevel = Math.Clamp(baseLevel + difficultyDelta, 1, 3);

        return adjustedLevel switch
        {
            1 => "Easy",
            2 => "Medium",
            3 => "Hard",
            _ => "Medium"
        };
    }
}
