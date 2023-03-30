using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Domain;
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

    public async Task<string> GetCompletion(CompletionRequest request)
    {
        var json = JsonSerializer.Serialize(request, _serializerOptions);
        // fluent validation

        var response = await MakePostRequest(GetClient(), OpenAiUrls.ChatCompletionUrl, json);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }

    public async Task<Stream> GetCompletionStream(CompletionRequest request)
    {
        var json = JsonSerializer.Serialize(request, _serializerOptions);
        // fluent validation

        var response = await MakePostRequest(GetClient(), OpenAiUrls.ChatCompletionUrl, json);
        var stream = await response.Content.ReadAsStreamAsync();

        // var test = await response.Content.ReadAsStringAsync();


        // var headers = response.Headers;
        return stream;
        // return new MemoryStream(Encoding.UTF8.GetBytes(test));
    }

    public async Task<Stream> GetImage(ImageRequest request)
    {
        var json = JsonSerializer.Serialize(request, _serializerOptions);

        var response = await MakePostRequest(GetClient(), OpenAiUrls.ImageUrl, json);
        var data = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(data);

        var url = doc.RootElement.GetProperty("data")[0].GetProperty("url").ToString();

        // var url = JsonSerializer.Deserialize(rootObject.data.url.ToString(), typeof(string), _serializerOptions) as string;

        using var client = new HttpClient();
        var imageResponse = await client.GetAsync(url);
        var stream = await imageResponse.Content.ReadAsStreamAsync();


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
