using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.LessonMaterials;

public interface ILessonMaterialRepository
{
    Task AddAsync(LessonMaterial lessonMaterial, CancellationToken cancellationToken);

    Task<int> GetLatestVersionByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LessonMaterial>> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken);
}
