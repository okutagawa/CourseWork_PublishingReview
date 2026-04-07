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
    public class AttachmentLogic : BaseLogic, IAttachmentLogic
    {
        private readonly ILogger<AttachmentLogic> _logger;
        private readonly IAttachmentStorage _storage;

        public AttachmentLogic(ILogger<AttachmentLogic> logger, IAttachmentStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public bool Create(AttachmentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            if (model.ReviewId <= 0) throw new ArgumentException("ReviewId is required");
            if (string.IsNullOrWhiteSpace(model.FileName)) throw new ArgumentException("FileName is required");

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(AttachmentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var existing = _storage.GetElement(new AttachmentSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Attachment not found");

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(AttachmentBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public AttachmentViewModel? ReadElement(AttachmentSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<AttachmentViewModel>? ReadList(AttachmentSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }
    }
}
