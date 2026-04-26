using EnglishSeedEngine.Application.LessonMaterials;
using EnglishSeedEngine.Domain.Lessons;

namespace EnglishSeedEngine.Infrastructure.LessonMaterials;

public sealed class TemplateLessonMaterialGenerator : ILessonMaterialGenerator
{
    public Task<GeneratedLessonMaterials> GenerateAsync(Lesson lesson, CancellationToken cancellationToken)
    {
        var focus = lesson.WeeklyFocus;
        var difficulty = lesson.TargetDifficulty;

        var vocabulary = new[]
        {
            $"{focus} - key verb",
            $"{focus} - key noun",
            $"{focus} - daily expression"
        };

        var phrases = new[]
        {
            $"I can explain {focus.ToLowerInvariant()} in simple sentences.",
            $"This {difficulty.ToLowerInvariant()} exercise helps me practice {focus.ToLowerInvariant()}."
        };

        var exercises = new[]
        {
            new GeneratedLessonExercise(
                "cloze",
                $"Complete the sentence: I usually ____ when we practice {focus.ToLowerInvariant()}.",
                "review"),
            new GeneratedLessonExercise(
                "translation",
                $"Translate into English: \"Esta semana practico {focus.ToLowerInvariant()}.\"",
                $"This week I practice {focus.ToLowerInvariant()}."),
            new GeneratedLessonExercise(
                "dictation",
                $"Write exactly: \"{focus} builds confidence every day.\"",
                $"{focus} builds confidence every day.")
        };

        return Task.FromResult(new GeneratedLessonMaterials(vocabulary, phrases, exercises));
    }
}
