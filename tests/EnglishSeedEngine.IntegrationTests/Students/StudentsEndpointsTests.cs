using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using EnglishSeedEngine.IntegrationTests.Students.TestData;
using FluentAssertions;

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
        var response = await CreateStudentAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStudent_WhenTutorEmailAlreadyExists_Returns409Conflict()
    {
        var payload = new CreateStudentRequestBuilder().Build();

        var firstResponse = await Client.PostAsJsonAsync("/students", payload);
        var secondResponse = await Client.PostAsJsonAsync("/students", payload);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task GetStudentById_WithNonExistingId_Returns404NotFound()
    {
        var nonExistingId = Guid.NewGuid();
        var response = await Client.GetAsync($"/students/{nonExistingId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(InvalidCreateStudentRequests))]
    public async Task CreateStudent_WithInvalidPayload_Returns400BadRequest(CreateStudentRequest invalidRequest, string invalidField)
    {
        var response = await Client.PostAsJsonAsync("/students", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, $"because {invalidField} is invalid");
    }

    [Fact]
    public async Task GetStudentById_AfterCreate_ReturnsPersistedStudent()
    {
        var createResponse = await CreateStudentAsync();

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse.Headers.Location.Should().NotBeNull();

        var createdStudent = await createResponse.Content.ReadFromJsonAsync<StudentResponse>();
        createdStudent.Should().NotBeNull();
        createdStudent!.Id.Should().NotBe(Guid.Empty);

        var getResponse = await Client.GetAsync($"/students/{createdStudent.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var persistedStudent = await getResponse.Content.ReadFromJsonAsync<StudentResponse>();
        persistedStudent.Should().NotBeNull();
        persistedStudent!.Id.Should().Be(createdStudent.Id);
        persistedStudent.FullName.Should().Be("Alice Carter");
        persistedStudent.Age.Should().Be(11);
        persistedStudent.TutorEmail.Should().Be("parent.alice@example.com");
        persistedStudent.TargetLevel.Should().Be("A2");
        persistedStudent.CreatedAtUtc.Should().NotBe(default);
    }

    private Task<HttpResponseMessage> CreateStudentAsync()
    {
        return Client.PostAsJsonAsync("/students", new CreateStudentRequestBuilder().Build());
    }

    public static TheoryData<CreateStudentRequest, string> InvalidCreateStudentRequests => new()
    {
        { new CreateStudentRequestBuilder().WithInvalidFullName().Build(), "fullName" },
        { new CreateStudentRequestBuilder().WithInvalidAge().Build(), "age" },
        { new CreateStudentRequestBuilder().WithInvalidTutorEmail().Build(), "tutorEmail" },
        { new CreateStudentRequestBuilder().WithInvalidTargetLevel().Build(), "targetLevel" }
    };
}
