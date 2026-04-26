using EnglishSeedEngine.Api.Contracts.Lessons;
using EnglishSeedEngine.Application.Lessons;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
public sealed class LessonsController : ControllerBase
{
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService)
    {
        _lessonService = lessonService;
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

    internal static class RouteNames
    {
        public const string GetLessonById = nameof(GetLessonById);
    }
}
