using System;

namespace PublishingReviewBusinessLogic.ViewModels
{
    public class CommentModel
    {
        public int Id { get; set; }

        public int ReviewId { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
