using System.Text;
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

    public async Task<string> StreamToClient(string connectionId, string callbackName, Stream stream)
    {
        _logger.LogInformation("Sending object to {ConnectionId}", connectionId);
        // await _hub.Clients.Client(connectionId).SendAsync(callbackName, obj);

        using StreamReader reader = new StreamReader(stream);
        WebsocketResponse res = new WebsocketResponse();

        StringBuilder sb = new StringBuilder();
        bool isError = false;

        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();
            if (line is null or "" ) continue;
            // if (line == "data: [DONE]")
            // {
            //     break;
            // }
            // if (line.Contains("message"))
            // {
            //     isError = true;
            // }

            try
            {

                // var cleaned = line;
                var cleaned = line.Replace("data: ", "")
                .Replace("finish_reason", "finishreason")
                //         // .Replace("message: ", "")
                //     .Replace(""" "message": """, "")
                ;
                // if (isError)
                // {
                //     cleaned = cleaned.Replace(""".",""", """." """);
                //
                // }
                Console.WriteLine(cleaned);
                JsonDocument doc = null;
                try
                {
                    doc = JsonDocument.Parse(cleaned);
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    sb.Append(line);
                    continue;
                }

                if (isError)
                {
                    // res.Id = "error";
                    // res.Content = cleaned;
                    // res.FinishReason = "error";
                    // await _hub.SendObject(connectionId, callbackName, res);
                    sb.Append(cleaned);
                    continue;
                }
                // if (cleaned.Contains("error"))
                // {
                //     isError = true;
                //     continue;
                // }
                //
                // if (isError)
                // {
                //     sb.Append(cleaned);
                //     continue;
                // }

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

                sb.Append(res.Content);
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

        return sb.ToString();
    }
}
