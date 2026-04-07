using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using PublishingReviewRestApi.Models.Dto;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public CommentsController(PublishingDatabase db) => _db = db;

    [HttpGet("ByReview/{reviewId:int}")]
    public async Task<IActionResult> GetByReview(int reviewId) => Ok(await _db.Comments.Where(c => c.ReviewId == reviewId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CommentCreateDto dto)
    {
        var comment = new Comment { ReviewId = dto.ReviewId, UserId = dto.UserId, Text = dto.Text };
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByReview), new { reviewId = dto.ReviewId }, comment);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Comments.FindAsync(id);
        if (c == null) return NotFound();
        _db.Comments.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
