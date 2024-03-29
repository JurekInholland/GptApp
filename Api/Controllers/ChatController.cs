﻿using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.OpenAi;
using Services;

namespace Api.Controllers;

public class OpenAiCompletion
{
    public ApiMessage[] Messages { get; set; } = null!;
    public double Temperature { get; set; }
    public int N { get; set; }
    public bool Stream { get; set; }
    public string Model { get; set; } = "";

    [JsonPropertyName("max_tokens")] public int MaxTokens { get; set; }
}

public class LowercaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToLower();
    }
}

[ApiController]
[Route("/api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly ChatHub _hub;
    private readonly IOpenAiService _openAiService;
    private readonly ISignalRService _signalRService;

    public ChatController(ILogger<ChatController> logger, ChatHub hub, IOpenAiService openAiService, ISignalRService signalRService)
    {
        _logger = logger;
        _hub = hub;
        _openAiService = openAiService;
        _signalRService = signalRService;
    }


    [HttpPost(nameof(ServiceRequest))]
    public async Task<IActionResult> ServiceRequest([FromBody] CompletionRequest completion, string? connectionId)
    {
        // completion.Stream = false;
        if (connectionId is null || completion.Messages.Length is 0) return BadRequest("Connection Id or Messages are null");
        _logger.LogInformation("{RequestMessage} --- {ConnectionId}", completion.Messages[0].Content, completion.Messages[0].Role);
        var stream = await _openAiService.GetCompletionStream(completion);
        var result = await _signalRService.StreamToClient(connectionId, "update", stream);

        return Ok(result);
        // var res = await _openAiService.GetCompletion(completion);
        // _logger.LogInformation("{ResponseMessage}", res);
        // return Ok(res);
    }


    [HttpGet(nameof(StreamResponse))]
    public async Task<IActionResult> StreamResponse(string requestMessage, string connectionId)
    {
        _logger.LogInformation("{RequestMessage} --- {ConnectionId}", requestMessage, connectionId);
        OpenAiCompletion completion = new OpenAiCompletion
        {
            Messages = new[] {Prompts.BasedSystemMessage, new() {Content = requestMessage, Role = "user"}},
            Temperature = 0.5,
            N = 1,
            Stream = false,
            MaxTokens = 1000,
            Model = "gpt-3.5-turbo",
        };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new LowercaseNamingPolicy()
        };
        var json = JsonSerializer.Serialize(completion, options);

        HttpClient httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "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");
        httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", "org-JgXRnmTtTXa4MTTZ4VmWDDRN");


        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        StringBuilder builder = new StringBuilder();
        Stream stream = await response.Content.ReadAsStreamAsync();


        using StreamReader reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            string? line = await reader.ReadLineAsync();

            if (line is null or "") continue;
            if (line == "data: [DONE]")
            {
                break;
            }

            WebsocketResponse res = new();

            try
            {
                var cleaned = line.Replace("data: ", "").Replace("finish_reason", "finishreason");
                var doc = JsonDocument.Parse(cleaned);
                var id = doc.RootElement.GetProperty("id");

                dynamic rootObject = doc.RootElement;
                ChatCompletionChunk chunk = JsonSerializer.Deserialize<ChatCompletionChunk>(rootObject, options);

                var content = chunk.Choices[0].Delta.Content;
                res = new WebsocketResponse {Content = content, Id = chunk.Id, FinishReason = chunk.Choices[0].FinishReason};

                if (cleaned.Contains("stop"))
                {
                    Console.WriteLine("STOP");
                }

                Console.WriteLine(chunk.Choices[0]);
                if (content == "" && chunk.Choices[0].FinishReason is null)
                {
                    continue;
                    // builder.Append(content);
                }

                if (chunk.Choices[0].FinishReason != null)
                {
                    Console.WriteLine("finish reason");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + line);
            }

            try
            {
                await _hub.SendObject(connectionId, "update", res);
            }
            catch (Exception e)
            {
                Console.WriteLine("failed to send wss" + e.Message);
            }


            // _logger.LogInformation();
            // result += apiResponse?.Choices[0].Delta.Content;
        }

        var result = builder.ToString();
        Console.WriteLine(result);
        return new OkObjectResult(result);
    }

    [HttpGet(nameof(Engines))]
    public async Task<IActionResult> Engines()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "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");
        client.DefaultRequestHeaders.Add("OpenAI-Organization", "org-JgXRnmTtTXa4MTTZ4VmWDDRN");

        var response = await client.GetAsync("https://api.openai.com/v1/engines");
        var result = await response.Content.ReadAsStringAsync();
        return new OkObjectResult(result);
    }


    [HttpGet(nameof(Test))]
    public async Task<IActionResult> Test(string requestMessage)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", "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");
        client.DefaultRequestHeaders.Add("OpenAI-Organization", "org-JgXRnmTtTXa4MTTZ4VmWDDRN");


        var cont = new ApiRequest
        {
            Model = "gpt-3.5-turbo",
            Messages = new ApiMessage[] {new() {Content = requestMessage, Role = "user"}},
            Temperature = 0.5f
        };

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = new LowercaseNamingPolicy()
        };
        var obj = JsonSerializer.Serialize(cont, options);

        var res = await client.PostAsync("https://api.openai.com/v1/chat/completions",
            new StringContent(obj, Encoding.UTF8, "application/json"));

        // var response = await client.GetAsync("https://api.openai.com/v1/models");

        return new OkObjectResult(res.Content.ReadAsStringAsync().Result);
    }
}
