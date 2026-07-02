using MemeApp.Api.Dtos;

namespace MemeApp.Api.Services;

public interface IMemeService
{
    Task<IReadOnlyList<MemeDto>> GetAllAsync(string? query, CancellationToken cancellationToken = default);

    Task<MemeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<(byte[] Data, string ContentType)?> GetImageAsync(int id, CancellationToken cancellationToken = default);

    Task<(MemeDto? Meme, string? Error)> CreateAsync(CreateMemeRequest request, CancellationToken cancellationToken = default);

    Task<(MemeDto? Meme, string? Error)> UpdateAsync(int id, UpdateMemeRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
