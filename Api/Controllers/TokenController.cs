using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenizerService _tokenizerService;

    public TokenController(ILogger<TokenController> logger, ITokenizerService tokenizerService)
    {
        _logger = logger;
        _tokenizerService = tokenizerService;
    }

    [HttpPost(nameof(GetTokenCount))]
    public async Task<IActionResult> GetTokenCount([FromBody] string text)
    {
        var tokens = await _tokenizerService.GetTokens(text);
        return Ok(tokens);
    }
}
