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
    public class FavoriteLogic : BaseLogic, IFavoriteLogic
    {
        private readonly ILogger<FavoriteLogic> _logger;
        private readonly IFavoriteStorage _storage;

        public FavoriteLogic(ILogger<FavoriteLogic> logger, IFavoriteStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public bool Create(FavoriteBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            if (model.UserId <= 0 || model.PublicationId <= 0) throw new ArgumentException("UserId and PublicationId are required");

            var res = _storage.Insert(model);
            return res != null;
        }

        public bool Update(FavoriteBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var existing = _storage.GetElement(new FavoriteSearchModel { Id = model.Id });
            if (existing == null) throw new InvalidOperationException("Favorite not found");

            var res = _storage.Update(model);
            return res != null;
        }

        public bool Delete(FavoriteBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            EnsureIdValid(model.Id);

            var res = _storage.Delete(model);
            return res != null;
        }

        public FavoriteViewModel? ReadElement(FavoriteSearchModel model)
        {
            EnsureNotNull(model, nameof(model));
            return _storage.GetElement(model);
        }

        public List<FavoriteViewModel>? ReadList(FavoriteSearchModel? model)
        {
            return model == null ? _storage.GetFullList() : _storage.GetFilteredList(model);
        }
    }
}
