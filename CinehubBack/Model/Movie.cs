using System.Text.Json.Serialization;

namespace CinehubBack.Model
{
    public class Movie : BaseEntity
    {
        [JsonPropertyName("title")] 
        public required string Title { get; set; }

        [JsonPropertyName("overview")] 
        public required string Overview { get; set; }

        [JsonPropertyName("vote_count")]  
        public required int VoteCount { get; set; } = 0;

        [JsonPropertyName("vote_average")] 
        public required decimal VoteAverage { get; set; } = 0;

        [JsonPropertyName("release_date")]  
        public required DateTime ReleaseDate { get; set; }

        [JsonPropertyName("revenue")]  
        public required decimal Revenue { get; set; }

        [JsonPropertyName("runtime")]  
        public required int RunTime { get; set; }

        [JsonPropertyName("adult")]  
        public required bool Adult { get; set; }

        [JsonPropertyName("budget")]  
        public required decimal Budget { get; set; }

        [JsonPropertyName("poster_path")] public string PosterPhotoUrl { get; set; } = string.Empty;

        [JsonPropertyName("backdrop_path")] public string BackPhotoUrl { get; set; } = string.Empty;

        [JsonPropertyName("original_language")]  
        public required string OriginalLanguage { get; set; }

        [JsonPropertyName("popularity")] 
        public required decimal Popularity { get; set; } = 0;

        [JsonPropertyName("tagline")]  
        public required string Tagline { get; set; }

        [JsonPropertyName("keywords")]  
        public required string KeyWords { get; set; }
        
        [JsonPropertyName("production_companies")]
        public required string Productions { get; set; }
        
        [JsonPropertyName("genres")]
        public required string Genres { get; set; }
    }
}
