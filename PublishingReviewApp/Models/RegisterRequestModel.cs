namespace PublishingReviewApp.Models;

public class RegisterRequestModel
{
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string? Organization { get; set; }
}