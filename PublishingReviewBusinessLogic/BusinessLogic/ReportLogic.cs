using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.SearchModels;

namespace PublishingReviewBusinessLogic.BusinessLogics
{
    public class ReportLogic : BaseLogic, IReportLogic
    {
        private readonly ILogger<ReportLogic> _logger;
        private readonly IReviewStorage _reviewStorage;
        private readonly IPublicationStorage _publicationStorage;

        public ReportLogic(ILogger<ReportLogic> logger, IReviewStorage reviewStorage, IPublicationStorage publicationStorage)
        {
            _logger = logger;
            _reviewStorage = reviewStorage;
            _publicationStorage = publicationStorage;
        }

        // Заглушка: возвращает текстовый отчёт в виде строки (позже заменить на PDF/Excel)
        public string GenerateTextReport(ReportBindingModel model)
        {
            EnsureNotNull(model, nameof(model));
            if (model.DateFrom > model.DateTo) throw new ArgumentException("DateFrom must be <= DateTo");

            var reviews = _reviewStorage.GetFilteredList(new ReviewSearchModel { CreatedAt = model.DateFrom });
            var pubs = _publicationStorage.GetFullList();

            var lines = new List<string>
            {
                $"Report from {model.DateFrom:yyyy-MM-dd} to {model.DateTo:yyyy-MM-dd}",
                $"Total reviews: {reviews?.Count ?? 0}",
                $"Total publications: {pubs?.Count ?? 0}"
            };

            return string.Join(Environment.NewLine, lines);
        }
    }
}
