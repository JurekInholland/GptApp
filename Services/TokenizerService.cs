using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Models.Responses;
using Python.Runtime;

namespace Services;

public class TokenizerService : ITokenizerService
{
    private const string ModelName = "gpt-3.5-turbo";

    private readonly ILogger<TokenizerService> _logger;
    private dynamic? _enc;
    private readonly AppConfig _config;
    // private PyObject? _enc;

    public TokenizerService(ILogger<TokenizerService> logger, IOptions<AppConfig> config)
    {
        Console.WriteLine("TokenizerService");
        _logger = logger;
        _config = config.Value;
        InitializePython();
    }

    public async Task<TokenResponse> GetTokens(string text)
    {
        // int[] tokens;

        var tokens = await Task.Run(() =>
        {
            using (Py.GIL())
            {
                if (_enc is null)
                {
                    _enc = InitializeEncoder();
                }

                dynamic encodedTokens = _enc.encode(text);
                var tokens = (int[]) encodedTokens;
                _logger.LogInformation("Tokens: {tokens}", tokens.Length);
                return tokens;
            }
        });


        return new TokenResponse {Tokens = tokens, CharacterCount = text.Length, TokenCount = tokens.Length};
    }

    private void InitializePython()
    {
        if (PythonEngine.IsInitialized) return;
        Console.WriteLine("Initializing Python");
        // Runtime.PythonDLL = _config.PythonPath;
        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    private static dynamic InitializeEncoder()
    {
        dynamic tiktoken = Py.Import("tiktoken");
        return tiktoken.encoding_for_model(ModelName);
    }
}
