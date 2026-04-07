using Microsoft.AspNetCore.Mvc;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewRestApi.Models.Dto;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewsController : ControllerBase
{

    private readonly IReviewLogic _reviewLogic;

    public ReviewsController(IReviewLogic reviewLogic)
    {
        _reviewLogic = reviewLogic;
    }

    [HttpGet]
    public IActionResult GetAll() => Ok(_reviewLogic.ReadList(null) ?? new());

    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        var review = _reviewLogic.ReadElement(new ReviewSearchModel { Id = id });
        return review == null ? NotFound() : Ok(review);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ReviewCreateDto dto)
    {
        var model = new ReviewBindingModel
        {
            PublicationId = dto.PublicationId,
            ReviewerId = dto.UserId,
            Content = dto.Content,
            Rating = dto.Rating,
            IsApproved = dto.IsApproved,
            ApprovedByEmployeeId = dto.ApprovedByEmployeeId,
            CreatedAt = DateTime.UtcNow
        };

        _reviewLogic.Create(model);
        return Ok();
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] ReviewCreateDto dto)
    {
        var model = new ReviewBindingModel
        {
            Id = id,
            PublicationId = dto.PublicationId,
            ReviewerId = dto.UserId,
            Content = dto.Content,
            Rating = dto.Rating,
            IsApproved = dto.IsApproved,
            ApprovedByEmployeeId = dto.ApprovedByEmployeeId
        };

        _reviewLogic.Update(model);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _reviewLogic.Delete(new ReviewBindingModel { Id = id });
        return NoContent();
    }
}
