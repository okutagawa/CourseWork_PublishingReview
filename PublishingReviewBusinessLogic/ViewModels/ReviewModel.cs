using System;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class ReviewModel
    {
        public int Id { get; set; }

        public int PublicationId { get; set; }
        public string PublicationTitle { get; set; } = string.Empty;

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public int Rating { get; set; }

        public bool IsApproved { get; set; } = false;
        public int? ApprovedByEmployeeId { get; set; }
        public string? ApprovedByEmployeeName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
