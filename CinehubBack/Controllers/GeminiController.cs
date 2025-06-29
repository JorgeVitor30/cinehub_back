using CinehubBack.Data.Dtos.IA;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GeminiController : ControllerBase
{
    private readonly GeminiService _geminiService;

    public GeminiController(GeminiService geminiService)
    {
        _geminiService = geminiService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] PromptDto request)
    {
        var result = await _geminiService.GenerateContentAsync(request.Prompt);
        return Ok(new { response = result });
    }
}
