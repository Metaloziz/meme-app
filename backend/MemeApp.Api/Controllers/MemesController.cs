using MemeApp.Api.Data;
using MemeApp.Api.Dtos;
using MemeApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemesController(AppDbContext context) : ControllerBase
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
    };

    private const long MaxImageBytes = 2 * 1024 * 1024;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemeDto>>> GetAll([FromQuery] string? q)
    {
        var query = context.Memes.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(term) ||
                m.Description.ToLower().Contains(term));
        }

        var memes = await query
            .OrderByDescending(m => m.PopularityScore)
            .ThenBy(m => m.Title)
            .Select(m => new MemeDto(m.Id, m.Title, m.Description, m.Year, m.PopularityScore))
            .ToListAsync();

        return Ok(memes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemeDto>> GetById(int id)
    {
        var meme = await context.Memes
            .Where(m => m.Id == id)
            .Select(m => new MemeDto(m.Id, m.Title, m.Description, m.Year, m.PopularityScore))
            .FirstOrDefaultAsync();

        if (meme is null)
        {
            return NotFound();
        }

        return Ok(meme);
    }

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetImage(int id)
    {
        var meme = await context.Memes
            .Where(m => m.Id == id)
            .Select(m => new { m.ImageData, m.ImageContentType })
            .FirstOrDefaultAsync();

        if (meme?.ImageData is null || meme.ImageData.Length == 0)
        {
            return NotFound();
        }

        return File(meme.ImageData, meme.ImageContentType ?? "image/jpeg");
    }

    [Authorize]
    [HttpPost]
    [RequestSizeLimit(MaxImageBytes)]
    public async Task<ActionResult<MemeDto>> Create([FromForm] CreateMemeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest(new { message = "Название и описание обязательны." });
        }

        if (request.Image.Length == 0)
        {
            return BadRequest(new { message = "Изображение обязательно." });
        }

        if (request.Image.Length > MaxImageBytes)
        {
            return BadRequest(new { message = "Максимальный размер изображения — 2 MB." });
        }

        if (!AllowedContentTypes.Contains(request.Image.ContentType))
        {
            return BadRequest(new { message = "Допустимые форматы: JPEG, PNG, WebP." });
        }

        await using var stream = new MemoryStream();
        await request.Image.CopyToAsync(stream);

        var meme = new Meme
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ImageData = stream.ToArray(),
            ImageContentType = request.Image.ContentType,
            Year = request.Year ?? DateTime.UtcNow.Year,
            PopularityScore = 50,
        };

        context.Memes.Add(meme);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = meme.Id }, new MemeDto(
            meme.Id,
            meme.Title,
            meme.Description,
            meme.Year,
            meme.PopularityScore));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var meme = await context.Memes.FindAsync(id);
        if (meme is null)
        {
            return NotFound();
        }

        context.Memes.Remove(meme);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
