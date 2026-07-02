namespace MemeApp.Api.Dtos;

public class UpdateMemeRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public int? Year { get; set; }
    public IFormFile? Image { get; set; }
}
