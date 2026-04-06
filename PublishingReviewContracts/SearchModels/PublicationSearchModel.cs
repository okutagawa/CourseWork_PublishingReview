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
        public int? Id { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Authors { get; set; } = string.Empty;
        public string? Publisher { get; set; } = string.Empty;
        public DateTime? PublishDate { get; set; }
        public string? Description { get; set; } = string.Empty;
    }
}