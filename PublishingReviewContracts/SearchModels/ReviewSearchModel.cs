using PublishingReviewDataModels.Enums;
using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.SearchModels
{
    public class ReviewSearchModel
    {
        public int? Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Content { get; set; } = string.Empty;
        public double? Rating { get; set; }
        public ReviewStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public int? PublicationId { get; set; }
        public int? ReviewerId { get; set; }
        public int? ConfirmedById { get; set; }
    }
}