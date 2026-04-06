using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.SearchModels
{
    public class FavoriteSearchModel
    {
        public int? Id { get; set; }
        public DateTime? AddedAt { get; set; }
        public int? UserId { get; set; }
        public int? PublicationId { get; set; }
    }
}