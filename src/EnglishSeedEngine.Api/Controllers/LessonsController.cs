using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Application.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
public sealed class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;
    private readonly ILessonMaterialService _lessonMaterialService;
    private readonly IParentFeedbackService _parentFeedbackService;

    public LessonsController(
        ILessonService lessonService,
        ILessonMaterialService lessonMaterialService,
        IParentFeedbackService parentFeedbackService)
    {
        _lessonService = lessonService;
        _lessonMaterialService = lessonMaterialService;
        _parentFeedbackService = parentFeedbackService;
    }

    [HttpGet("lessons/{id:guid}", Name = RouteNames.GetLessonById)]
    public async Task<IActionResult> GetLessonById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var lesson = await _lessonService.GetByIdAsync(id, cancellationToken);

        if (lesson is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(lesson));
    }

    [HttpPost("lessons/{id:guid}/materials:generate")]
    public async Task<IActionResult> GenerateLessonMaterials([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var lessonMaterial = await _lessonMaterialService.GenerateAsync(id, cancellationToken);

            if (lessonMaterial is null)
            {
                return NotFound();
            }

            return CreatedAtRoute(
                RouteNames.GetLessonMaterialsByLessonId,
                new { id },
                ToMaterialResponse(lessonMaterial));
        }
        catch (AiProviderUnavailableException ex)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
            {
                Title = "Lesson material generation unavailable",
                Detail = ex.Message,
                Status = StatusCodes.Status503ServiceUnavailable
            });
        }
    }

    [HttpGet("lessons/{id:guid}/materials", Name = RouteNames.GetLessonMaterialsByLessonId)]
    public async Task<IActionResult> GetLessonMaterialsByLessonId([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var lessonMaterials = await _lessonMaterialService.GetByLessonIdAsync(id, cancellationToken);

        if (lessonMaterials is null)
        {
            return NotFound();
        }

        var response = lessonMaterials
            .OrderByDescending(x => x.Version)
            .Select(ToMaterialResponse)
            .ToArray();

        return Ok(response);
    }

    [HttpPost("lessons/{id:guid}/parent-feedback")]
    public async Task<IActionResult> SubmitParentFeedback(
        [FromRoute] Guid id,
        [FromBody] SubmitParentFeedbackRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var parentFeedback = await _parentFeedbackService.SubmitAsync(id, request.Rating, cancellationToken);

            if (parentFeedback is null)
            {
                return NotFound();
            }

            return CreatedAtRoute(
                RouteNames.GetLessonById,
                new { id },
                ToParentFeedbackResponse(parentFeedback));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid parent feedback",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    internal static LessonResponse ToResponse(Domain.Lessons.Lesson lesson)
    {
        return new LessonResponse(
            lesson.Id,
            lesson.LearningPlanId,
            lesson.WeekNumber,
            lesson.WeeklyFocus,
            lesson.TargetDifficulty,
            lesson.Status,
            lesson.CreatedAtUtc);
    }

    internal static LessonMaterialResponse ToMaterialResponse(Domain.Lessons.LessonMaterial lessonMaterial)
    {
        return new LessonMaterialResponse(
            lessonMaterial.Id,
            lessonMaterial.LessonId,
            lessonMaterial.Version,
            lessonMaterial.GeneratedAtUtc,
            lessonMaterial.Vocabulary,
            lessonMaterial.Phrases,
            lessonMaterial.Exercises
                .Select(x => new LessonMaterialExerciseResponse(x.Type, x.Prompt, x.ExpectedAnswer))
                .ToArray());
    }

    internal static ParentFeedbackResponse ToParentFeedbackResponse(Domain.Lessons.ParentFeedback parentFeedback)
    {
        return new ParentFeedbackResponse(
            parentFeedback.Id,
            parentFeedback.LessonId,
            parentFeedback.Rating,
            parentFeedback.DifficultyDelta,
            parentFeedback.SubmittedAtUtc);
    }

    internal static class RouteNames
    {
        public const string GetLessonById = nameof(GetLessonById);

        public const string GetLessonMaterialsByLessonId = nameof(GetLessonMaterialsByLessonId);
    }
}
