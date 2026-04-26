using EnglishSeedEngine.Domain.Lessons;
using EnglishSeedEngine.Domain.PracticeSessions;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.PracticeSessions;

public sealed class PracticeSessionTests
{
    [Fact]
    public void Finish_WithSubmittedAnswers_ComputesScoreAndWeakPoints()
    {
        var practiceSession = PracticeSession.Start(
            Guid.NewGuid(),
            BuildLessonExercises(),
            DateTime.UtcNow);

        practiceSession.SubmitAnswers(
        [
            new PracticeSessionAnswer(1, "review"),
            new PracticeSessionAnswer(2, "incorrect answer"),
            new PracticeSessionAnswer(3, "Daily routine builds confidence.")
        ]);

        var result = practiceSession.Finish(DateTime.UtcNow);

        result.TotalExercises.Should().Be(3);
        result.CorrectAnswers.Should().Be(2);
        result.ScorePercentage.Should().Be(67);
        result.WeakPoints.Should().ContainSingle().Which.Should().Be("translation");
        practiceSession.Status.Should().Be(PracticeSession.Statuses.Finished);
    }

    [Fact]
    public void SubmitAnswers_AfterFinish_ThrowsInvalidOperationException()
    {
        var practiceSession = PracticeSession.Start(
            Guid.NewGuid(),
            BuildLessonExercises(),
            DateTime.UtcNow);

        practiceSession.Finish(DateTime.UtcNow);

        var action = () => practiceSession.SubmitAnswers(
        [
            new PracticeSessionAnswer(1, "review")
        ]);

        action.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Start_WithoutExercises_ThrowsArgumentException()
    {
        var action = () => PracticeSession.Start(Guid.NewGuid(), [], DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    private static LessonMaterialExercise[] BuildLessonExercises()
    {
        return
        [
            new LessonMaterialExercise("cloze", "Complete the sentence", "review"),
            new LessonMaterialExercise("translation", "Translate sentence", "I practice every day."),
            new LessonMaterialExercise("dictation", "Write exactly", "Daily routine builds confidence.")
        ];
    }
}
