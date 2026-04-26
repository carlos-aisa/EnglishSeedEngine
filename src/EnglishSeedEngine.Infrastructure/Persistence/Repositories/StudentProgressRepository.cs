using EnglishSeedEngine.Application.Students;
using EnglishSeedEngine.Domain.PracticeSessions;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class StudentProgressRepository : IStudentProgressRepository
{
    private readonly AppDbContext _dbContext;

    public StudentProgressRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<CompletedPracticeSessionSnapshot>> GetCompletedSessionsByStudentIdAsync(
        Guid studentId,
        CancellationToken cancellationToken)
    {
        return await (
            from practiceSession in _dbContext.PracticeSessions.AsNoTracking()
            join lesson in _dbContext.Lessons.AsNoTracking() on practiceSession.LessonId equals lesson.Id
            join learningPlan in _dbContext.LearningPlans.AsNoTracking() on lesson.LearningPlanId equals learningPlan.Id
            where learningPlan.StudentId == studentId
                  && practiceSession.Status == PracticeSession.Statuses.Finished
                  && practiceSession.ScorePercentage.HasValue
                  && practiceSession.FinishedAtUtc.HasValue
            orderby practiceSession.FinishedAtUtc descending
            select new CompletedPracticeSessionSnapshot(
                practiceSession.ScorePercentage!.Value,
                practiceSession.FinishedAtUtc!.Value,
                practiceSession.WeakPointsJson))
            .ToArrayAsync(cancellationToken);
    }
}
