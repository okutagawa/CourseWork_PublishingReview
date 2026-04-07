using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDataModels.Enums;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class ReviewLogic : BaseLogic, IReviewLogic
    {
        private readonly ILogger<ReviewLogic> _logger;
        private readonly IReviewStorage _storage;

        public ReviewLogic(ILogger<ReviewLogic> logger, IReviewStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public bool Create(ReviewBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            ValidateModel(model);

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(ReviewBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);
            ValidateModel(model);

            var existing = _storage.GetElement(new ReviewSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Review not found");

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(ReviewBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public ReviewViewModel? ReadElement(ReviewSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<ReviewViewModel>? ReadList(ReviewSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }

        private void ValidateModel(ReviewBindingModel model)
        {
            if (model.PublicationId <= 0) throw new ArgumentException("PublicationId is required");
            if (string.IsNullOrWhiteSpace(model.Content)) throw new ArgumentException("Content is required");
            if (model.Rating < 0 || model.Rating > 5) throw new ArgumentException("Rating must be between 0 and 5");
        }

        // Подтверждение рецензии сотрудником
        public bool ConfirmReview(int reviewId, int employeeId)
        {
            EnsureIdValid(reviewId);
            EnsureIdValid(employeeId);

            var review = _storage.GetElement(new ReviewSearchModel { Id = reviewId });
            if (review == null) throw new InvalidOperationException("Review not found");

            var binding = new ReviewBindingModel
            {
                Id = review.Id,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                Status = ReviewStatus.Confirmed,
                CreatedAt = review.CreatedAt,
                ConfirmedAt = DateTime.UtcNow,
                PublicationId = review.PublicationId,
                ReviewerId = review.ReviewerId,
                ConfirmedById = employeeId
            };

            var res = _storage.Update(binding);
            return res != null;
        }
    }
}
