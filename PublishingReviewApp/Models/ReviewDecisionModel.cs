namespace PublishingReviewApp.Models;

public class ReviewDecisionModel
{
    public int TaskId { get; set; }

    public ReviewWorkflowState Decision { get; set; }

    public string? Comment { get; set; }
}