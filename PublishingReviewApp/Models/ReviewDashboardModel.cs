namespace PublishingReviewApp.Models;

public class ReviewDashboardModel
{
    public int TotalPublications { get; set; }

    public int WaitingForReviewer { get; set; }

    public int InReview { get; set; }

    public int RequiresRevision { get; set; }

    public int Approved { get; set; }

    public int Rejected { get; set; }

    public List<ReviewTaskModel> NearestDeadlines { get; set; } = [];
}
