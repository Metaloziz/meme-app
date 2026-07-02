using MemeApp.Api.Data;
using MemeApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MemeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemesController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Meme>>> GetAll([FromQuery] string? q)
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
            .ToListAsync();

        return Ok(memes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Meme>> GetById(int id)
    {
        var meme = await context.Memes.FindAsync(id);

        if (meme is null)
        {
            return NotFound();
        }

        return Ok(meme);
    }
}
