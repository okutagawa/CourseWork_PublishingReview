using System;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class PublicationReportItemModel
    {
        public int Id { get; set; }
        public string Authors { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;

        public decimal ResourcesRate { get; set; }
        public int Volume { get; set; }
        public DateTime? Date { get; set; }

        public int ReviewsCount { get; set; }
    }
}
