namespace PublishingReviewRestApi.Models.Dto;

public class AttachmentCreateDto
{
    public int ReviewId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
}
