using EnglishSeedEngine.Api.Contracts.PracticeSessions;
using EnglishSeedEngine.Application.PracticeSessions;
using EnglishSeedEngine.Domain.PracticeSessions;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
public sealed class PracticeSessionsController : ControllerBase
{
    private readonly IPracticeSessionService _practiceSessionService;

    public PracticeSessionsController(IPracticeSessionService practiceSessionService)
    {
        _practiceSessionService = practiceSessionService;
    }

    [HttpPost("practice-sessions")]
    public async Task<IActionResult> StartPracticeSession(
        [FromBody] CreatePracticeSessionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var practiceSession = await _practiceSessionService.StartAsync(request.LessonId, cancellationToken);

            if (practiceSession is null)
            {
                return NotFound();
            }

            return CreatedAtRoute(
                RouteNames.GetPracticeSessionResult,
                new { id = practiceSession.Id },
                ToResponse(practiceSession));
        }
        catch (LessonMaterialsRequiredException ex)
        {
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Lesson materials required",
                Detail = ex.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid practice session payload",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPost("practice-sessions/{id:guid}/answers")]
    public async Task<IActionResult> SubmitAnswers(
        [FromRoute] Guid id,
        [FromBody] SubmitPracticeSessionAnswersRequest request,
        CancellationToken cancellationToken)
    {
        var answers = request.Answers
            .Select(x => new PracticeSessionAnswer(x.ExerciseIndex, x.Answer))
            .ToArray();

        try
        {
            var practiceSession = await _practiceSessionService.SubmitAnswersAsync(id, answers, cancellationToken);

            if (practiceSession is null)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (PracticeSessionFinishedException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Practice session already finished",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid submitted answers",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPost("practice-sessions/{id:guid}:finish")]
    public async Task<IActionResult> FinishPracticeSession([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var practiceSession = await _practiceSessionService.FinishAsync(id, cancellationToken);

            if (practiceSession is null)
            {
                return NotFound();
            }

            var result = practiceSession.GetResult();
            return Ok(ToResultResponse(practiceSession, result));
        }
        catch (PracticeSessionFinishedException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Practice session already finished",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    [HttpGet("practice-sessions/{id:guid}/result", Name = RouteNames.GetPracticeSessionResult)]
    public async Task<IActionResult> GetPracticeSessionResult([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var practiceSession = await _practiceSessionService.GetByIdAsync(id, cancellationToken);
        if (practiceSession is null)
        {
            return NotFound();
        }

        try
        {
            var result = await _practiceSessionService.GetResultAsync(id, cancellationToken);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(ToResultResponse(practiceSession, result));
        }
        catch (PracticeSessionNotFinishedException ex)
        {
            return Conflict(new ProblemDetails
            {
                Title = "Practice session not finished",
                Detail = ex.Message,
                Status = StatusCodes.Status409Conflict
            });
        }
    }

    private static PracticeSessionResponse ToResponse(PracticeSession practiceSession)
    {
        return new PracticeSessionResponse(
            practiceSession.Id,
            practiceSession.LessonId,
            practiceSession.Status,
            practiceSession.Exercises.Count,
            practiceSession.CreatedAtUtc,
            practiceSession.FinishedAtUtc);
    }

    private static PracticeSessionResultResponse ToResultResponse(
        PracticeSession practiceSession,
        PracticeSessionResult result)
    {
        return new PracticeSessionResultResponse(
            result.PracticeSessionId,
            practiceSession.Status,
            result.TotalExercises,
            result.AnsweredExercises,
            result.CorrectAnswers,
            result.ScorePercentage,
            result.WeakPoints,
            practiceSession.FinishedAtUtc);
    }

    private static class RouteNames
    {
        public const string GetPracticeSessionResult = nameof(GetPracticeSessionResult);
    }
}
