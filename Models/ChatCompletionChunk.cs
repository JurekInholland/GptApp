namespace Models;

public class ChatCompletionChunk
{
    public string Id { get; set; } = "";
    public string Object { get; set; } = "";
    public long Created { get; set; }
    public string Model { get; set; } = "";
    public Choice[] Choices { get; set; } = null!;
}

public class Choice
{
    public Delta Delta { get; set; } = null!;
    public int Index { get; set; }

    public string? FinishReason { get; set; }
    // public string ToString() => Delta.Content;
}

public class Delta
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}
