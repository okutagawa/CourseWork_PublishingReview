namespace PublishingReviewApp.Models;

public class UserReviewUpsertModel
{
    public int Id { get; set; }

    public int PublicationId { get; set; }

    public string ReviewText { get; set; } = string.Empty;

    public IFormFile? Attachment { get; set; }
}