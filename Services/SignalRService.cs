using System.Text.Json;
using Domain;
using Microsoft.Extensions.Logging;
using Models;

namespace Services;

public class SignalRService : ISignalRService
{
    private readonly ILogger<SignalRService> _logger;
    private readonly ChatHub _hub;

    private JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = new LowercaseNamingPolicy()
    };

    public SignalRService(ILogger<SignalRService> logger, ChatHub chatHub)
    {
        _logger = logger;
        _hub = chatHub;
    }

    public async Task StreamToClient(string connectionId, string callbackName, Stream stream)
    {
        _logger.LogInformation("Sending object to {ConnectionId}", connectionId);
        // await _hub.Clients.Client(connectionId).SendAsync(callbackName, obj);

        using StreamReader reader = new StreamReader(stream);
        WebsocketResponse res = new WebsocketResponse();

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            if (line is null or "") continue;
            if (line == "data: [DONE]")
            {
                break;
            }

            try
            {
                var cleaned = line.Replace("data: ", "").Replace("finish_reason", "finishreason");
                var doc = JsonDocument.Parse(cleaned);

                dynamic rootObject = doc.RootElement;
                ChatCompletionChunk chunk = JsonSerializer.Deserialize<ChatCompletionChunk>(rootObject, _serializerOptions);

                var content = chunk.Choices[0].Delta.Content;

                res.Id = chunk.Id;
                res.Content = content;
                res.FinishReason = chunk.Choices[0].FinishReason;

                if (cleaned.Contains("stop"))
                {
                    Console.WriteLine("STOP");
                }

                if (content == "" && chunk.Choices[0].FinishReason is null)
                {
                    continue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error: " + e.Message + " " + line);
                Console.WriteLine(line);
            }

            try
            {
                await _hub.SendObject(connectionId, callbackName, res);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to send wss" + e.Message);
            }
        }
    }
}
