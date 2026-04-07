using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using PublishingReviewRestApi.Models.Dto;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public ReviewsController(PublishingDatabase db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Reviews.ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        return Ok(r);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
    {
        var review = new Review { PublicationId = dto.PublicationId, UserId = dto.UserId, Text = dto.Text, Approved = dto.Approved };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = review.Id }, review);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReviewCreateDto dto)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();
        review.Text = dto.Text; review.Approved = dto.Approved;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();
        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
