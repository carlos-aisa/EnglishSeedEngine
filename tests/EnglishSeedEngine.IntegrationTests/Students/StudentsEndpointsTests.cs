using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using EnglishSeedEngine.IntegrationTests.Infrastructure;

namespace EnglishSeedEngine.IntegrationTests.Students;

[Collection(ApiTestCollection.Name)]
public sealed class StudentsEndpointsTests : ApiTestBase
{
    public StudentsEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateStudent_WithValidPayload_Returns201Created()
    {
        var payload = new
        {
            fullName = "Alice Carter",
            age = 11,
            tutorEmail = "parent.alice@example.com",
            targetLevel = "A2"
        };

        var response = await Client.PostAsJsonAsync("/students", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStudent_WhenTutorEmailAlreadyExists_Returns409Conflict()
    {
        var payload = new
        {
            fullName = "Alice Carter",
            age = 11,
            tutorEmail = "parent.alice@example.com",
            targetLevel = "A2"
        };

        var firstResponse = await Client.PostAsJsonAsync("/students", payload);
        var secondResponse = await Client.PostAsJsonAsync("/students", payload);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}

