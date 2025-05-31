using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CinehubBack.Data.Movie;

namespace CinehubBack.Services.ImageUploadService;

public class CloudinaryImageUploadService : IImageUploadService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryImageUploadService(IConfiguration configuration)
    {
        var account = new Account(
            configuration["Cloudinary:CloudName"] ?? throw new ArgumentNullException(nameof(configuration), "Cloudinary:CloudName não configurada."),
            configuration["Cloudinary:ApiKey"] ?? throw new ArgumentNullException(nameof(configuration), "Cloudinary:ApiKey não configurada."),
            configuration["Cloudinary:ApiSecret"] ?? throw new ArgumentNullException(nameof(configuration), "Cloudinary:ApiSecret não configurada.")
        );
        _cloudinary = new Cloudinary(account);
    }

    public ResponseUploadImgDto UploadImage(IFormFile posterPhoto, IFormFile backPhoto)
    {
        if (posterPhoto == null || backPhoto == null)
            throw new ArgumentNullException("Os arquivos de imagem não podem ser nulos.");

        var posterUrl = UploadSingleImage(posterPhoto);
        var backUrl = UploadSingleImage(backPhoto);

        return new ResponseUploadImgDto
        {
            PosterPhotoUrl = posterUrl,
            BackPhotoUrl = backUrl
        };
    }

    private string UploadSingleImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("O arquivo de imagem é inválido.", nameof(file));

        using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        var uploadResult = _cloudinary.Upload(uploadParams);

        if (string.IsNullOrEmpty(uploadResult.SecureUrl.AbsoluteUri))
            throw new Exception("Não foi possível obter a URL da imagem.");

        return uploadResult.SecureUrl.AbsoluteUri;
    }
} 