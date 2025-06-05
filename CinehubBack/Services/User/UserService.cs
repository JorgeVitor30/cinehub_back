using System.Linq.Expressions;
using System.Net;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CinehubBack.Data;
using CinehubBack.Data.Dtos.User;
using CinehubBack.Data.Movie;
using CinehubBack.Data.Rate;
using CinehubBack.Encrypt;
using CinehubBack.Expections;
using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Services.User;

public class UserService : IUserService
{

    private readonly IRepository<Model.User> _repository;
    private readonly IRepository<Model.Rate> _rateRepository;
    private readonly IRepository<Model.Movie> _movieRepository;
    private readonly IMapper _mapper;
    private readonly IPasswordEncoder _passwordEncoder;

    public UserService(IRepository<Model.User> repository, IMapper mapper, IPasswordEncoder passwordEncoder, IRepository<Model.Rate> rateRepository, IRepository<Model.Movie> movieRepository)
    {
        _repository = repository;
        _rateRepository = rateRepository;
        _movieRepository = movieRepository;
        _mapper = mapper;
        _passwordEncoder = passwordEncoder;
    }

    public ReadUserDto Create(CreateUserDto createUserDto)
    {
        var user = _mapper.Map<Model.User>(createUserDto);
        user.Role = Role.User;
        user.Password = _passwordEncoder.Encode(user.Password);
        
        CheckForDuplicate(u => u.Email  == user.Email, "User with this email already exists");

        _repository.Create(user);
        _repository.SaveChanges();
        return _mapper.Map<ReadUserDto>(user);
    }

    public void Delete(Guid id)
    {
        _repository.DeleteById(id);
        _repository.SaveChanges();
    }

    public Page<ReadUserDto> GetAll(Parameter parameter)
    {
        return _repository.GetAll<ReadUserDto>(query =>
        {
            var name = parameter.Get<string>("name");
            if (!string.IsNullOrEmpty(name))
                query = query.Where(u => EF.Functions.Like(u.Name, $"%{name}%"));

            return query
                .Include(u => u.Favorites)
                .ThenInclude(f => f.Movie)
                .ProjectTo<ReadUserDto>(_mapper.ConfigurationProvider);
        }, parameter);
    }

    public Model.User? GetByEmail(string email)
    {
        return _repository.Raw<Model.User?>(
            query => query.FirstOrDefault(u => u.Email.Equals(email)));
    }

    public ReadUserByIdDto GetById(Guid id)
    {
        var user = _repository.Raw(query => query
            .Include(u => u.Favorites)
            .ThenInclude(f => f.Movie)
            .FirstOrDefault(u => u.Id == id)
        );
        if (user is null)
        {  
            throw new Exception("Usuário não encontrado");
        }
        var readUserDto = _mapper.Map<ReadUserByIdDto>(user);
        if (user.Photo != null)
        {
            readUserDto.Photo = $"data:image/jpeg;base64,{Convert.ToBase64String(user.Photo)}";
        }
        
        readUserDto.Favorites = user.Favorites
            .Where(f => f.Movie != null)
            .Select(f => _mapper.Map<ReadMovieDto>(f.Movie))
            .ToList();

        var ratedListDtoRate = new List<ReadRateDto?>();
        string mostCommonGenre = "Em Breve";
        var ratedList = _rateRepository.Raw(q => q.Where(r=> r.UserId == id)).ToList();
        if (ratedList.Count < 1)
        {
            ratedListDtoRate = null;
        }
        else
        {
            Dictionary<string, int> genreCount = new Dictionary<string, int>();
            foreach (var rateEntity in ratedList)
            {
                var movie = _movieRepository.GetById(rateEntity.MovieId);
                foreach (var genre in movie.Genres.Split(","))
                {
                    if (genreCount.ContainsKey(genre))
                    {
                        genreCount[genre]++;
                    }
                    else
                    {
                        genreCount[genre] = 1;
                    }
                }
                mostCommonGenre = genreCount.OrderByDescending(g => g.Value).FirstOrDefault().Key;
                var movieDto = _mapper.Map<ReadMovieDto>(movie);
                var rateDto = new ReadRateDto()
                {
                    Rate = rateEntity.RateValue,
                    Comment = rateEntity.Comment,
                    Movie = movieDto,
                    Id = rateEntity.Id,
                };
                ratedListDtoRate.Add(rateDto);
            }
        }
        
        readUserDto.RatedList = ratedListDtoRate;
        readUserDto.Genre = mostCommonGenre;

        return readUserDto;
    }

    private Model.User GetByIdOrThrow(Guid id)
    {
        return _repository.GetById(id)
               ?? throw new BaseException(
                   ErrorCode.NotFound<Model.User>(),
                   HttpStatusCode.NotFound,
                   "User not found"
               );
    }
    
    public void Update(Guid id, UpdateUserDto updateUserDto)
    {
        var user = GetByIdOrThrow(id);
        user.VisibilityPublic = updateUserDto.VisibilityPublic;
        user.Email = updateUserDto.Email ?? user.Email;
        user.Name = updateUserDto.Name ?? user.Name;

        CheckForDuplicate(u => u.Email == user.Email && u.Id != id, "User with this email already exists");
        _repository.Update(user);
        _repository.SaveChanges();
    }

    public void SaveChanges()
    {
        _repository.SaveChanges();
    }
    
    private void CheckForDuplicate(Expression<Func<Model.User, bool>> predicate, string errorMessage)
    {
        var exists = _repository.Raw<Model.User?>(query => query.FirstOrDefault(predicate));

        if (exists is not null)
        {
            throw new BaseException(
                ErrorCode.BadRequest(),
                HttpStatusCode.BadRequest,
                errorMessage
            );
        }
    }
    
    public async Task UploadPhoto(Guid id, IFormFile file)
    {
        var user = GetByIdOrThrow(id);

        if (file == null || file.Length == 0)
            throw new BaseException(ErrorCode.BadRequest(), HttpStatusCode.BadRequest, "File is required");

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        user.Photo = memoryStream.ToArray();

        _repository.SaveChanges();
    }

    public void ChangePassword(Guid id, ChangePasswordDto changePasswordDto)
    {
        var user = GetByIdOrThrow(id);
        if (!_passwordEncoder.Verify(changePasswordDto.LastPassword, user.Password))
        {
            throw new BaseException(
                ErrorCode.BadRequest(),
                HttpStatusCode.BadRequest,
                "Wrong password"
            );
        }

        user.Password = _passwordEncoder.Encode(changePasswordDto.NewPassword);
        _repository.SaveChanges();
    }
}