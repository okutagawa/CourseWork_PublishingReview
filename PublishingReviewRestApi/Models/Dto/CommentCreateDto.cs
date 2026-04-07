namespace PublishingReviewRestApi.Models.Dto;

public class CommentCreateDto
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}
