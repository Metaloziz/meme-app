using MemeApp.Api.Data;
using MemeApp.Api.Dtos;
using MemeApp.Api.Mapping;
using MemeApp.Api.Models;
using MemeApp.Api.Validation;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Services;

public class MemeService(AppDbContext context) : IMemeService
{
    public async Task<IReadOnlyList<MemeDto>> GetAllAsync(string? query, CancellationToken cancellationToken = default)
    {
        var memesQuery = context.Memes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var term = query.Trim().ToLower();
            memesQuery = memesQuery.Where(m =>
                m.Title.ToLower().Contains(term) ||
                m.Description.ToLower().Contains(term));
        }

        return await memesQuery
            .OrderByDescending(m => m.PopularityScore)
            .ThenBy(m => m.Title)
            .Select(m => new MemeDto(m.Id, m.Title, m.Description, m.Year, m.PopularityScore))
            .ToListAsync(cancellationToken);
    }

    public async Task<MemeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await context.Memes
            .Where(m => m.Id == id)
            .Select(m => new MemeDto(m.Id, m.Title, m.Description, m.Year, m.PopularityScore))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(byte[] Data, string ContentType)?> GetImageAsync(int id, CancellationToken cancellationToken = default)
    {
        var meme = await context.Memes
            .Where(m => m.Id == id)
            .Select(m => new { m.ImageData, m.ImageContentType })
            .FirstOrDefaultAsync(cancellationToken);

        if (meme?.ImageData is null || meme.ImageData.Length == 0)
        {
            return null;
        }

        return (meme.ImageData, meme.ImageContentType ?? "image/jpeg");
    }

    public async Task<(MemeDto? Meme, string? Error)> CreateAsync(
        CreateMemeRequest request,
        CancellationToken cancellationToken = default)
    {
        var textValidation = MemeImageValidator.ValidateTextFields(request.Title, request.Description);
        if (!textValidation.IsValid)
        {
            return (null, textValidation.ErrorMessage);
        }

        var imageValidation = MemeImageValidator.ValidateRequiredImage(request.Image);
        if (!imageValidation.IsValid)
        {
            return (null, imageValidation.ErrorMessage);
        }

        var (imageData, contentType) = await ReadImageAsync(request.Image, cancellationToken);

        var meme = new Meme
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ImageData = imageData,
            ImageContentType = contentType,
            Year = request.Year ?? DateTime.UtcNow.Year,
            PopularityScore = 50,
        };

        context.Memes.Add(meme);
        await context.SaveChangesAsync(cancellationToken);

        return (MemeMapper.ToDto(meme), null);
    }

    public async Task<(MemeDto? Meme, string? Error)> UpdateAsync(
        int id,
        UpdateMemeRequest request,
        CancellationToken cancellationToken = default)
    {
        var textValidation = MemeImageValidator.ValidateTextFields(request.Title, request.Description);
        if (!textValidation.IsValid)
        {
            return (null, textValidation.ErrorMessage);
        }

        var imageValidation = MemeImageValidator.ValidateOptionalImage(request.Image);
        if (!imageValidation.IsValid)
        {
            return (null, imageValidation.ErrorMessage);
        }

        var meme = await context.Memes.FindAsync([id], cancellationToken);
        if (meme is null)
        {
            return (null, null);
        }

        meme.Title = request.Title.Trim();
        meme.Description = request.Description.Trim();
        meme.Year = request.Year ?? meme.Year;

        if (request.Image is not null)
        {
            var (imageData, contentType) = await ReadImageAsync(request.Image, cancellationToken);
            meme.ImageData = imageData;
            meme.ImageContentType = contentType;
        }

        await context.SaveChangesAsync(cancellationToken);

        return (MemeMapper.ToDto(meme), null);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var meme = await context.Memes.FindAsync([id], cancellationToken);
        if (meme is null)
        {
            return false;
        }

        context.Memes.Remove(meme);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static async Task<(byte[] Data, string ContentType)> ReadImageAsync(
        IFormFile image,
        CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();
        await image.CopyToAsync(stream, cancellationToken);
        return (stream.ToArray(), image.ContentType);
    }
}
