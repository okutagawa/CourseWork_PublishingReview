using System;

namespace PublishingReviewRestApi.Models.Dto;

public class PublicationCreateDto
{
    public string Title { get; set; } = string.Empty;
    public int? SubjectId { get; set; }
    public string SubjectText { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public int Volume { get; set; }
    public string AuthorsText { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal ResourcesRate { get; set; }
}
