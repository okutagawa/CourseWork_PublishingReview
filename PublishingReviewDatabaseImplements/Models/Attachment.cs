using PublishingReviewContracts.BindingModel;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublishingReviewDatabase.Models
{
    public class Attachment
    {
        [Required]
        public int ReviewId { get; private set; }

        [Required]
        public string FileName { get; private set; } = string.Empty;

        public long SizeBytes { get; private set; } = 0;

        public DateTime UploadedAt { get; private set; } = DateTime.UtcNow;

        public string? MimeType { get; private set; }

        public string? StoragePath { get; private set; }

        public int Id { get; private set; }

        [ForeignKey("ReviewId")]
        public virtual Review Review { get; private set; } = null!;

        public static Attachment? Create(AttachmentBindingModel model)
        {
            if (model == null) return null;
            return new Attachment
            {
                ReviewId = model.ReviewId,
                FileName = model.FileName,
                SizeBytes = model.SizeBytes,
                UploadedAt = model.UploadedAt ?? DateTime.UtcNow,
                MimeType = model.MimeType,
                StoragePath = model.StoragePath
            };
        }

        public void Update(AttachmentBindingModel model)
        {
            if (model == null) return;
            ReviewId = model.ReviewId;
            FileName = model.FileName;
            SizeBytes = model.SizeBytes;
            MimeType = model.MimeType;
            StoragePath = model.StoragePath;
        }

        public AttachmentBindingModel GetAttachment => new AttachmentBindingModel
        {
            Id = Id,
            ReviewId = ReviewId,
            FileName = FileName,
            SizeBytes = SizeBytes,
            UploadedAt = UploadedAt,
            MimeType = MimeType,
            StoragePath = StoragePath
        };
    }
}
