using Models.OpenAi;

namespace Services;

public interface IOpenAiService
{
    public Task<Stream> GetCompletionStream(CompletionRequest request);
}
