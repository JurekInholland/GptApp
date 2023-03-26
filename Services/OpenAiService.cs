using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.OpenAi;

namespace Services;

public class LowercaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToLower();
    }
}

public class OpenAiService : IOpenAiService
{
    private readonly ILogger<OpenAiService> _logger;

    private JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = new LowercaseNamingPolicy()
    };
    private readonly AppConfig _config;

    private HttpClient GetClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _config.ApiKey);
        client.DefaultRequestHeaders.Add("OpenAI-Organization", _config.OrganizationId);
        return client;
    }

    public OpenAiService(ILogger<OpenAiService> logger, IOptions<AppConfig> config)
    {
        _logger = logger;
        _config = config.Value;
    }

    public async Task<Stream> GetCompletionStream(CompletionRequest request)
    {
        var json = JsonSerializer.Serialize(request, _serializerOptions);
        // fluent validation

        var response = await MakePostRequest(GetClient(), "https://api.openai.com/v1/chat/completions", json);
        var stream = await response.Content.ReadAsStreamAsync();
        return stream;
    }

    private async Task<HttpResponseMessage> MakePostRequest(HttpClient client, string url, string json)
    {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        return response;
    }
}
