using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.ViewModels;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class CommentLogic : BaseLogic, ICommentLogic
    {
        private readonly ILogger<CommentLogic> _logger;
        private readonly ICommentStorage _storage;

        public CommentLogic(ILogger<CommentLogic> logger, ICommentStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public bool Create(CommentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            if (model.ReviewId <= 0) throw new ArgumentException("ReviewId is required");
            if (string.IsNullOrWhiteSpace(model.Content)) throw new ArgumentException("Content is required");

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(CommentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var existing = _storage.GetElement(new CommentSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Comment not found");

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(CommentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public CommentViewModel? ReadElement(CommentSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<CommentViewModel>? ReadList(CommentSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }
    }
}
