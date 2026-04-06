using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface ICommentModel : IId
    {
        int ReviewId { get; }
        int AuthorId { get; }
        string Content { get; }
        DateTime CreatedAt { get; }
    }
}