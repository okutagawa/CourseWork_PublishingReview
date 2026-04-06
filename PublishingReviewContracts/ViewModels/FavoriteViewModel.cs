using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class FavoriteViewModel
    {
        public int Id { get; set; }

        [DisplayName("Дата добавления")]
        public DateTime AddedAt { get; set; }

        public int UserId { get; set; }

        public int PublicationId { get; set; }
    }
}