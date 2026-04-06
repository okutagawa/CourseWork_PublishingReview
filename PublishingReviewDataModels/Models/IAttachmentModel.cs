using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IAttachmentModel : IId
    {
        int ReviewId { get; }
        string FileName { get; }
        byte[] FileData { get; }
        DateTime UploadedAt { get; }
    }
}