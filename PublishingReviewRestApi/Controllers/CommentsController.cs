using Microsoft.AspNetCore.Mvc;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewRestApi.Models.Dto;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CommentsController : ControllerBase
{
    private readonly ICommentLogic _commentLogic;

    public CommentsController(ICommentLogic commentLogic)
    {
        _commentLogic = commentLogic;
    }

    [HttpGet("ByReview/{reviewId:int}")]
    public IActionResult GetByReview(int reviewId) => Ok(_commentLogic.ReadList(new CommentSearchModel { ReviewId = reviewId }) ?? new());
    
    [HttpPost]
    public IActionResult Create([FromBody] CommentCreateDto dto)
    {
        var model = new CommentBindingModel
        {
            ReviewId = dto.ReviewId,
            AuthorId = dto.UserId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _commentLogic.Create(model);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _commentLogic.Delete(new CommentBindingModel { Id = id });
        return NoContent();
    }
}
