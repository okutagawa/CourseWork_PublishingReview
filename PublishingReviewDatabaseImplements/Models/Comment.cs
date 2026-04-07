using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDatabaseImplements.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublishingReviewDatabase.Models
{
    public class Comment
    {
        [Required]
        public int ReviewId { get; private set; }

        [Required]
        public int UserId { get; private set; }

        [Required]
        public string Content { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public int Id { get; private set; }

        [ForeignKey("ReviewId")]
        public virtual Review Review { get; private set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; private set; } = null!;

        public static Comment? Create(CommentBindingModel model)
        {
            if (model == null) return null;
            return new Comment
            {
                ReviewId = model.ReviewId,
                UserId = model.UserId,
                Content = model.Content,
                CreatedAt = model.CreatedAt
            };
        }

        public void Update(CommentBindingModel model)
        {
            if (model == null) return;
            ReviewId = model.ReviewId;
            UserId = model.UserId;
            Content = model.Content;
        }

        public CommentBindingModel GetComment => new CommentBindingModel
        {
            Id = Id,
            ReviewId = ReviewId,
            AuthorId = UserId,
            Content = Content,
            CreatedAt = CreatedAt
        };

        public CommentViewModel GetCommentViewModel => new CommentViewModel
        {
            Id = Id,
            ReviewId = ReviewId,
            AuthorId = UserId,
            Content = Content,
            CreatedAt = CreatedAt
        };
    }
}
