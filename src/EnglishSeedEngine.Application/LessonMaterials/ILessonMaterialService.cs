using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.LessonMaterials;

public interface ILessonMaterialService
{
    Task<LessonMaterial?> GenerateAsync(Guid lessonId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LessonMaterial>?> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken);
}
