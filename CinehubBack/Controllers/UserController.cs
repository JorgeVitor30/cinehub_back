using System.Reflection.Metadata;
using CinehubBack.Data.Dtos.User;
using CinehubBack.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parameter = CinehubBack.Data.Parameter;

namespace Api.Controllers;

[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{

    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service;
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Create([FromBody] CreateUserDto createUserDto){
        var readUserDto = _service.Create(createUserDto);
        return CreatedAtAction(nameof(GetById),new { readUserDto.Id }, readUserDto);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public IActionResult GetAll([FromQuery] string? name, [FromQuery] int size = 10, [FromQuery] int page = 0)
    {
      var parameter = new Parameter {
          Page = page, Size = size,
          Args = new Dictionary<string, object?> { {"name", name} }
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
        _service.Delete(id);
        return NoContent();
    }
    
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "User, Admin")]
    public IActionResult Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        _service.Update(id, updateUserDto);
        return NoContent();
    }

    [HttpPost("photo/{id:guid}")]
    [Authorize(Roles = "User, Admin")]
    public IActionResult UploadPhoto(Guid id,[FromForm] IFormFile file)
    {
        _service.UploadPhoto(id, file);
        return NoContent();
    }
}