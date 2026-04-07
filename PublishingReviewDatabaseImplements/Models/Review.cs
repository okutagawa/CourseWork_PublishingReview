using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDataModels.Enums;
using PublishingReviewDatabaseImplements.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublishingReviewDatabase.Models
{
    public class Review
    {
        [Required]
        public int PublicationId { get; private set; }

        [Required]
        public int UserId { get; private set; }

        [Required]
        public string Content { get; private set; } = string.Empty;

        public int Rating { get; private set; } = 0;

        public bool IsApproved { get; private set; } = false;

        public int? ApprovedByEmployeeId { get; private set; }

        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public int Id { get; private set; }

        [ForeignKey("PublicationId")]
        public virtual Publication Publication { get; private set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; private set; } = null!;

        public virtual List<Comment> Comments { get; private set; } = new();

        public virtual List<Attachment> Attachments { get; private set; } = new();

        [ForeignKey("ApprovedByEmployeeId")]
        public virtual Employee? ApprovedByEmployee { get; private set; }

        public static Review? Create(ReviewBindingModel model)
        {
            if (model == null) return null;
            return new Review
            {
                PublicationId = model.PublicationId,
                UserId = model.UserId,
                Content = model.Content,
                Rating = Convert.ToInt32(model.Rating),
                IsApproved = model.IsApproved,
                ApprovedByEmployeeId = model.ApprovedByEmployeeId,
                CreatedAt = model.CreatedAt
            };
        }

        public void Update(ReviewBindingModel model)
        {
            if (model == null) return;
            PublicationId = model.PublicationId;
            UserId = model.UserId;
            Content = model.Content;
            Rating = Convert.ToInt32(model.Rating);
            IsApproved = model.IsApproved;
            ApprovedByEmployeeId = model.ApprovedByEmployeeId;
        }

        public ReviewBindingModel GetReview => new ReviewBindingModel
        {
            Id = Id,
            Title = string.Empty,
            PublicationId = PublicationId,
            ReviewerId = UserId,
            Content = Content,
            Rating = Rating,
            Status = IsApproved ? ReviewStatus.Confirmed : ReviewStatus.Pending,
            ConfirmedById = ApprovedByEmployeeId,
            CreatedAt = CreatedAt
        };

        public ReviewViewModel GetReviewViewModel => new ReviewViewModel
        {
            Id = Id,
            Title = string.Empty,
            PublicationId = PublicationId,
            ReviewerId = UserId,
            Content = Content,
            Rating = Rating,
            Status = IsApproved ? ReviewStatus.Confirmed : ReviewStatus.Pending,
            ConfirmedById = ApprovedByEmployeeId ?? 0,
            CreatedAt = CreatedAt
        };
    }
}
