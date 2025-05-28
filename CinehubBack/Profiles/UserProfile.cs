using AutoMapper;
using CinehubBack.Data.Dtos.User;
using CinehubBack.Data.Movie;
using CinehubBack.Model;

namespace CinehubBack.Profiles;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<User, ReadUserDto>()
            .ForMember(dest => dest.Favorites, opt => opt.Ignore());
        CreateMap<UpdateUserDto, User>();
        CreateMap<Movie, ReadMovieDto>();
        CreateMap<User, ReadUserByIdDto>()
            .ForMember(dest => dest.Favorites, opt => opt.Ignore());
    }
}