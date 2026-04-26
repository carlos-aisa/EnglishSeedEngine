using EnglishSeedEngine.Api.Contracts.LearningPlans;
using EnglishSeedEngine.Application.LearningPlans;
using Microsoft.AspNetCore.Mvc;

namespace EnglishSeedEngine.Api.Controllers;

[ApiController]
public sealed class LearningPlansController : ControllerBase
{
    private readonly ILearningPlanService _learningPlanService;

    public LearningPlansController(ILearningPlanService learningPlanService)
    {
        _learningPlanService = learningPlanService;
    }

    [HttpPost("students/{id:guid}/learning-plans")]
    public async Task<IActionResult> CreateLearningPlan([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var learningPlan = await _learningPlanService.CreateForStudentAsync(id, cancellationToken);

            if (learningPlan is null)
            {
                return NotFound();
            }

            return CreatedAtRoute(
                RouteNames.GetLearningPlanById,
                new { id = learningPlan.Id },
                ToResponse(learningPlan));
        }
        catch (MissingInitialAssessmentException ex)
        {
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Initial assessment required",
                Detail = ex.Message,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }
    }

    [HttpGet("learning-plans/{id:guid}", Name = RouteNames.GetLearningPlanById)]
    public async Task<IActionResult> GetLearningPlanById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var learningPlan = await _learningPlanService.GetByIdAsync(id, cancellationToken);

        if (learningPlan is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(learningPlan));
    }

    private static LearningPlanResponse ToResponse(Domain.LearningPlans.LearningPlan learningPlan)
    {
        var weeklyGoals = learningPlan.WeeklyGoals
            .OrderBy(x => x.WeekNumber)
            .Select(x => new LearningPlanWeeklyGoalResponse(x.WeekNumber, x.Goal))
            .ToArray();

        return new LearningPlanResponse(
            learningPlan.Id,
            learningPlan.StudentId,
            learningPlan.StartLevel,
            learningPlan.TargetLevel,
            learningPlan.Status,
            learningPlan.CreatedAtUtc,
            weeklyGoals);
    }

    private static class RouteNames
    {
        public const string GetLearningPlanById = nameof(GetLearningPlanById);
    }
}
