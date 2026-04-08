namespace PublishingReviewApp.Models;

public class ReviewerAssignmentModel
{
    public int TaskId { get; set; }

    public string ReviewerEmail { get; set; } = string.Empty;

    public DateTime? DeadlineUtc { get; set; }
}