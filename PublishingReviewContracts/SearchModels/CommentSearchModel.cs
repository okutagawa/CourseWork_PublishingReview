using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.SearchModels
{
    public class CommentSearchModel
    {
        public int? Id { get; set; }
        public string? Content { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; }
        public int? ReviewId { get; set; }
        public int? AuthorId { get; set; }
    }
}
