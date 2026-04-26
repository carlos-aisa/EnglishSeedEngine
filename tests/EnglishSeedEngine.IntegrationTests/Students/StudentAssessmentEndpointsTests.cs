using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using EnglishSeedEngine.IntegrationTests.Students.TestData;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.Students;

[Collection(ApiTestCollection.Name)]
public sealed class StudentAssessmentEndpointsTests : ApiTestBase
{
    public StudentAssessmentEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SubmitInitialAssessment_AssignsLevelAndPersistsResult()
    {
        var student = await CreateStudentAsync();
        var request = new SubmitInitialAssessmentRequest
        {
            CorrectAnswers = 8,
            TotalQuestions = 10
        };

        var postResponse = await Client.PostAsJsonAsync($"/students/{student.Id}/assessments/initial", request);

        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        postResponse.Headers.Location.Should().NotBeNull();

        var assessmentResponse = await postResponse.Content.ReadFromJsonAsync<StudentLevelResponse>();
        assessmentResponse.Should().NotBeNull();
        assessmentResponse!.StudentId.Should().Be(student.Id);
        assessmentResponse.CefrLevel.Should().Be("B1");
        assessmentResponse.ScorePercentage.Should().Be(80);
        assessmentResponse.CorrectAnswers.Should().Be(8);
        assessmentResponse.TotalQuestions.Should().Be(10);
        assessmentResponse.AssessedAtUtc.Should().NotBe(default);

        var getResponse = await Client.GetAsync($"/students/{student.Id}/level");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var persistedLevel = await getResponse.Content.ReadFromJsonAsync<StudentLevelResponse>();
        persistedLevel.Should().NotBeNull();
        persistedLevel.Should().BeEquivalentTo(assessmentResponse);
    }

    [Fact]
    public async Task SubmitInitialAssessment_StudentNotFound_Returns404NotFound()
    {
        var request = new SubmitInitialAssessmentRequest
        {
            CorrectAnswers = 5,
            TotalQuestions = 10
        };

        var response = await Client.PostAsJsonAsync($"/students/{Guid.NewGuid()}/assessments/initial", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(InvalidAssessmentRequests))]
    public async Task SubmitInitialAssessment_InvalidAnswers_Returns400BadRequest(
        SubmitInitialAssessmentRequest invalidRequest,
        string invalidField)
    {
        var student = await CreateStudentAsync();

        var response = await Client.PostAsJsonAsync($"/students/{student.Id}/assessments/initial", invalidRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest, $"because {invalidField} is invalid");
    }

    public static TheoryData<SubmitInitialAssessmentRequest, string> InvalidAssessmentRequests => new()
    {
        { new SubmitInitialAssessmentRequest { CorrectAnswers = -1, TotalQuestions = 10 }, "correctAnswers" },
        { new SubmitInitialAssessmentRequest { CorrectAnswers = 11, TotalQuestions = 10 }, "correctAnswers" },
        { new SubmitInitialAssessmentRequest { CorrectAnswers = 5, TotalQuestions = 0 }, "totalQuestions" }
    };

    private async Task<StudentResponse> CreateStudentAsync()
    {
        var response = await Client.PostAsJsonAsync("/students", new CreateStudentRequestBuilder().Build());
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();
        student.Should().NotBeNull();

        return student!;
    }
}
