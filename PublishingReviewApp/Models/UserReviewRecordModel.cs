namespace PublishingReviewApp.Models;

public record UserReviewRecordModel(
    int Id,
    string UserEmail,
    string PublicationTitle,
    string ReviewText,
    string? AttachmentFileName,
    DateTime CreatedUtc);