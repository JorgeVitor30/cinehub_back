using CinehubBack.Data.Movie;

public interface IImageUploadService
{
    ResponseUploadImgDto UploadImage(IFormFile posterPhoto, IFormFile backPhoto);
}