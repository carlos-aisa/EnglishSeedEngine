namespace EnglishSeedEngine.Domain.Students;

public static class InitialAssessmentScoring
{
    public static int CalculateScorePercentage(int correctAnswers, int totalQuestions)
    {
        ValidateAnswers(correctAnswers, totalQuestions);

        return (int)Math.Round(
            (double)correctAnswers / totalQuestions * 100,
            MidpointRounding.AwayFromZero);
    }

    public static string DetermineLevel(int correctAnswers, int totalQuestions)
    {
        var scorePercentage = CalculateScorePercentage(correctAnswers, totalQuestions);

        if (scorePercentage < 40)
        {
            return "A1";
        }

        if (scorePercentage < 75)
        {
            return "A2";
        }

        return "B1";
    }

    private static void ValidateAnswers(int correctAnswers, int totalQuestions)
    {
        if (correctAnswers < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(correctAnswers), "Correct answers cannot be negative.");
        }

        if (totalQuestions <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalQuestions), "Total questions must be greater than zero.");
        }

        if (correctAnswers > totalQuestions)
        {
            throw new ArgumentOutOfRangeException(nameof(correctAnswers), "Correct answers cannot exceed total questions.");
        }
    }
}
