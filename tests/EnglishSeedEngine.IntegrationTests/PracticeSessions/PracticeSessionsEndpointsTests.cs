using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Api.Contracts.PracticeSessions;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace EnglishSeedEngine.IntegrationTests.PracticeSessions;

[Collection(ApiTestCollection.Name)]
public sealed class PracticeSessionsEndpointsTests : ApiTestBase
{
    public PracticeSessionsEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task StartSession_Returns201()
    {
        var setup = await CreateLessonWithMaterialsAsync();

        var startResponse = await Client.PostAsJsonAsync(
            "/practice-sessions",
            new CreatePracticeSessionRequest { LessonId = setup.Lesson.Id });

        startResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        startResponse.Headers.Location.Should().NotBeNull();

        var session = await startResponse.Content.ReadFromJsonAsync<PracticeSessionResponse>();
        session.Should().NotBeNull();
        session!.LessonId.Should().Be(setup.Lesson.Id);
        session.Status.Should().Be("InProgress");
        session.TotalExercises.Should().Be(3);
        session.FinishedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task StartSession_LessonNotFound_Returns404()
    {
        var response = await Client.PostAsJsonAsync(
            "/practice-sessions",
            new CreatePracticeSessionRequest { LessonId = Guid.NewGuid() });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SubmitAnswers_AfterFinish_Returns409()
    {
        var setup = await CreateLessonWithMaterialsAsync();
        var session = await StartPracticeSessionAsync(setup.Lesson.Id);

        var firstSubmitResponse = await SubmitAnswersAsync(session.Id, setup.Material, wrongExerciseType: null);
        firstSubmitResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var finishResponse = await Client.PostAsync($"/practice-sessions/{session.Id}:finish", content: null);
        finishResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var submitAfterFinishResponse = await SubmitAnswersAsync(session.Id, setup.Material, wrongExerciseType: null);
        submitAfterFinishResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task FinishSession_ComputesScoreAndWeakPoints()
    {
        var setup = await CreateLessonWithMaterialsAsync();
        var session = await StartPracticeSessionAsync(setup.Lesson.Id);

        var submitResponse = await SubmitAnswersAsync(session.Id, setup.Material, wrongExerciseType: "translation");
        submitResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var finishResponse = await Client.PostAsync($"/practice-sessions/{session.Id}:finish", content: null);
        finishResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await finishResponse.Content.ReadFromJsonAsync<PracticeSessionResultResponse>();
        result.Should().NotBeNull();
        result!.PracticeSessionId.Should().Be(session.Id);
        result.Status.Should().Be("Finished");
        result.ScorePercentage.Should().Be(67);
        result.CorrectAnswers.Should().Be(2);
        result.TotalExercises.Should().Be(3);
        result.WeakPoints.Should().Contain("translation");

        var getResultResponse = await Client.GetAsync($"/practice-sessions/{session.Id}/result");
        getResultResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var persistedResult = await getResultResponse.Content.ReadFromJsonAsync<PracticeSessionResultResponse>();
        persistedResult.Should().NotBeNull();
        persistedResult!.ScorePercentage.Should().Be(result.ScorePercentage);
        persistedResult.WeakPoints.Should().BeEquivalentTo(result.WeakPoints);
    }

    private async Task<HttpResponseMessage> SubmitAnswersAsync(
        Guid sessionId,
        LessonMaterialResponse material,
        string? wrongExerciseType)
    {
        var answers = material.Exercises
            .Select((exercise, index) => new SubmitPracticeSessionAnswerRequest
            {
                ExerciseIndex = index + 1,
                Answer = wrongExerciseType is not null &&
                         exercise.Type.Equals(wrongExerciseType, StringComparison.OrdinalIgnoreCase)
                    ? "wrong answer"
                    : exercise.ExpectedAnswer
            })
            .ToArray();

        return await Client.PostAsJsonAsync(
            $"/practice-sessions/{sessionId}/answers",
            new SubmitPracticeSessionAnswersRequest { Answers = answers });
    }

    private async Task<PracticeSessionResponse> StartPracticeSessionAsync(Guid lessonId)
    {
        var response = await Client.PostAsJsonAsync(
            "/practice-sessions",
            new CreatePracticeSessionRequest { LessonId = lessonId });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var practiceSession = await response.Content.ReadFromJsonAsync<PracticeSessionResponse>();
        practiceSession.Should().NotBeNull();

        return practiceSession!;
    }

    private async Task<(LessonResponse Lesson, LessonMaterialResponse Material)> CreateLessonWithMaterialsAsync()
    {
        var lesson = await CreateLessonAsync();

        var generateMaterialsResponse = await Client.PostAsync($"/lessons/{lesson.Id}/materials:generate", content: null);
        generateMaterialsResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var material = await generateMaterialsResponse.Content.ReadFromJsonAsync<LessonMaterialResponse>();
        material.Should().NotBeNull();

        return (lesson, material!);
    }

    private async Task<LessonResponse> CreateLessonAsync()
    {
        var plan = await CreateLearningPlanAsync();

        var response = await Client.PostAsync($"/learning-plans/{plan.Id}/lessons:next", content: null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var lesson = await response.Content.ReadFromJsonAsync<LessonResponse>();
        lesson.Should().NotBeNull();

        return lesson!;
    }

    private async Task<LearningPlanResponse> CreateLearningPlanAsync()
    {
        var student = await CreateStudentAsync();
        await SubmitInitialAssessmentAsync(student.Id, correctAnswers: 8, totalQuestions: 10);

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
            FullName = "Practice Student",
            Age = 12,
            TutorEmail = $"parent.practice.{Guid.NewGuid():N}@example.com",
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
