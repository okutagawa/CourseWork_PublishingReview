using PublishingReviewDataModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class ReviewViewModel
    {
        public int Id { get; set; }

        [DisplayName("Заголовок")]
        public string Title { get; set; } = string.Empty;

        [DisplayName("Контент")]
        public string Content { get; set; } = string.Empty;

        [DisplayName("Рейтинг")]
        public double Rating { get; set; }

        [DisplayName("Статус")]
        public ReviewStatus Status { get; set; }

        [DisplayName("Дата создания")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Дата одобрения")]
        public DateTime? ConfirmedAt { get; set; }

        public int PublicationId { get; set; }

        public int ReviewerId { get; set; }

        public int ConfirmedById { get; set; }
    }
}