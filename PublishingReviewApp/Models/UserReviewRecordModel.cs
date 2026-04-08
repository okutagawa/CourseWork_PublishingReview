namespace PublishingReviewApp.Models;

public record UserReviewRecordModel(
    int Id,
    string UserEmail,
    int PublicationId,
    string PublicationTitle,
    string ReviewText,
    string? AttachmentFileName,
    DateTime CreatedUtc);