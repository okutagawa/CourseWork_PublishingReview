namespace PublishingReviewApp.Models;

public enum ReviewState
{
    WaitingForReviewer = 0,
    InReview = 1,
    RequiresRevision = 2,
    Approved = 3,
    Rejected = 4
}

public record ReviewerTaskModel(
    int Id,
    int PublicationId,
    string Title,
    string AuthorFullName,
    string PublicationType,
    ReviewState State,
    string? ReviewerEmail,
    DateTime? DeadlineUtc,
    string? EditorComment);

public class DashboardViewModel
{
    public int TotalPublications { get; set; }

    public int WaitingForReviewer { get; set; }

    public int InReview { get; set; }

    public int RequiresRevision { get; set; }

    public int Approved { get; set; }

    public int Rejected { get; set; }

    public List<ReviewerTaskModel> NearestDeadlines { get; set; } = [];
}

public class CreateReviewPublicationRequest
{
    public string Title { get; set; } = string.Empty;

    public string AuthorFullName { get; set; } = string.Empty;

    public string PublicationType { get; set; } = string.Empty;

    public string? EditorComment { get; set; }
}

public class AssignReviewerRequest
{
    public int TaskId { get; set; }

    public string ReviewerEmail { get; set; } = string.Empty;

    public DateTime? DeadlineUtc { get; set; }
}

public class SubmitReviewRequest
{
    public int TaskId { get; set; }

    public ReviewState Decision { get; set; }

    public string? Comment { get; set; }
}
