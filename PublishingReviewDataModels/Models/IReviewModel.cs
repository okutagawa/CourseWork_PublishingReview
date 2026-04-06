using PublishingReviewDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IReviewModel : IId
    {
        int PublicationId { get; }
        int ReviewerId { get; }
        string Title { get; }
        string Content { get; }
        double Rating { get; }
        ReviewStatus Status { get; }
        DateTime CreatedAt { get; }
        int? ConfirmedById { get; }
        DateTime? ConfirmedAt { get; }
    }
}