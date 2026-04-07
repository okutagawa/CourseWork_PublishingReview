using PublishingReviewContracts.BindingModel;
using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.SearchModels
{
    public class PublicationSearchModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Authors { get; set; } = string.Empty;
        public string Publisher { get; set; } = string.Empty;
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = string.Empty;

        // Словарь рецензентов/авторов: ключ — Id пользователя, значение — краткая модель рецензента
        public Dictionary<int, ReviewerBindingModel> PublicationReviewers { get; set; } = new();
    }

    public class ReviewerSearchModel
    {
        public int Id { get; set; }
        public string Note { get; set; }
    }
}