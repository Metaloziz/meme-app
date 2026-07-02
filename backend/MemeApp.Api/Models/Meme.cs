namespace MemeApp.Api.Models;

public class Meme
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
    public byte[]? ImageData { get; set; }
    public string? ImageContentType { get; set; }
    public int Year { get; set; }
    public int PopularityScore { get; set; }
}
