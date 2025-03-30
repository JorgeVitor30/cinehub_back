using AutoMapper;
using CinehubBack.Data.Movie;
using CinehubBack.Model;

namespace CinehubBack.Profiles;

public class MovieProfile: Profile{
    public MovieProfile()
    {
        CreateMap<Movie, ReadMovieDto>();
        CreateMap<CreateMovieDto, Movie>();
    }
}