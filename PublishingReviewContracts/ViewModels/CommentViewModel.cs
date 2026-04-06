using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class CommentViewModel : ICommentModel
    {
        public int Id { get; set; }

        [DisplayName("Контент")]

        public string Content { get; set; } = string.Empty;

        [DisplayName("Дата создания")]

        public DateTime CreatedAt { get; set; }

        public int ReviewId { get; set; }

        public int AuthorId { get; set; }
    }
}