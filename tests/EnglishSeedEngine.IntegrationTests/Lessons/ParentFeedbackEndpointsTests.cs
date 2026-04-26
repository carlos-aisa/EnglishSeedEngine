using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.Lessons;

[Collection(ApiTestCollection.Name)]
public sealed class ParentFeedbackEndpointsTests : ApiTestBase
{
    public ParentFeedbackEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task SubmitFeedback_AdjustsNextLessonDifficulty()
    {
        var plan = await CreateLearningPlanAsync();
        var firstLesson = await CreateNextLessonAsync(plan.Id);
        firstLesson.TargetDifficulty.Should().Be("Easy");

        var submitFeedbackResponse = await Client.PostAsJsonAsync(
            $"/lessons/{firstLesson.Id}/parent-feedback",
            new SubmitParentFeedbackRequest { Rating = "too_easy" });

        submitFeedbackResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondLesson = await CreateNextLessonAsync(plan.Id);
        secondLesson.TargetDifficulty.Should().Be("Hard");
    }

    [Fact]
    public async Task SubmitFeedback_InvalidValue_Returns400()
    {
        var plan = await CreateLearningPlanAsync();
        var lesson = await CreateNextLessonAsync(plan.Id);

        var response = await Client.PostAsJsonAsync(
            $"/lessons/{lesson.Id}/parent-feedback",
            new SubmitParentFeedbackRequest { Rating = "invalid" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task SubmitFeedback_LessonNotFound_Returns404()
    {
        var response = await Client.PostAsJsonAsync(
            $"/lessons/{Guid.NewGuid()}/parent-feedback",
            new SubmitParentFeedbackRequest { Rating = "adequate" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<LessonResponse> CreateNextLessonAsync(Guid learningPlanId)
    {
        var response = await Client.PostAsync($"/learning-plans/{learningPlanId}/lessons:next", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var lesson = await response.Content.ReadFromJsonAsync<LessonResponse>();
        lesson.Should().NotBeNull();

        return lesson!;
    }

    private async Task<LearningPlanResponse> CreateLearningPlanAsync()
    {
        var student = await CreateStudentAsync();
        await SubmitInitialAssessmentAsync(student.Id, 7, 10);

        var response = await Client.PostAsync($"/students/{student.Id}/learning-plans", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var plan = await response.Content.ReadFromJsonAsync<LearningPlanResponse>();
        plan.Should().NotBeNull();

        return plan!;
    }

    private async Task<StudentResponse> CreateStudentAsync()
    {
        var response = await Client.PostAsJsonAsync("/students", new CreateStudentRequest
        {
            FullName = "Feedback Student",
            Age = 10,
            TutorEmail = $"parent.feedback.{Guid.NewGuid():N}@example.com",
            TargetLevel = "B1"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var student = await response.Content.ReadFromJsonAsync<StudentResponse>();
        student.Should().NotBeNull();

        return student!;
    }

    private async Task SubmitInitialAssessmentAsync(Guid studentId, int correctAnswers, int totalQuestions)
    {
        var response = await Client.PostAsJsonAsync(
            $"/students/{studentId}/assessments/initial",
            new SubmitInitialAssessmentRequest
            {
                CorrectAnswers = correctAnswers,
                TotalQuestions = totalQuestions
            });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
}
