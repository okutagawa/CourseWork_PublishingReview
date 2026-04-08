namespace PublishingReviewApp.Models;

public enum ReviewWorkflowState
{
    WaitingForReviewer = 0,
    InReview = 1,
    RequiresRevision = 2,
    Approved = 3,
    Rejected = 4
}