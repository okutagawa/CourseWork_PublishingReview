using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Models;

[Route("api/[controller]")]
[ApiController]
public class FavoritesController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public FavoritesController(PublishingDatabase db) => _db = db;

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromQuery] int userId, [FromQuery] int publicationId)
    {
        var exists = await _db.PublicationFavorites.AnyAsync(f => f.UserId == userId && f.PublicationId == publicationId);
        if (exists) return BadRequest("Already favorite");
        _db.PublicationFavorites.Add(new PublicationFavorite { UserId = userId, PublicationId = publicationId });
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("Remove")]
    public async Task<IActionResult> Remove([FromQuery] int userId, [FromQuery] int publicationId)
    {
        var fav = await _db.PublicationFavorites.FirstOrDefaultAsync(f => f.UserId == userId && f.PublicationId == publicationId);
        if (fav == null) return NotFound();
        _db.PublicationFavorites.Remove(fav);
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
