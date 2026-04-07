using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;

namespace PublishingReviewContracts.BindingModel
{
    public class PublicationBindingModel : IPublicationModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string AuthorsText
        {
            get => Authors;
            set => Authors = value;
        }
        public string Publisher { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;
        public decimal ResourcesRate { get; set; }
        public int Volume { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectText { get; set; } = string.Empty;

        public Dictionary<int, ReviewerBindingModel> PublicationAuthors
        {
            get => PublicationReviewers;
            set => PublicationReviewers = value ?? new();
        }

        // Словарь рецензентов/авторов: ключ — Id пользователя, значение — краткая модель рецензента
        public Dictionary<int, ReviewerBindingModel> PublicationReviewers { get; set; } = new();
    }

    public class ReviewerBindingModel
    {
        public int Id { get; set; }
        public string? Note { get; set; }
    }
}
