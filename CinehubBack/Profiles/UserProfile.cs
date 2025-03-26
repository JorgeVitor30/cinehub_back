using AutoMapper;
using CinehubBack.Data.Dtos.User;
using CinehubBack.Model;

namespace CinehubBack.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<User, ReadUserDto>();
    }
}