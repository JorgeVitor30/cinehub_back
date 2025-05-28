using AutoMapper;
using CinehubBack.Data.Rate;
using CinehubBack.Model;

namespace CinehubBack.Profiles;

public class RateProfile: Profile
{
    public RateProfile()
    {
        CreateMap<CreateRateDto, Rate>();
        CreateMap<UpdateRateDto, Rate>();
    }
}