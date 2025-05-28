using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using CinehubBack.Data;
using CinehubBack.Data.Favorite;
using CinehubBack.Expections;

namespace CinehubBack.Services.Favorite;

public class FavoriteService: IFavoriteService
{
    private readonly IRepository<Model.Favorites> _repository;
    private readonly IRepository<Model.Movie> _movieRepository;
    private readonly IRepository<Model.User> _userRepository;
    private readonly IMapper _mapper;

    public FavoriteService(IRepository<Model.Favorites> repository, IRepository<Model.Movie> movieRepository, IRepository<Model.User> userRepository, IMapper mapper)
    {
        _repository = repository;
        _movieRepository = movieRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public ReadFavoriteDto CreateFavorite(CreateFavoriteDto createFavoriteDto)
    {
        var favorite = _mapper.Map<Model.Favorites>(createFavoriteDto);
        
        if (_repository.Raw(query => query.FirstOrDefault(f => f.UserId == createFavoriteDto.UserId && f.MovieId == createFavoriteDto.MovieId)) != null)
        {
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "Movie already Favorited");
        }

        if (_movieRepository.Raw(query => query.FirstOrDefault(m => m.Id == createFavoriteDto.MovieId)) == null)
        {
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "Movie not found");
        }
        
        if (_userRepository.Raw(query => query.FirstOrDefault(u => u.Id == createFavoriteDto.UserId)) == null)
        {
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "User not found");
        }
        
        _repository.Create(favorite);
        _repository.SaveChanges();
        return _mapper.Map<ReadFavoriteDto>(favorite);
    }

    public void DeleteFavorite(DeleteFavoriteDto deleteFavoriteDto)
    {
        var favorite = _repository.Raw(query => query.FirstOrDefault(f => f.UserId == deleteFavoriteDto.UserId && f.MovieId == deleteFavoriteDto.MovieId));
        if (favorite == null)
        {
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "Movie not Favorited");
        }
        
        _repository.Delete(favorite);
        _repository.SaveChanges();
    }
}