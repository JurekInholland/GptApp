using Models.OpenAi;

namespace Services;

public interface IOpenAiService
{
    public Task<Stream> GetCompletionStream(CompletionRequest request);
    public Task<Stream> GetImage(ImageRequest request);
}
