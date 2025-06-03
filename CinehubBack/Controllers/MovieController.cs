using CinehubBack.Data.Movie;
using CinehubBack.Middlewares.Auth;
using CinehubBack.Services.Auth;
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
    private readonly ITokenService _tokenService;

    public MovieController(IMovieService service, ITokenService tokenService)
    {
        _service = service;
        _tokenService = tokenService;
    }

    //[Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Create([FromBody] CreateMovieDto createMovieDto)
    {
        var readMovieDto = _service.Create(createMovieDto);
        return CreatedAtAction(nameof(GetById), new { readMovieDto.Id }, readMovieDto);
    }
    
    [Authorize(Roles = "Admin, User")]
    [HttpGet]
    public IActionResult GetAll(
        [FromQuery] string? title,
        [FromQuery] string? genre = null,
        [FromQuery] decimal note = 0,
        [FromQuery] string? sortBy = null,
        [FromQuery] int size = 10,
        [FromQuery] int page = 0)
    {
        var token = BearerHandler.ExtractTokenFromHeader(Request.Headers);
        var userId = _tokenService.GetUserIdFromToken(token);
        var parameter = new Parameter {
            Page = page,
            Size = size,
            Args = new Dictionary<string, object?> {
                {"title", title},
                {"genre", genre},
                {"note", note},
                {"sortBy", sortBy},
            }
        };
        return Ok(_service.GetAll(parameter, userId));
    }
    
    [HttpGet("home")]
    public IActionResult GetHome()
    {
        return Ok(_service.GetHome());
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

    [HttpPost("{id:guid}/photo")]
    //[Authorize(Roles = "Admin")]
    public IActionResult Create(Guid id, [FromForm] AddMoviePhotosDto addMoviePhotos)
    {
        return Ok(_service.AddPhotoMovies(id, addMoviePhotos));
    }
}