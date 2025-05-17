using CinehubBack.Data.Rate;

namespace CinehubBack.Services.Rate;

public interface IRateService
{
    void CreateRate(CreateRateDto createRateDto);
    void UpdateRate(UpdateRateDto updateRateDto, Guid rateId);
}