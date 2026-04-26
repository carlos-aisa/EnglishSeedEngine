using EnglishSeedEngine.Application.PracticeSessions;
using EnglishSeedEngine.Domain.PracticeSessions;
using Microsoft.EntityFrameworkCore;

namespace EnglishSeedEngine.Infrastructure.Persistence.Repositories;

public sealed class PracticeSessionRepository : IPracticeSessionRepository
{
    private readonly AppDbContext _dbContext;

    public PracticeSessionRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(PracticeSession practiceSession, CancellationToken cancellationToken)
    {
        await _dbContext.PracticeSessions.AddAsync(practiceSession, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<PracticeSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext.PracticeSessions
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(PracticeSession practiceSession, CancellationToken cancellationToken)
    {
        if (_dbContext.Entry(practiceSession).State == EntityState.Detached)
        {
            _dbContext.PracticeSessions.Attach(practiceSession);
            _dbContext.Entry(practiceSession).State = EntityState.Modified;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
