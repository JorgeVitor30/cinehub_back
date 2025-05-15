using CinehubBack.Data.Favorite;
using CinehubBack.Data.Rate;
using CinehubBack.Services.Auth;
using CinehubBack.Services.Favorite;
using CinehubBack.Services.Rate;
using Microsoft.AspNetCore.Mvc;

namespace CinehubBack.Controllers;

[ApiController]
[Route("/api/rate")]
public class RateController: ControllerBase
{
    private readonly IAuthService _authService; 
    private readonly IRateService _rateService;
    
    public RateController(IAuthService authService, IRateService rateService)
    {
        _authService = authService;
        _rateService = rateService;
    }
    
    [HttpPost("rate")]
    public void CreateRate([FromBody] CreateRateDto createRateDto)
    {
        _rateService.CreateRate(createRateDto);
    }
    
}