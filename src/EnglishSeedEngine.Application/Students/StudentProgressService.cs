using System.Text.Json;
using EnglishSeedEngine.Domain.Students;
using Microsoft.Extensions.Logging;

namespace EnglishSeedEngine.Application.Students;

public sealed class StudentProgressService : IStudentProgressService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IStudentProgressRepository _studentProgressRepository;
    private readonly ILogger<StudentProgressService> _logger;

    public StudentProgressService(
        IStudentRepository studentRepository,
        IStudentProgressRepository studentProgressRepository,
        ILogger<StudentProgressService> logger)
    {
        _studentRepository = studentRepository;
        _studentProgressRepository = studentProgressRepository;
        _logger = logger;
    }

    public async Task<StudentProgressOverview?> GetOverviewAsync(Guid studentId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Student progress overview requested for student {StudentId}", studentId);

        var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);
        if (student is null)
        {
            _logger.LogWarning("Student progress overview skipped because student {StudentId} was not found", studentId);
            return null;
        }

        var completedSessions = await _studentProgressRepository.GetCompletedSessionsByStudentIdAsync(studentId, cancellationToken);
        var lastFiveSessions = completedSessions
            .OrderByDescending(x => x.FinishedAtUtc)
            .Take(5)
            .ToArray();

        var hasSessions = completedSessions.Count > 0;
        var lastFiveAverageScore = StudentProgressInsights.CalculateLastFiveAverage(lastFiveSessions.Select(x => x.ScorePercentage).ToArray());

        var weakPoints = lastFiveSessions
            .SelectMany(x => DeserializeWeakPoints(x.WeakPointsJson))
            .ToArray();

        var recommendedFocus = hasSessions
            ? StudentProgressInsights.CalculateRecommendedFocus(weakPoints)
            : "Start with the first practice session.";

        var currentLevel = student.InitialAssessmentLevel ?? student.TargetLevel;

        var overview = new StudentProgressOverview(
            studentId,
            currentLevel,
            completedSessions.Count,
            lastFiveAverageScore,
            recommendedFocus,
            hasSessions);

        _logger.LogInformation(
            "Student progress overview generated for student {StudentId} with {CompletedSessions} completed sessions",
            studentId,
            overview.CompletedSessions);

        return overview;
    }

    private static IReadOnlyCollection<string> DeserializeWeakPoints(string weakPointsJson)
    {
        if (string.IsNullOrWhiteSpace(weakPointsJson))
        {
            return [];
        }

        return JsonSerializer.Deserialize<string[]>(weakPointsJson) ?? [];
    }
}
