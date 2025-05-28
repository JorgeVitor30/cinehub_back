using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using CinehubBack.Model;

namespace CinehubBack.Data;

public class SeedingData
{
    private readonly IRepository<Model.Movie> _movieRepository;
    private readonly IMapper _mapper;

    public SeedingData(IRepository<Model.Movie> repository, IMapper mapper)
    {
        _movieRepository = repository;
        _mapper = mapper; 
    }
    
    public void Initialize()
    {
        SeedDataBase();
    }

    private void SeedDataBase()
    {
        if (_movieRepository.Raw(query => query.Any()))
        {
            return;
        }
        
        string filePath = Path.Combine(AppContext.BaseDirectory, "Utils", "Docs", "movies.json");
        string json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            AllowTrailingCommas = true
        };

        List<Model.Movie>? movies = JsonSerializer.Deserialize<List<Model.Movie>>(json, options);

        if (movies != null)
        {
            _movieRepository.CreateRange(movies);
            _movieRepository.SaveChanges();
            Console.WriteLine($"Foram adicionados {movies.Count} filmes ao banco de dados.");
        }
    }
    
}


