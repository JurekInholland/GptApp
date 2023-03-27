using System.Text.Json.Serialization;
using Models.Requests;

namespace Models.OpenAi;

public class ImageRequest : ApiImageRequest
{
    public ImageRequest(ApiImageRequest request)
    {
        Prompt = request.Prompt;
        Size = request.Size;
        N = 1;
        ResponseFormat = "url";
    }


    public int N { get; set; }

    [JsonPropertyName("response_format")] public string ResponseFormat { get; set; } = "";
}
