using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BindingModel
{
    public class AttachmentBindingModel : IAttachmentModel
    {
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public byte[] FileData { get; set; } = new byte[0];
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int ReviewId { get; set; }
    }
}
