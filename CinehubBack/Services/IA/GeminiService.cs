using System.Text;
using System.Text.Json;
using CinehubBack.Services.IA;
using Microsoft.Extensions.Options;

public class GeminiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private const string Model = "gemini-1.5-flash";

    public GeminiService(IOptions<GeminiApiSettings> options)
    {
        _apiKey = options.Value.Key;
        _httpClient = new HttpClient();
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={_apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var responseText = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(responseText);

        var result = doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString();

        return result ?? "(sem resposta)";
    }
}