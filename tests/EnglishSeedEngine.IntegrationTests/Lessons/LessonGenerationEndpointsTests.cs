using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.Lessons;

[Collection(ApiTestCollection.Name)]
public sealed class LessonGenerationEndpointsTests : ApiTestBase
{
    public LessonGenerationEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task CreateNextLesson_Returns201()
    {
        var plan = await CreateLearningPlanAsync();

        var createResponse = await Client.PostAsync($"/learning-plans/{plan.Id}/lessons:next", content: null);

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        createResponse.Headers.Location.Should().NotBeNull();

        var createdLesson = await createResponse.Content.ReadFromJsonAsync<LessonResponse>();
        createdLesson.Should().NotBeNull();
        createdLesson!.LearningPlanId.Should().Be(plan.Id);
        createdLesson.WeekNumber.Should().Be(1);
        createdLesson.TargetDifficulty.Should().Be("Easy");
        createdLesson.WeeklyFocus.Should().NotBeNullOrWhiteSpace();
        createdLesson.Status.Should().Be("Draft");
        createdLesson.CreatedAtUtc.Should().NotBe(default);

        var getResponse = await Client.GetAsync($"/lessons/{createdLesson.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var persistedLesson = await getResponse.Content.ReadFromJsonAsync<LessonResponse>();
        persistedLesson.Should().NotBeNull();
        persistedLesson.Should().BeEquivalentTo(createdLesson);
    }

    [Fact]
    public async Task CreateNextLesson_PlanCompleted_Returns409()
    {
        var plan = await CreateLearningPlanAsync();

        for (var i = 0; i < 4; i++)
        {
            var response = await Client.PostAsync($"/learning-plans/{plan.Id}/lessons:next", content: null);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        var completedResponse = await Client.PostAsync($"/learning-plans/{plan.Id}/lessons:next", content: null);

        completedResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var refreshedPlanResponse = await Client.GetAsync($"/learning-plans/{plan.Id}");
        refreshedPlanResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var refreshedPlan = await refreshedPlanResponse.Content.ReadFromJsonAsync<LearningPlanResponse>();
        refreshedPlan.Should().NotBeNull();
        refreshedPlan!.Status.Should().Be("Completed");
    }

    [Fact]
    public async Task CreateNextLesson_PlanNotFound_Returns404()
    {
        var response = await Client.PostAsync($"/learning-plans/{Guid.NewGuid()}/lessons:next", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<LearningPlanResponse> CreateLearningPlanAsync()
    {
        var student = await CreateStudentAsync();
        await SubmitInitialAssessmentAsync(student.Id, 6, 10);

        var response = await Client.PostAsync($"/students/{student.Id}/learning-plans", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var plan = await response.Content.ReadFromJsonAsync<LearningPlanResponse>();
        plan.Should().NotBeNull();

        return plan!;
    }

    private async Task<StudentResponse> CreateStudentAsync()
    {
        var request = new CreateStudentRequest
        {
            FullName = "Lesson Student",
            Age = 12,
            TutorEmail = $"parent.lesson.{Guid.NewGuid():N}@example.com",
            TargetLevel = "B1"
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
