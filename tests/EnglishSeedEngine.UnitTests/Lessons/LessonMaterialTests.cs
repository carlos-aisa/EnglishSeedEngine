using EnglishSeedEngine.Domain.Lessons;
using FluentAssertions;

namespace EnglishSeedEngine.UnitTests.Lessons;

public sealed class LessonMaterialTests
{
    [Fact]
    public void Create_WithValidPayload_CreatesVersionedMaterial()
    {
        var lessonId = Guid.NewGuid();
        var generatedAtUtc = new DateTime(2026, 4, 26, 16, 0, 0, DateTimeKind.Utc);

        var material = LessonMaterial.Create(
            lessonId,
            version: 1,
            vocabulary: ["greet", "review", "confidence"],
            phrases: ["I can greet people politely.", "I review vocabulary every day."],
            exercises:
            [
                new LessonMaterialExercise("cloze", "Complete: I ___ every day.", "practice"),
                new LessonMaterialExercise("translation", "Translate: Practico todos los dias.", "I practice every day."),
                new LessonMaterialExercise("dictation", "Write: Practice makes progress.", "Practice makes progress.")
            ],
            generatedAtUtc);

        material.Id.Should().NotBeEmpty();
        material.LessonId.Should().Be(lessonId);
        material.Version.Should().Be(1);
        material.Vocabulary.Should().Contain("greet");
        material.Phrases.Should().Contain("I can greet people politely.");
        material.Exercises.Should().HaveCount(3);
        material.Exercises.Select(x => x.Type).Should().BeEquivalentTo(["cloze", "translation", "dictation"]);
    }

    [Fact]
    public void Create_WithMissingRequiredExerciseType_ThrowsArgumentException()
    {
        var action = () => LessonMaterial.Create(
            Guid.NewGuid(),
            version: 1,
            vocabulary: ["practice"],
            phrases: ["I practice."],
            exercises:
            [
                new LessonMaterialExercise("cloze", "Complete sentence", "practice"),
                new LessonMaterialExercise("translation", "Translate sentence", "I practice.")
            ],
            DateTime.UtcNow);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithInvalidVersion_ThrowsArgumentOutOfRangeException()
    {
        var action = () => LessonMaterial.Create(
            Guid.NewGuid(),
            version: 0,
            vocabulary: ["practice"],
            phrases: ["I practice."],
            exercises:
            [
                new LessonMaterialExercise("cloze", "Complete sentence", "practice"),
                new LessonMaterialExercise("translation", "Translate sentence", "I practice."),
                new LessonMaterialExercise("dictation", "Write sentence", "I practice.")
            ],
            DateTime.UtcNow);

        action.Should().Throw<ArgumentOutOfRangeException>();
    }
}
