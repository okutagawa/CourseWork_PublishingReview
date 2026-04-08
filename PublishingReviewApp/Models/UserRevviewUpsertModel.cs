namespace PublishingReviewApp.Models;

public class UserReviewUpsertModel
{
    public int Id { get; set; }

    public string PublicationTitle { get; set; } = string.Empty;

    public string ReviewText { get; set; } = string.Empty;

    public string? AttachmentFileName { get; set; }
}