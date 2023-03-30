using Models.Responses;

namespace Services;

public interface ITokenizerService
{
    public Task<TokenResponse> GetTokens(string text);
}
