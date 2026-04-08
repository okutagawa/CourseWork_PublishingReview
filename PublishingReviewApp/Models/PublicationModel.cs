namespace PublishingReviewApp.Models;

public class PublicationModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public DateTime CreatedAt { get; set; }
}