using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BindingModel
{
    public class FavoriteBindingModel : IFavoriteModel
    {
        public int Id { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public int PublicationId { get; set; }
    }
}