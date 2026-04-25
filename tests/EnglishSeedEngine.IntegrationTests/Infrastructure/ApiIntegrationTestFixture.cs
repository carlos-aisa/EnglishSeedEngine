using EnglishSeedEngine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EnglishSeedEngine.IntegrationTests.Infrastructure;

public sealed class ApiIntegrationTestFixture : IAsyncLifetime
{
    private readonly PostgresContainerFixture _postgresContainer = new();
    private readonly RespawnCheckpoint _respawnCheckpoint = new();

    public IntegrationTestWebApplicationFactory Factory { get; private set; } = null!;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.InitializeAsync();

        Factory = new IntegrationTestWebApplicationFactory(_postgresContainer.ConnectionString);
        Client = Factory.CreateClient();

        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.EnsureCreatedAsync();

        await _respawnCheckpoint.InitializeAsync(_postgresContainer.ConnectionString);
    }

    public Task ResetDatabaseAsync()
    {
        return _respawnCheckpoint.ResetAsync(_postgresContainer.ConnectionString);
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        await Factory.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }
}
