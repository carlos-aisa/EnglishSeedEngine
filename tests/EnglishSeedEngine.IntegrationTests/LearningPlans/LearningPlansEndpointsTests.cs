using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.LearningPlans;

[Collection(ApiTestCollection.Name)]
public sealed class LearningPlansEndpointsTests : ApiTestBase
{
    public LearningPlansEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreatePlan_FromAssessment_Returns201()
    {
        var student = await CreateStudentAsync("B1");
        await SubmitInitialAssessmentAsync(student.Id, correctAnswers: 5, totalQuestions: 10);

        var createResponse = await Client.PostAsync($"/students/{student.Id}/learning-plans", content: null);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse.Headers.Location.Should().NotBeNull();

        var createdPlan = await createResponse.Content.ReadFromJsonAsync<LearningPlanResponse>();
        createdPlan.Should().NotBeNull();
        createdPlan!.StudentId.Should().Be(student.Id);
        createdPlan.StartLevel.Should().Be("A2");
        createdPlan.TargetLevel.Should().Be("B1");
        createdPlan.Status.Should().Be("Active");
        createdPlan.WeeklyGoals.Should().HaveCount(4);
        createdPlan.WeeklyGoals.Select(x => x.WeekNumber).Should().Equal(1, 2, 3, 4);

        var getResponse = await Client.GetAsync($"/learning-plans/{createdPlan.Id}");

        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var persistedPlan = await getResponse.Content.ReadFromJsonAsync<LearningPlanResponse>();
        persistedPlan.Should().NotBeNull();
        persistedPlan.Should().BeEquivalentTo(createdPlan);
    }

    [Fact]
    public async Task CreatePlan_WithoutAssessment_Returns422()
    {
        var student = await CreateStudentAsync("B1");

        var response = await Client.PostAsync($"/students/{student.Id}/learning-plans", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task GetPlan_NotFound_Returns404()
    {
        var response = await Client.GetAsync($"/learning-plans/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<StudentResponse> CreateStudentAsync(string targetLevel)
    {
        var request = new CreateStudentRequest
        {
            FullName = "Plan Student",
            Age = 12,
            TutorEmail = $"parent.plan.{Guid.NewGuid():N}@example.com",
            TargetLevel = targetLevel
        };

        var response = await Client.PostAsJsonAsync("/students", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();
        student.Should().NotBeNull();

        return student!;
    }

    private async Task SubmitInitialAssessmentAsync(Guid studentId, int correctAnswers, int totalQuestions)
    {
        var request = new SubmitInitialAssessmentRequest
        {
            CorrectAnswers = correctAnswers,
            TotalQuestions = totalQuestions
        };

        var response = await Client.PostAsJsonAsync($"/students/{studentId}/assessments/initial", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
