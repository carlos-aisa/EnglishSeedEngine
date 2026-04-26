namespace EnglishSeedEngine.Domain.Students;

public static class StudentProgressInsights
{
    public static int? CalculateLastFiveAverage(IReadOnlyCollection<int> scores)
    {
        ArgumentNullException.ThrowIfNull(scores);

        if (scores.Count == 0)
        {
            return null;
        }

        return (int)Math.Round(scores.Average(), MidpointRounding.AwayFromZero);
    }

    public static string CalculateRecommendedFocus(IReadOnlyCollection<string> weakPoints)
    {
        ArgumentNullException.ThrowIfNull(weakPoints);

        if (weakPoints.Count == 0)
        {
            return "Maintain current practice plan.";
        }

        var mostFrequentWeakPoint = weakPoints
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim().ToLowerInvariant())
            .GroupBy(x => x)
            .OrderByDescending(x => x.Count())
            .ThenBy(x => x.Key, StringComparer.Ordinal)
            .Select(x => x.Key)
            .FirstOrDefault();

        return mostFrequentWeakPoint switch
        {
            "cloze" => "Grammar and sentence completion",
            "translation" => "Translation accuracy",
            "dictation" => "Listening and dictation",
            null => "Maintain current practice plan.",
            _ => $"{mostFrequentWeakPoint} practice"
        };
    }
}
