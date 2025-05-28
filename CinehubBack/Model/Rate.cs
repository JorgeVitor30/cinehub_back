namespace CinehubBack.Model;

public class Rate : BaseEntity
{
    public required Guid UserId { get; set; }
    public required  Guid MovieId { get; set; }
    public required int RateValue { get; set; }
    public required string Comment { get; set; }
    
    public User User { get; set; }
    public Movie Movie { get; set; } 
    
}