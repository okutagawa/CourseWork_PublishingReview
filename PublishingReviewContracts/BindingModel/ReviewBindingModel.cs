using PublishingReviewDataModels.Enums;
using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BindingModel
{
    public class ReviewBindingModel : IReviewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public double Rating { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeadlineUtc { get; set; }
        public DateTime? ConfirmedAt { get; set; } = DateTime.UtcNow;
        public int PublicationId { get; set; }
        public int ReviewerId { get; set; }
        public int? ConfirmedById { get; set; }

        // Совместимость со старым именованием
        public int UserId
        {
            get => ReviewerId;
            set => ReviewerId = value;
        }

        public bool IsApproved
        {
            get => Status == ReviewStatus.Confirmed;
            set => Status = value ? ReviewStatus.Confirmed : ReviewStatus.Pending;
        }

        public int? ApprovedByEmployeeId
        {
            get => ConfirmedById;
            set => ConfirmedById = value;
        }
    }
}