using System.ComponentModel.DataAnnotations;

namespace MemeApp.Api.Dtos;

public class UpdateMemeRequest
{
    [MaxLength(120)]
    public required string Title { get; set; }

    [MaxLength(500)]
    public required string Description { get; set; }

    public int? Year { get; set; }

    public IFormFile? Image { get; set; }
}
