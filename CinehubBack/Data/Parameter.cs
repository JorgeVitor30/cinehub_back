namespace Api.Data.Dtos;

public class Parameter
{
    private int page;
    private int size;

    public int Page
    {
        get => page;
        init => page = value >= 0 ? value : 0;
    }
    public int Size
    {
        get => size;
        init => size = value > 0 ? value : 10;
    }
    public Dictionary<string, object?> Args { get; init; } = new Dictionary<string, object?>();

    public T? Get<T>(string key) => (T?) Args[key];
}