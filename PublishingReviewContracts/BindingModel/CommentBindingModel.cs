using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BindingModel
{
    public class CommentBindingModel : ICommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int ReviewId { get; set; }
        public int AuthorId { get; set; }

        // Совместимость со старым именованием полей
        public int UserId
        {
            get => AuthorId;
            set => AuthorId = value;
        }
    }
}