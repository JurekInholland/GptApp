using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class WebAppController : ControllerBase
{
    private static readonly string IndexHtml = System.IO.File.ReadAllText("wwwroot/index.html");

    [HttpGet("/{route?}", Name = nameof(Index))]
    public IActionResult Index([FromRoute] string? route = "")
    {
        return new ContentResult
        {
            Content = IndexHtml,

            ContentType = "text/html",
            StatusCode = 200
        };
    }
}
