using CinehubBack.Data;
using CinehubBack.Data.Dtos.User;

namespace CinehubBack.Services.User;

public interface IUserService
{
    ReadUserDto Create(CreateUserDto createUserDto);
    Page<ReadUserDto> GetAll(Parameter parameter);
    void Delete(Guid id);
    ReadUserByIdDto GetById(Guid id);
    Model.User? GetByEmail(string email);
    void Update(Guid id, UpdateUserDto updateUserDto);
    void UploadPhoto(Guid id, IFormFile file);
    void ChangePassword(Guid id, ChangePasswordDto changePasswordDto);
}