using CinehubBack.Data.Favorite;
using CinehubBack.Services.Auth;
using CinehubBack.Services.Favorite;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Controllers;

public class FavoriteController: ControllerBase
{
    private readonly IAuthService _authService; 
    private readonly IFavoriteService _favoriteService;
    
    public FavoriteController(IAuthService authService, IFavoriteService favoriteService)
    {
        _authService = authService;
        _favoriteService = favoriteService;
    }
    
    [HttpPost("favorite")]
    public IActionResult Favorite([FromBody] CreateFavoriteDto createFavoriteDto)
    {
        return Ok(_favoriteService.CreateFavorite(createFavoriteDto));
    }
}