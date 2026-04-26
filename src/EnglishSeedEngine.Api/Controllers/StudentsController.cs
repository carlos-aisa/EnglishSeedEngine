using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.Application.Students;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
[Route("students")]
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private readonly IStudentProgressService _studentProgressService;

    public StudentsController(
        IStudentService studentService,
        IStudentProgressService studentProgressService)
    {
        _studentService = studentService;
        _studentProgressService = studentProgressService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var student = await _studentService.CreateAsync(
                new CreateStudentInput(
                    request.FullName,
                    request.Age,
                    request.TutorEmail,
                    request.TargetLevel),
                cancellationToken);

            return CreatedAtRoute(
                RouteNames.GetStudentById,
                new { id = student.Id },
                ToResponse(student));
        }
        catch (DuplicateTutorEmailException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Duplicate tutor email",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpPost("{id:guid}/assessments/initial")]
    public async Task<IActionResult> SubmitInitialAssessment(
        [FromRoute] Guid id,
        [FromBody] SubmitInitialAssessmentRequest request,
        CancellationToken cancellationToken)
    {
        var student = await _studentService.SubmitInitialAssessmentAsync(
            id,
            new SubmitInitialAssessmentInput(request.CorrectAnswers, request.TotalQuestions),
            cancellationToken);

        if (student is null)
        {
            return NotFound();
        }

        return CreatedAtRoute(
            RouteNames.GetStudentLevel,
            new { id = student.Id },
            ToLevelResponse(student));
    }

    [HttpGet("{id:guid}", Name = RouteNames.GetStudentById)]
    public async Task<IActionResult> GetStudentById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);

        if (student is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(student));
    }

    [HttpGet("{id:guid}/level", Name = RouteNames.GetStudentLevel)]
    public async Task<IActionResult> GetStudentLevel([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var student = await _studentService.GetByIdAsync(id, cancellationToken);

        if (student is null || !student.HasInitialAssessment)
        {
            return NotFound();
        }

        return Ok(ToLevelResponse(student));
    }

    [HttpGet("{id:guid}/progress/overview")]
    public async Task<IActionResult> GetStudentProgressOverview([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var overview = await _studentProgressService.GetOverviewAsync(id, cancellationToken);
        if (overview is null)
        {
            return NotFound();
        }

        return Ok(ToProgressOverviewResponse(overview));
    }

    private static StudentResponse ToResponse(Domain.Students.Student student)
    {
        return new StudentResponse(
            student.Id,
            student.FullName,
            student.Age,
            student.TutorEmail,
            student.TargetLevel,
            student.CreatedAtUtc);
    }

    private static StudentLevelResponse ToLevelResponse(Domain.Students.Student student)
    {
        return new StudentLevelResponse(
            student.Id,
            student.InitialAssessmentCorrectAnswers!.Value,
            student.InitialAssessmentTotalQuestions!.Value,
            student.InitialAssessmentScorePercentage!.Value,
            student.InitialAssessmentLevel!,
            student.InitialAssessmentCompletedAtUtc!.Value);
    }

    private static StudentProgressOverviewResponse ToProgressOverviewResponse(StudentProgressOverview overview)
    {
        return new StudentProgressOverviewResponse(
            overview.StudentId,
            overview.CurrentLevel,
            overview.CompletedSessions,
            overview.LastFiveAverageScore,
            overview.RecommendedFocus,
            overview.HasSessions);
    }

    private static class RouteNames
    {
        public const string GetStudentById = nameof(GetStudentById);

        public const string GetStudentLevel = nameof(GetStudentLevel);
    }
}
