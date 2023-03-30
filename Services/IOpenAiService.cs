using Models.OpenAi;

namespace Services;

public interface IOpenAiService
{
    public Task<string> GetCompletion(CompletionRequest request);

    public Task<Stream> GetCompletionStream(CompletionRequest request);
    public Task<Stream> GetImage(ImageRequest request);
}
