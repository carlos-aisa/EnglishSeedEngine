using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Api.Contracts.PracticeSessions;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.Students;

[Collection(ApiTestCollection.Name)]
public sealed class StudentProgressOverviewEndpointsTests : ApiTestBase
{
    public StudentProgressOverviewEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GetOverview_ReturnsAggregatedMetrics()
    {
        var setup = await CreateLessonWithMaterialsSetupAsync();

        await CompletePracticeSessionAsync(setup.Lesson.Id, setup.Material, ["translation"]);
        await CompletePracticeSessionAsync(setup.Lesson.Id, setup.Material, ["translation"]);
        await CompletePracticeSessionAsync(setup.Lesson.Id, setup.Material, ["dictation"]);
        await CompletePracticeSessionAsync(setup.Lesson.Id, setup.Material, ["translation", "dictation"]);
        await CompletePracticeSessionAsync(setup.Lesson.Id, setup.Material, []);

        var response = await Client.GetAsync($"/students/{setup.Student.Id}/progress/overview");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var overview = await response.Content.ReadFromJsonAsync<StudentProgressOverviewResponse>();
        overview.Should().NotBeNull();
        overview!.StudentId.Should().Be(setup.Student.Id);
        overview.CurrentLevel.Should().Be("A2");
        overview.CompletedSessions.Should().Be(5);
        overview.LastFiveAverageScore.Should().Be(67);
        overview.RecommendedFocus.Should().Be("Translation accuracy");
        overview.HasSessions.Should().BeTrue();
    }

    [Fact]
    public async Task GetOverview_StudentNotFound_Returns404()
    {
        var response = await Client.GetAsync($"/students/{Guid.NewGuid()}/progress/overview");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetOverview_NoSessions_ReturnsEmptyState()
    {
        var student = await CreateStudentAsync("No Session Student");

        var response = await Client.GetAsync($"/students/{student.Id}/progress/overview");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var overview = await response.Content.ReadFromJsonAsync<StudentProgressOverviewResponse>();
        overview.Should().NotBeNull();
        overview!.StudentId.Should().Be(student.Id);
        overview.CompletedSessions.Should().Be(0);
        overview.LastFiveAverageScore.Should().BeNull();
        overview.RecommendedFocus.Should().Be("Start with the first practice session.");
        overview.HasSessions.Should().BeFalse();
    }

    private async Task CompletePracticeSessionAsync(
        Guid lessonId,
        LessonMaterialResponse material,
        IReadOnlyCollection<string> wrongTypes)
    {
        var startResponse = await Client.PostAsJsonAsync(
            "/practice-sessions",
            new CreatePracticeSessionRequest { LessonId = lessonId });
        startResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var session = await startResponse.Content.ReadFromJsonAsync<PracticeSessionResponse>();
        session.Should().NotBeNull();

        var answers = material.Exercises
            .Select((exercise, index) => new SubmitPracticeSessionAnswerRequest
            {
                ExerciseIndex = index + 1,
                Answer = wrongTypes.Contains(exercise.Type, StringComparer.OrdinalIgnoreCase)
                    ? "wrong answer"
                    : exercise.ExpectedAnswer
            })
            .ToArray();

        var submitResponse = await Client.PostAsJsonAsync(
            $"/practice-sessions/{session!.Id}/answers",
            new SubmitPracticeSessionAnswersRequest { Answers = answers });
        submitResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var finishResponse = await Client.PostAsync($"/practice-sessions/{session.Id}:finish", content: null);
        finishResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private async Task<(StudentResponse Student, LessonResponse Lesson, LessonMaterialResponse Material)> CreateLessonWithMaterialsSetupAsync()
    {
        var student = await CreateStudentAsync("Progress Student");
        await SubmitInitialAssessmentAsync(student.Id, 7, 10);

        var plan = await CreateLearningPlanAsync(student.Id);
        var lesson = await CreateNextLessonAsync(plan.Id);
        var material = await GenerateLessonMaterialsAsync(lesson.Id);

        return (student, lesson, material);
    }

    private async Task<StudentResponse> CreateStudentAsync(string fullName)
    {
        var response = await Client.PostAsJsonAsync("/students", new CreateStudentRequest
        {
            FullName = fullName,
            Age = 12,
            TutorEmail = $"parent.progress.{Guid.NewGuid():N}@example.com",
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

    private async Task<LearningPlanResponse> CreateLearningPlanAsync(Guid studentId)
    {
        var response = await Client.PostAsync($"/students/{studentId}/learning-plans", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var plan = await response.Content.ReadFromJsonAsync<LearningPlanResponse>();
        plan.Should().NotBeNull();

        return plan!;
    }

    private async Task<LessonResponse> CreateNextLessonAsync(Guid learningPlanId)
    {
        var response = await Client.PostAsync($"/learning-plans/{learningPlanId}/lessons:next", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var lesson = await response.Content.ReadFromJsonAsync<LessonResponse>();
        lesson.Should().NotBeNull();

        return lesson!;
    }

    private async Task<LessonMaterialResponse> GenerateLessonMaterialsAsync(Guid lessonId)
    {
        var response = await Client.PostAsync($"/lessons/{lessonId}/materials:generate", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var material = await response.Content.ReadFromJsonAsync<LessonMaterialResponse>();
        material.Should().NotBeNull();

        return material!;
    }
}
