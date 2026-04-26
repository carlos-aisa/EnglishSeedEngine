using EnglishSeedEngine.Application.Students;
using EnglishSeedEngine.Domain.LearningPlans;

namespace EnglishSeedEngine.Application.LearningPlans;

public sealed class LearningPlanService : ILearningPlanService
{
    private readonly ILearningPlanRepository _learningPlanRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly TimeProvider _timeProvider;

    public LearningPlanService(
        ILearningPlanRepository learningPlanRepository,
        IStudentRepository studentRepository,
        TimeProvider timeProvider)
    {
        _learningPlanRepository = learningPlanRepository;
        _studentRepository = studentRepository;
        _timeProvider = timeProvider;
    }

    public async Task<LearningPlan?> CreateForStudentAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);

        if (student is null)
        {
            return null;
        }

        if (!student.HasInitialAssessment)
        {
            throw new MissingInitialAssessmentException(studentId);
        }

        var learningPlan = LearningPlan.Create(
            student.Id,
            student.InitialAssessmentLevel!,
            student.TargetLevel,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _learningPlanRepository.AddAsync(learningPlan, cancellationToken);

        return learningPlan;
    }

    public Task<LearningPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _learningPlanRepository.GetByIdAsync(id, cancellationToken);
    }
}
