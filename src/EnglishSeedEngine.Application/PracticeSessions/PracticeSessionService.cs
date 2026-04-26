using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Domain.PracticeSessions;
using Microsoft.Extensions.Logging;

namespace EnglishSeedEngine.Application.PracticeSessions;

public sealed class PracticeSessionService : IPracticeSessionService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonMaterialRepository _lessonMaterialRepository;
    private readonly IPracticeSessionRepository _practiceSessionRepository;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<PracticeSessionService> _logger;

    public PracticeSessionService(
        ILessonRepository lessonRepository,
        ILessonMaterialRepository lessonMaterialRepository,
        IPracticeSessionRepository practiceSessionRepository,
        TimeProvider timeProvider,
        ILogger<PracticeSessionService> logger)
    {
        _lessonRepository = lessonRepository;
        _lessonMaterialRepository = lessonMaterialRepository;
        _practiceSessionRepository = practiceSessionRepository;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public Task<PracticeSession?> GetByIdAsync(Guid practiceSessionId, CancellationToken cancellationToken)
    {
        return _practiceSessionRepository.GetByIdAsync(practiceSessionId, cancellationToken);
    }

    public async Task<PracticeSession?> StartAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Practice session start requested for lesson {LessonId}", lessonId);

        var lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null)
        {
            _logger.LogWarning("Practice session start skipped because lesson {LessonId} was not found", lessonId);
            return null;
        }

        var lessonMaterials = await _lessonMaterialRepository.GetByLessonIdAsync(lessonId, cancellationToken);
        var latestMaterial = lessonMaterials
            .OrderByDescending(x => x.Version)
            .FirstOrDefault();

        if (latestMaterial is null)
        {
            _logger.LogWarning("Practice session start blocked because lesson {LessonId} has no generated materials", lessonId);
            throw new LessonMaterialsRequiredException(lessonId);
        }

        var practiceSession = PracticeSession.Start(
            lessonId,
            latestMaterial.Exercises,
            _timeProvider.GetUtcNow().UtcDateTime);

        await _practiceSessionRepository.AddAsync(practiceSession, cancellationToken);

        _logger.LogInformation(
            "Practice session {PracticeSessionId} started for lesson {LessonId}",
            practiceSession.Id,
            lessonId);

        return practiceSession;
    }

    public async Task<PracticeSession?> SubmitAnswersAsync(
        Guid practiceSessionId,
        IReadOnlyCollection<PracticeSessionAnswer> answers,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Practice session answer submission started for session {PracticeSessionId}", practiceSessionId);

        var practiceSession = await _practiceSessionRepository.GetByIdAsync(practiceSessionId, cancellationToken);
        if (practiceSession is null)
        {
            _logger.LogWarning("Practice session answer submission skipped because session {PracticeSessionId} was not found", practiceSessionId);
            return null;
        }

        try
        {
            practiceSession.SubmitAnswers(answers);
            await _practiceSessionRepository.UpdateAsync(practiceSession, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Practice session answer submission failed because session {PracticeSessionId} is finished", practiceSessionId);
            throw new PracticeSessionFinishedException(practiceSessionId);
        }

        _logger.LogInformation("Practice session answer submission completed for session {PracticeSessionId}", practiceSessionId);
        return practiceSession;
    }

    public async Task<PracticeSession?> FinishAsync(Guid practiceSessionId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Practice session finish started for session {PracticeSessionId}", practiceSessionId);

        var practiceSession = await _practiceSessionRepository.GetByIdAsync(practiceSessionId, cancellationToken);
        if (practiceSession is null)
        {
            _logger.LogWarning("Practice session finish skipped because session {PracticeSessionId} was not found", practiceSessionId);
            return null;
        }

        try
        {
            practiceSession.Finish(_timeProvider.GetUtcNow().UtcDateTime);
            await _practiceSessionRepository.UpdateAsync(practiceSession, cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Practice session finish failed because session {PracticeSessionId} is already finished", practiceSessionId);
            throw new PracticeSessionFinishedException(practiceSessionId);
        }

        _logger.LogInformation("Practice session finish completed for session {PracticeSessionId}", practiceSessionId);
        return practiceSession;
    }

    public async Task<PracticeSessionResult?> GetResultAsync(Guid practiceSessionId, CancellationToken cancellationToken)
    {
        var practiceSession = await _practiceSessionRepository.GetByIdAsync(practiceSessionId, cancellationToken);
        if (practiceSession is null)
        {
            return null;
        }

        try
        {
            return practiceSession.GetResult();
        }
        catch (InvalidOperationException)
        {
            throw new PracticeSessionNotFinishedException(practiceSessionId);
        }
    }
}
