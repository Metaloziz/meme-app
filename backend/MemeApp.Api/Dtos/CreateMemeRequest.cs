using System.ComponentModel.DataAnnotations;

namespace MemeApp.Api.Dtos;

public class CreateMemeRequest
{
    [MaxLength(120)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    public int? Year { get; set; }

    public required IFormFile Image { get; set; }
}
