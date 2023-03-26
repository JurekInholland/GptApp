using System.Text.Json.Serialization;

namespace Models.OpenAi;

public class CompletionRequest
{
    public String Model { get; set; } = "";
    public ApiMessage[] Messages { get; set; } = null!;
    public float? Temperature { get; set; } = 0.7f;
    public bool Stream { get; set; } = false;
    public int N { get; set; } = 1;

    [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; } = 100;

    [JsonPropertyName("presence_penalty")] public float PresencePenalty { get; set; } = 0;

    [JsonPropertyName("frequency_penalty")]
    public float FrequencyPenalty { get; set; } = 0;
}
