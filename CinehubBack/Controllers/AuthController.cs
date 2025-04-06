using CinehubBack.Data.Auth;
using CinehubBack.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService; 

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        return Ok(_authService.Login(dto));
    }
    
    [HttpPost("decode")]
    public IActionResult Decode([FromBody] DecodeDto decodeDto)
    {
        return Ok(_authService.Decode(decodeDto.Token));
    }
}