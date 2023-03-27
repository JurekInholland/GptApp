using Domain;
using Microsoft.AspNetCore.Mvc;
using Models.OpenAi;
using Models.Requests;
using Services;

namespace Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ImageController : ControllerBase
{
    private readonly ILogger<ImageController> _logger;
    private readonly ChatHub _hub;
    private readonly IOpenAiService _openAiService;
    private readonly ISignalRService _signalRService;

    public ImageController(ILogger<ImageController> logger, ChatHub hub, IOpenAiService openAiService, ISignalRService signalRService)
    {
        _logger = logger;
        _hub = hub;
        _openAiService = openAiService;
        _signalRService = signalRService;
    }

    [HttpPost(nameof(RequestImage))]
    public async Task<IActionResult> RequestImage([FromBody] ApiImageRequest imageRequest)
    {
        ImageRequest request = new ImageRequest(imageRequest);

        _logger.LogInformation("{@RequestMessage}", imageRequest);
        try
        {
            var stream = await _openAiService.GetImage(request);
            return File(stream, "image/png");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while requesting image");
            return BadRequest(e.Message);
        }
    }
}
