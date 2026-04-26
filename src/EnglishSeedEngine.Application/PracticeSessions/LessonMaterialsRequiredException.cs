namespace EnglishSeedEngine.Application.PracticeSessions;

public sealed class LessonMaterialsRequiredException : Exception
{
    public LessonMaterialsRequiredException(Guid lessonId)
        : base($"Lesson {lessonId} requires generated materials before starting a practice session.")
    {
    }
}
