using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class PublicationViewModel
    {
        public int Id { get; set; }

        [DisplayName("Заголовок")]
        public string Title { get; set; } = string.Empty;

        [DisplayName("Автор")]
        public string Authors { get; set; } = string.Empty;

        [DisplayName("Публикация")]
        public string Publisher { get; set; } = string.Empty;

        [DisplayName("Дата публикации")]
        public DateTime? PublishDate { get; set; }

        [DisplayName("Описание")]
        public string Description { get; set; } = string.Empty;
    }
}