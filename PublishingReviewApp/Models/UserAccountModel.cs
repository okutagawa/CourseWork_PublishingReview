namespace PublishingReviewApp.Models;

public record UserAccountModel(
    string FullName,
    string Email,
    string Password,
    string Role,
    string? Organization);