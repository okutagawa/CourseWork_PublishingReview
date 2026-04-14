using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class AttachmentViewModel : IAttachmentModel
    {
        public int Id { get; set; }

        [DisplayName("Название файла")]
        public string FileName { get; set; } = string.Empty;

        [DisplayName("Размер файла")]

        public byte[] FileData { get; set; } = new byte[0];

        [DisplayName("Дата создания")]

        public string MimeType { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        public string StoragePath { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int ReviewId { get; set; }
    }
}