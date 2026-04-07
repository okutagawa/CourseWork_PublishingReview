using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Models;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public FavoritesController(PublishingDatabase db)
    {
        _db = db;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromQuery] int userId, [FromQuery] int publicationId)
    {
        if (!await _db.Users.AnyAsync(u => u.Id == userId) || !await _db.Publications.AnyAsync(p => p.Id == publicationId))
        {
            return NotFound("User or publication not found");
        }
        var exists = await _db.PublicationFavorites.AnyAsync(f => f.UserId == userId && f.PublicationId == publicationId);
        if (exists) return BadRequest("Already favorite");

        _db.PublicationFavorites.Add(new PublicationFavorite { UserId = userId, PublicationId = publicationId });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("Remove")]
    public async Task<IActionResult> Remove([FromQuery] int userId, [FromQuery] int publicationId)
    {
        var favorite = await _db.PublicationFavorites.FirstOrDefaultAsync(f => f.UserId == userId && f.PublicationId == publicationId);
        if (favorite == null) return NotFound();

        _db.PublicationFavorites.Remove(favorite);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("ByUser/{userId:int}")]
    public async Task<IActionResult> ByUser(int userId)
    {
        var list = await _db.PublicationFavorites.Where(f => f.UserId == userId).ToListAsync();
        return Ok(list);
    }
}
