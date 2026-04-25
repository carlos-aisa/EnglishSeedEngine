using System.Net.Http;
using System.Threading.Tasks;
using EnglishSeedEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EnglishSeedEngine.IntegrationTests.Infrastructure;

public sealed class ApiIntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgresContainerFixture _postgres = new();
    private readonly RespawnCheckpoint _respawn = new();

    public required IntegrationTestWebApplicationFactory Factory { get; private set; }
    public required HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        await _postgres.InitializeAsync();

        Factory = new IntegrationTestWebApplicationFactory(_postgres.ConnectionString);
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();

        await _respawn.InitializeAsync(_postgres.ConnectionString);
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawn.ResetAsync(_postgres.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
