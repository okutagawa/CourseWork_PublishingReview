using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.ViewModels;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class PublicationLogic : BaseLogic, IPublicationLogic
    {
        private readonly ILogger<PublicationLogic> _logger;
        private readonly IPublicationStorage _storage;

        public PublicationLogic(ILogger<PublicationLogic> logger, IPublicationStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public bool Create(PublicationBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            ValidateModel(model);

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(PublicationBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);
            ValidateModel(model);

            var existing = _storage.GetElement(new PublicationSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Publication not found");

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(PublicationBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public PublicationViewModel? ReadElement(PublicationSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<PublicationViewModel>? ReadList(PublicationSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }

        private void ValidateModel(PublicationBindingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Title)) throw new ArgumentException("Title is required");
        }

        // Добавление рецензентов/авторов — безопасная реализация без dynamic
        public bool AddReviewersToPublication(int publicationId, int[] reviewerIds)
        {
            EnsureIdValid(publicationId);
            var element = _storage.GetElement(new PublicationSearchModel { Id = publicationId });
            if (element == null) throw new InvalidOperationException("Publication not found");

            // Создаём binding и копируем доступные поля из ViewModel
            var binding = new PublicationBindingModel
            {
                Id = element.Id,
                Title = element.Title,
                Authors = element.Authors,
                Publisher = element.Publisher,
                PublishDate = element.PublishDate,
                Description = element.Description,
                PublicationReviewers = new Dictionary<int, ReviewerBindingModel>()
            };

            // Добавляем новых рецензентов в словарь
            foreach (var id in reviewerIds)
            {
                if (!binding.PublicationReviewers.ContainsKey(id))
                    binding.PublicationReviewers.Add(id, new ReviewerBindingModel { Id = id });
            }

            var res = _storage.Update(binding);
            return res != null;
        }
    }
}
