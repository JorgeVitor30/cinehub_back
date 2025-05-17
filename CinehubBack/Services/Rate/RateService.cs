using System.Net;
using AutoMapper;
using CinehubBack.Data;
using CinehubBack.Data.Rate;
using CinehubBack.Expections;

namespace CinehubBack.Services.Rate;

public class RateService: IRateService
{
    private readonly IRepository<Model.Rate> _repository;
    private readonly IRepository<Model.User> _userRepository;
    private readonly IRepository<Model.Movie> _movieRepository;
    private readonly IMapper _mapper;

    public RateService(IRepository<Model.Rate> repository, IMapper mapper, IRepository<Model.User> userRepository, IRepository<Model.Movie> movieRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _userRepository = userRepository;
        _movieRepository = movieRepository;
    }

    public void CreateRate(CreateRateDto createRateDto)
    {
        var movie = _movieRepository.GetById(createRateDto.MovieId);
        if (movie is null)
        {
            throw new BaseException("404", HttpStatusCode.NotFound, "Movie not found");
        }
        
        var user = _userRepository.GetById(createRateDto.UserId);
        if (user is null)
        {
            throw new BaseException("404", HttpStatusCode.NotFound, "User not found");
        }
        
        if (createRateDto.RateValue <= 0 || createRateDto.RateValue > 10)
        {
            throw new BaseException("400", HttpStatusCode.BadRequest, "Rate must be between 1 and 10");
        }
        
        var rate = _repository.Raw(query => query.FirstOrDefault(r => r.UserId == createRateDto.UserId && r.MovieId == createRateDto.MovieId));
        if (rate != null)
        {
            throw new BaseException("400", HttpStatusCode.BadRequest, "Movie already rated");
        }

        var rateEntity = _mapper.Map<Model.Rate>(createRateDto);
        _repository.Create(rateEntity);
        _repository.SaveChanges();
    }

    public void UpdateRate(UpdateRateDto updateRateDto, Guid rateId)
    {
       var rate =  _repository.GetById(rateId);
       if (rate is null)
       {
           throw new BaseException("404", HttpStatusCode.NotFound, "Rate not found");
       }
       
       _mapper.Map(updateRateDto, rate);
       _repository.Update(rate);
       _repository.SaveChanges();
    }
}