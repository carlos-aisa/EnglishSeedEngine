using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Application.LessonMaterials;

public interface ILessonMaterialGenerator
{
    Task<GeneratedLessonMaterials> GenerateAsync(Lesson lesson, CancellationToken cancellationToken);
}
