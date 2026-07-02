using MemeApp.Api.Dtos;
using MemeApp.Api.Models;

namespace MemeApp.Api.Mapping;

public static class MemeMapper
{
    public static MemeDto ToDto(Meme meme) =>
        new(meme.Id, meme.Title, meme.Description, meme.Year, meme.PopularityScore);
}
