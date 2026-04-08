namespace PublishingReviewApp.Models;

public record ReviewCommentItemModel(
    int Id,
    int ReviewId,
    string AuthorName,
    string Text,
    DateTime CreatedUtc);