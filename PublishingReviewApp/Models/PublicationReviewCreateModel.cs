namespace PublishingReviewApp.Models;

public class PublicationReviewCreateModel
{
    public string Title { get; set; } = string.Empty;

    public string AuthorFullName { get; set; } = string.Empty;

    public string PublicationType { get; set; } = string.Empty;

    public string? EditorComment { get; set; }
}
