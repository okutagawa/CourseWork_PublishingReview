using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewRestApi.Models.Dto;

[Route("api/[controller]")]
[ApiController]
public class AttachmentsController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public AttachmentsController(PublishingDatabase db) => _db = db;

    [HttpGet("ByReview/{reviewId:int}")]
    public async Task<IActionResult> GetByReview(int reviewId) => Ok(await _db.Attachments.Where(a => a.ReviewId == reviewId).ToListAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AttachmentCreateDto dto)
    {
        var att = new Attachment { ReviewId = dto.ReviewId, FileName = dto.FileName, FilePath = dto.FilePath };
        _db.Attachments.Add(att);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetByReview), new { reviewId = dto.ReviewId }, att);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Attachments.FindAsync(id);
        if (a == null) return NotFound();
        _db.Attachments.Remove(a);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
