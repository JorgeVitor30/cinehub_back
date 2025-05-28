using System;
using System.IO;
using System.Net;
using System.Text.Json;
using CinehubBack.Data.Movie;

namespace CinehubBack.Services.ImgBBService;

public class ImgBBImageUploadService : IImageUploadService
{
    private readonly string _apiKey;
    private readonly string _apiUrl = "https://api.imgbb.com/1/upload";
    private readonly HttpClient _httpClient;

    public ImgBBImageUploadService(IConfiguration configuration, HttpClient httpClient)
    {
        _apiKey = configuration["ImgBB:ApiKey"] 
            ?? throw new ArgumentNullException(nameof(configuration), "ImgBB:ApiKey não configurada.");
        _httpClient = httpClient;
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

        using var memoryStream = new MemoryStream();
        file.CopyTo(memoryStream);
        var imageBytes = memoryStream.ToArray();

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(imageBytes), "image", file.FileName);
        content.Add(new StringContent(_apiKey), "key");

        var response = _httpClient.PostAsync(_apiUrl, content).Result;
        var responseContent = response.Content.ReadAsStringAsync().Result;

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Erro ao fazer upload para ImgBB: {response.StatusCode} - {responseContent}");
        }

        using var jsonDoc = JsonDocument.Parse(responseContent);
        var imageUrl = jsonDoc.RootElement
            .GetProperty("data")
            .GetProperty("url")
            .GetString();

        if (string.IsNullOrEmpty(imageUrl))
            throw new Exception("Não foi possível obter a URL da imagem.");

        return imageUrl;
    }
}