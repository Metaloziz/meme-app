using MemeApp.Api.Constants;
using MemeApp.Api.Dtos;
using MemeApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemeApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MemesController(IMemeService memeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemeDto>>> GetAll([FromQuery] string? q, CancellationToken cancellationToken)
    {
        var memes = await memeService.GetAllAsync(q, cancellationToken);
        return Ok(memes);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MemeDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var meme = await memeService.GetByIdAsync(id, cancellationToken);
        if (meme is null)
        {
            return NotFound();
        }

        return Ok(meme);
    }

    [HttpGet("{id:int}/image")]
    public async Task<IActionResult> GetImage(int id, CancellationToken cancellationToken)
    {
        var image = await memeService.GetImageAsync(id, cancellationToken);
        if (image is null)
        {
            return NotFound();
        }

        return File(image.Value.Data, image.Value.ContentType);
    }

    [Authorize]
    [HttpPost]
    [RequestSizeLimit(MemeConstants.MaxImageBytes)]
    public async Task<ActionResult<MemeDto>> Create([FromForm] CreateMemeRequest request, CancellationToken cancellationToken)
    {
        var (meme, error) = await memeService.CreateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new ErrorResponse(error));
        }

        return CreatedAtAction(nameof(GetById), new { id = meme!.Id }, meme);
    }

    [Authorize]
    [HttpPut("{id:int}")]
    [RequestSizeLimit(MemeConstants.MaxImageBytes)]
    public async Task<ActionResult<MemeDto>> Update(int id, [FromForm] UpdateMemeRequest request, CancellationToken cancellationToken)
    {
        var (meme, error) = await memeService.UpdateAsync(id, request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(new ErrorResponse(error));
        }

        if (meme is null)
        {
            return NotFound();
        }

        return Ok(meme);
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await memeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
