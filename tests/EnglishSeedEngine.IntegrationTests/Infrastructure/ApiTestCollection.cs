using Xunit;

namespace EnglishSeedEngine.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class ApiTestCollection : ICollectionFixture<ApiIntegrationTestFixture>
{
    public const string Name = "api-tests";
}

[Collection(ApiTestCollection.Name)]
public abstract class ApiTestBase : IAsyncLifetime
{
    protected ApiTestBase(ApiIntegrationTestFixture fixture)
    {
        Fixture = fixture;
    }

    protected ApiIntegrationTestFixture Fixture { get; }

    protected HttpClient Client => Fixture.Client;

    public virtual async Task InitializeAsync()
    {
        await Fixture.ResetDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}

