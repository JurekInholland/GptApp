
namespace Models;

public class WebsocketResponse
{
    public string Id { get; set; }
    public string Content { get; set; }

    public string? FinishReason { get; set; }
}
