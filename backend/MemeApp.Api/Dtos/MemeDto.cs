namespace MemeApp.Api.Dtos;

public record MemeDto(
    int Id,
    string Title,
    string Description,
    int Year,
    int PopularityScore);
