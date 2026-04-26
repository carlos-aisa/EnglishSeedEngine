namespace EnglishSeedEngine.Application.LessonMaterials;

public sealed class AiProviderUnavailableException : Exception
{
    public AiProviderUnavailableException(string message)
        : base(message)
    {
    }

    public AiProviderUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
