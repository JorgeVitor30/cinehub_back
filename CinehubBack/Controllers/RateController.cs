using CinehubBack.Data.Favorite;
using CinehubBack.Services.Auth;
using CinehubBack.Services.Favorite;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Controllers;

[ApiController]
[Route("/api/rate")]
public class RateController: ControllerBase
{
    private readonly IAuthService _authService; 
    private readonly IFavoriteService _favoriteService;
    
    public RateController(IAuthService authService, IFavoriteService favoriteService)
    {
        _authService = authService;
        _favoriteService = favoriteService;
    }
    
    // [HttpPost("/api/favorite")]
    // public IActionResult Favorite([FromBody] CreateFavoriteDto createFavoriteDto)
    // {
    // }
    
}