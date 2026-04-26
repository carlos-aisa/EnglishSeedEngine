using EnglishSeedEngine.Application.Lessons;
using EnglishSeedEngine.Domain.Lessons;
using Microsoft.Extensions.Logging;

namespace EnglishSeedEngine.Application.LessonMaterials;

public sealed class LessonMaterialService : ILessonMaterialService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonMaterialRepository _lessonMaterialRepository;
    private readonly ILessonMaterialGenerator _lessonMaterialGenerator;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<LessonMaterialService> _logger;

    public LessonMaterialService(
        ILessonRepository lessonRepository,
        ILessonMaterialRepository lessonMaterialRepository,
        ILessonMaterialGenerator lessonMaterialGenerator,
        TimeProvider timeProvider,
        ILogger<LessonMaterialService> logger)
    {
        _lessonRepository = lessonRepository;
        _lessonMaterialRepository = lessonMaterialRepository;
        _lessonMaterialGenerator = lessonMaterialGenerator;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<LessonMaterial?> GenerateAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Lesson material generation started for lesson {LessonId}", lessonId);

        var lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);

        if (lesson is null)
        {
            _logger.LogWarning("Lesson material generation skipped because lesson {LessonId} was not found", lessonId);
            return null;
        }

        var latestVersion = await _lessonMaterialRepository.GetLatestVersionByLessonIdAsync(lessonId, cancellationToken);
        var nextVersion = latestVersion + 1;

        try
        {
            var generatedMaterials = await _lessonMaterialGenerator.GenerateAsync(lesson, cancellationToken);

            var lessonMaterial = LessonMaterial.Create(
                lessonId,
                nextVersion,
                generatedMaterials.Vocabulary,
                generatedMaterials.Phrases,
                generatedMaterials.Exercises
                    .Select(x => new LessonMaterialExercise(x.Type, x.Prompt, x.ExpectedAnswer))
                    .ToArray(),
                _timeProvider.GetUtcNow().UtcDateTime);

            await _lessonMaterialRepository.AddAsync(lessonMaterial, cancellationToken);

            _logger.LogInformation(
                "Lesson material generation completed for lesson {LessonId} with version {Version}",
                lessonId,
                lessonMaterial.Version);

            return lessonMaterial;
        }
        catch (AiProviderUnavailableException ex)
        {
            _logger.LogError(ex, "Lesson material generation failed due to AI provider outage for lesson {LessonId}", lessonId);
            throw;
        }
    }

    public async Task<IReadOnlyCollection<LessonMaterial>?> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null)
        {
            return null;
        }

        return await _lessonMaterialRepository.GetByLessonIdAsync(lessonId, cancellationToken);
    }
}
