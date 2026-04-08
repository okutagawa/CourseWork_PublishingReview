namespace PublishingReviewApp.Models;

public class ApiSettings
{
    public const string SectionName = "ApiSettings";

    public string BaseUrl { get; set; } = string.Empty;
    public int RequestTimeoutSeconds { get; set; } = 30;
}