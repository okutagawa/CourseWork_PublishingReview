using System;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class PublicationModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime? PublishDate { get; set; }
        public string Subject { get; set; } = string.Empty;
        public decimal ResourcesRate { get; set; }
        public int Volume { get; set; }
        public string? Description { get; set; }
    }
}
