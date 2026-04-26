using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Domain.Lessons;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class LessonMaterialRepository : ILessonMaterialRepository
{
    private readonly AppDbContext _dbContext;

    public LessonMaterialRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(LessonMaterial lessonMaterial, CancellationToken cancellationToken)
    {
        await _dbContext.LessonMaterials.AddAsync(lessonMaterial, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetLatestVersionByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        var latestVersion = await _dbContext.LessonMaterials
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .Select(x => (int?)x.Version)
            .MaxAsync(cancellationToken);

        return latestVersion ?? 0;
    }

    public async Task<IReadOnlyCollection<LessonMaterial>> GetByLessonIdAsync(Guid lessonId, CancellationToken cancellationToken)
    {
        return await _dbContext.LessonMaterials
            .AsNoTracking()
            .Where(x => x.LessonId == lessonId)
            .OrderByDescending(x => x.Version)
            .ToArrayAsync(cancellationToken);
    }
}
