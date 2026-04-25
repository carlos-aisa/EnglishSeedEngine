using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using Xunit;

namespace EnglishSeedEngine.IntegrationTests.Students;

[Collection(ApiTestCollection.Name)]
public sealed class StudentsEndpointsTests : ApiTestBase
{
    public StudentsEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateStudent_WithValidPayload_Returns201()
    {
        var payload = new
        {
            FullName = "Alice Carter",
            Age = 11,
            TutorEmail = "parent.alice@example.com",
            TargetLevel = "A2"
        };

        var response = await Client.PostAsJsonAsync("/students", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateStudent_WithInvalidPayload_Returns400()
    {
        var payload = new
        {
            FullName = "",
            Age = 0,
            TutorEmail = "invalid-email",
            TargetLevel = "A9"
        };

        var response = await Client.PostAsJsonAsync("/students", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}

