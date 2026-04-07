using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewDatabaseImplements.Models;
using PublishingReviewRestApi.Models.Dto;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PublicationsController : ControllerBase
{
    private readonly IPublicationLogic _publicationLogic;

    private readonly PublishingDatabase _db;
    public PublicationsController(IPublicationLogic publicationLogic, PublishingDatabase db)
    {
        _publicationLogic = publicationLogic;
        _db = db;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_publicationLogic.ReadList(null) ?? new());

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = id });
        return publication == null ? NotFound() : Ok(publication);
    }

    [HttpPost]
    public IActionResult Create([FromBody] PublicationCreateDto dto)
    {
        var model = new PublicationBindingModel
        {
            Title = dto.Title,
            SubjectId = dto.SubjectId,
            SubjectText = dto.SubjectText,
            PublishDate = dto.PublishDate ?? DateTime.UtcNow,
            Volume = dto.Volume,
            AuthorsText = dto.AuthorsText,
            Description = dto.Description,
            ResourcesRate = dto.ResourcesRate
        };

        _publicationLogic.Create(model);

        var created = _publicationLogic.ReadElement(new PublicationSearchModel { Title = dto.Title });
        return created == null
            ? Ok()
            : CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] PublicationCreateDto dto)
    {
        var model = new PublicationBindingModel
        {
            Id = id,
            Title = dto.Title,
            SubjectId = dto.SubjectId,
            SubjectText = dto.SubjectText,
            PublishDate = dto.PublishDate ?? DateTime.UtcNow,
            Volume = dto.Volume,
            AuthorsText = dto.AuthorsText,
            Description = dto.Description,
            ResourcesRate = dto.ResourcesRate
        };

        _publicationLogic.Update(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _publicationLogic.Delete(new PublicationBindingModel { Id = id });
        return NoContent();
    }

    [HttpPost("AddAuthors")]
    public async Task<IActionResult> AddAuthors([FromBody] AddAuthorsDto dto)
    {
        var publication = await _db.Publications.FindAsync(dto.PublicationId);
        if (publication == null) return NotFound("Publication not found");

        foreach (var userId in dto.UserIds.Distinct())
        {
            var exists = await _db.PublicationAuthors.AnyAsync(pa => pa.PublicationId == dto.PublicationId && pa.UserId == userId);
            if (!exists)
            {
                _db.PublicationAuthors.Add(new PublicationAuthor
                {
                    PublicationId = dto.PublicationId,
                    UserId = userId
                });
            }
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
