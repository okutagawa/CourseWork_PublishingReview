using Microsoft.AspNetCore.Mvc;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewRestApi.Models.Dto;

namespace PublishingReviewRestApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AttachmentsController : ControllerBase
{
    private readonly IAttachmentLogic _attachmentLogic;

    public AttachmentsController(IAttachmentLogic attachmentLogic)
    {
        _attachmentLogic = attachmentLogic;
    }

    [HttpGet("ByReview/{reviewId:int}")]
    public IActionResult GetByReview(int reviewId) => Ok(_attachmentLogic.ReadList(new AttachmentSearchModel { ReviewId = reviewId }) ?? new());

    [HttpPost]
    public IActionResult Create([FromBody] AttachmentCreateDto dto)
    {
        var model = new AttachmentBindingModel
        {
            ReviewId = dto.ReviewId,
            FileName = dto.FileName,
            MimeType = dto.MimeType,
            StoragePath = dto.StoragePath,
            SizeBytes = dto.SizeBytes,
            UploadedAt = DateTime.UtcNow
        };

        _attachmentLogic.Create(model);
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        _attachmentLogic.Delete(new AttachmentBindingModel { Id = id });
        return NoContent();
    }
}
