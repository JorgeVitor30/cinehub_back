using CinehubBack.Services.Movie;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parameter = CinehubBack.Data.Parameter;

namespace CinehubBack.Controllers;

[ApiController]
[Route("/api/movies")]
public class MovieController: ControllerBase
{
    private readonly IMovieService _service;

    public MovieController(IMovieService service)
    {
        _service = service;
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetAll([FromQuery] string? title, [FromQuery] int size = 10, [FromQuery] int page = 0)
    {
        var parameter = new Parameter {
            Page = page, Size = size,
            Args = new Dictionary<string, object?> { {"title", title} }
        };
        return Ok(_service.GetAll(parameter));
    }
    
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetById(Guid id)
    {
        return Ok(_service.GetById(id));
    }
    
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid id)
    {
        _service.DeleteById(id);
        return NoContent();
    }
}