namespace MemeApp.Api.Dtos;

public class CreateMemeRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int? Year { get; set; }
    public required IFormFile Image { get; set; }
}
