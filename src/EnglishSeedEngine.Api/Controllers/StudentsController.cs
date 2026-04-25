using EnglishSeedEngine.Api.Contracts.Students;
using EnglishSeedEngine.Application.Students;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
[Route("students")]
public sealed class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
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

    private static class RouteNames
    {
        public const string GetStudentById = nameof(GetStudentById);
    }
}

