using PublishingReviewDataModels.Enums;
using System;

namespace PublishingReviewContracts.SearchModels
{
    public class ReviewSearchModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public double? Rating { get; set; }
        public ReviewStatus? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public int? PublicationId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ConfirmedById { get; set; }

        // Для отчётов/фильтрации по периоду
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
