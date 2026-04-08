namespace PublishingReviewApp.Models;

public record PublicationCatalogItemModel(
    int Id,
    string Title,
    string AuthorFullName,
    string PublicationType,
    string ReviewSummary);