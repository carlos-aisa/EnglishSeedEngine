using System.Net;
using System.Net.Http.Json;
using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Domain.Lessons;
using EnglishSeedEngine.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EnglishSeedEngine.IntegrationTests.Lessons;

[Collection(ApiTestCollection.Name)]
public sealed class LessonMaterialsEndpointsTests : ApiTestBase
{
    public LessonMaterialsEndpointsTests(ApiIntegrationTestFixture fixture)
        : base(fixture)
    {
    }

    [Fact]
    public async Task GenerateMaterials_Returns201()
    {
        var lesson = await CreateLessonAsync();

        var firstGenerateResponse = await Client.PostAsync($"/lessons/{lesson.Id}/materials:generate", content: null);
        firstGenerateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var firstMaterial = await firstGenerateResponse.Content.ReadFromJsonAsync<LessonMaterialResponse>();
        firstMaterial.Should().NotBeNull();
        firstMaterial!.Version.Should().Be(1);
        firstMaterial.Vocabulary.Should().NotBeEmpty();
        firstMaterial.Phrases.Should().NotBeEmpty();
        firstMaterial.Exercises.Should().HaveCount(3);
        firstMaterial.Exercises.Select(x => x.Type).Should().BeEquivalentTo(["cloze", "translation", "dictation"]);

        var secondGenerateResponse = await Client.PostAsync($"/lessons/{lesson.Id}/materials:generate", content: null);
        secondGenerateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondMaterial = await secondGenerateResponse.Content.ReadFromJsonAsync<LessonMaterialResponse>();
        secondMaterial.Should().NotBeNull();
        secondMaterial!.Version.Should().Be(2);

        var getResponse = await Client.GetAsync($"/lessons/{lesson.Id}/materials");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var materials = await getResponse.Content.ReadFromJsonAsync<LessonMaterialResponse[]>();
        materials.Should().NotBeNull();
        materials.Should().HaveCount(2);
        materials![0].Version.Should().Be(2);
        materials[1].Version.Should().Be(1);
    }

    [Fact]
    public async Task GenerateMaterials_LessonNotFound_Returns404()
    {
        var response = await Client.PostAsync($"/lessons/{Guid.NewGuid()}/materials:generate", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GenerateMaterials_AiProviderFailure_Returns503()
    {
        var lesson = await CreateLessonAsync();

        using var failingFactory = Fixture.Factory.WithWebHostBuilder(builder =>
            builder.ConfigureServices(services =>
            {
                services.RemoveAll(typeof(ILessonMaterialGenerator));
                services.AddScoped<ILessonMaterialGenerator, FailingLessonMaterialGenerator>();
            }));

        using var failingClient = failingFactory.CreateClient();

        var response = await failingClient.PostAsync($"/lessons/{lesson.Id}/materials:generate", content: null);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
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
        await SubmitInitialAssessmentAsync(student.Id, 7, 10);

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
            FullName = "Material Student",
            Age = 11,
            TutorEmail = $"parent.material.{Guid.NewGuid():N}@example.com",
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

    private sealed class FailingLessonMaterialGenerator : ILessonMaterialGenerator
    {
        public Task<GeneratedLessonMaterials> GenerateAsync(Lesson lesson, CancellationToken cancellationToken)
        {
            throw new AiProviderUnavailableException("AI provider is temporarily unavailable.");
        }
    }
}
