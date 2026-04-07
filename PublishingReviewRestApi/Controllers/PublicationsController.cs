using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using PublishingReviewDatabaseImplements.Models;
using PublishingReviewRestApi.Models.Dto;

[Route("api/[controller]")]
[ApiController]
public class PublicationsController : ControllerBase
{
    private readonly PublishingDatabase _db;
    public PublicationsController(PublishingDatabase db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Publications.Include(p => p.Reviews).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var pub = await _db.Publications.Include(p => p.Reviews).FirstOrDefaultAsync(p => p.Id == id);
        if (pub == null) return NotFound();
        return Ok(pub);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PublicationCreateDto dto)
    {
        var pub = new Publication { Name = dto.Name, SubjectId = dto.SubjectId, Date = dto.Date, Volume = dto.Volume };
        _db.Publications.Add(pub);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = pub.Id }, pub);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PublicationCreateDto dto)
    {
        var pub = await _db.Publications.FindAsync(id);
        if (pub == null) return NotFound();
        pub.Name = dto.Name; pub.SubjectId = dto.SubjectId; pub.Date = dto.Date; pub.Volume = dto.Volume;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pub = await _db.Publications.FindAsync(id);
        if (pub == null) return NotFound();
        _db.Publications.Remove(pub);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("AddAuthors")]
    public async Task<IActionResult> AddAuthors([FromBody] AddAuthorsDto dto)
    {
        var pub = await _db.Publications.FindAsync(dto.PublicationId);
        if (pub == null) return NotFound("Publication not found");

        foreach (var userId in dto.UserIds)
        {
            var exists = await _db.PublicationAuthors.AnyAsync(pa => pa.PublicationId == dto.PublicationId && pa.UserId == userId);
            if (!exists) _db.PublicationAuthors.Add(new PublicationAuthor { PublicationId = dto.PublicationId, UserId = userId });
        }
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("RemoveAuthor")]
    public async Task<IActionResult> RemoveAuthor([FromQuery] int publicationId, [FromQuery] int userId)
    {
        var pa = await _db.PublicationAuthors.FirstOrDefaultAsync(x => x.PublicationId == publicationId && x.UserId == userId);
        if (pa == null) return NotFound();
        _db.PublicationAuthors.Remove(pa);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
