namespace Domain;

public static class OpenAiUrls
{
    private const string BaseUrl = "https://api.openai.com/v1";
    public static string ImageUrl => $"{BaseUrl}/images/generations";
    public static string ChatCompletionUrl => $"{BaseUrl}/chat/completions";
    public static string CompletionUrl => $"{BaseUrl}/completions";
    public static string EngineUrl => $"{BaseUrl}/engines";
    public static string ModelUrl => $"{BaseUrl}/models";
}
