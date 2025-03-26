namespace CinehubBack.Data;

public class Page<T>
{
    public required ICollection<T> Content { get; set; }
    public required int Total { get; set; }
    public required int CurrentPage { get; set; }
    public required int Size { get; set; }
}