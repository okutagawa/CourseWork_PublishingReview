using System;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class AttachmentModel
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }

        public string FileName { get; set; } = string.Empty;
        public long SizeBytes { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
