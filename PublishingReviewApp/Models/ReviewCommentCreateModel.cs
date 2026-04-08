namespace PublishingReviewApp.Models;

public class ReviewCommentCreateModel
{
    public int ReviewId { get; set; }

    public string Text { get; set; } = string.Empty;
}