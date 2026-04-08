namespace PublishingReviewApp.Models;

public class PublicationListViewModel
{
    public string ProjectName { get; set; } = "Publishing Review";

    public List<PublicationModel> Publications { get; set; } = [];
}