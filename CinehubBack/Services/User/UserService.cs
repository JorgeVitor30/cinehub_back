using System.Net;
using AutoMapper;
using CinehubBack.Data;
using CinehubBack.Data.Dtos.User;
using CinehubBack.Encrypt;
using CinehubBack.Expections;
using CinehubBack.Model;
using Microsoft.EntityFrameworkCore;

namespace CinehubBack.Services.User;

public class UserService : IUserService
{

    private readonly IRepository<Model.User> _repository;
    private readonly IMapper _mapper;
    private readonly IPasswordEncoder _passwordEncoder;

    public UserService(IRepository<Model.User> repository, IMapper mapper, IPasswordEncoder passwordEncoder)
    {
        _repository = repository;
        _mapper = mapper;
        _passwordEncoder = passwordEncoder;
    }

    public ReadUserDto Create(CreateUserDto createUserDto)
    {
        var user = _mapper.Map<Model.User>(createUserDto);
        user.Role = Role.User;
        user.Password = _passwordEncoder.Encode(user.Password);

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
        return _repository.GetAll<ReadUserDto>(query => {
            var name = parameter.Get<string>("name");
            if (name != null)
                query = query.Where(u => EF.Functions.Like(u.Name, $"%{name}%"));

            return query
                .Select(u => new ReadUserDto { Id = u.Id, Email = u.Email, Name = u.Name });
        }, parameter);
    }

    public Model.User? GetByEmail(string email)
    {
        return _repository.Raw<Model.User?>(
            query => query.FirstOrDefault(u => u.Email.Equals(email)));
    }

    public ReadUserDto GetById(Guid id)
    {
        return _mapper.Map<ReadUserDto>(GetByIdOrThrow(id));
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

    public void SaveChanges()
    {
        _repository.SaveChanges();
    }
}