namespace PublishingReviewRestApi.Models.Dto;

public class ReviewCreateDto
{
    public int PublicationId { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Rating { get; set; }
    public bool IsApproved { get; set; }
    public int? ApprovedByEmployeeId { get; set; }
}
