namespace PublishingReviewApp.Models;

public record ReviewTaskModel(
    int Id,
    int PublicationId,
    string Title,
    string AuthorFullName,
    string PublicationType,
    ReviewWorkflowState State,
    string? ReviewerEmail,
    DateTime? DeadlineUtc,
    string? EditorComment,
    DateTime CreatedUtc);