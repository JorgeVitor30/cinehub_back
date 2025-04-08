using CinehubBack.Data;
using CinehubBack.Data.Dtos.User;

namespace CinehubBack.Services.User;

public interface IUserService
{
    ReadUserDto Create(CreateUserDto createUserDto);
    Page<ReadUserDto> GetAll(Parameter parameter);
    void Delete(Guid id);
    ReadUserDto GetById(Guid id);
    Model.User? GetByEmail(string email);
    void Update(Guid id, UpdateUserDto updateUserDto);
}