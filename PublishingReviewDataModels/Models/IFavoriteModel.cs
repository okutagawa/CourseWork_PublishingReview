using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IFavoriteModel : IId
    {
        int UserId { get; }
        int PublicationId { get; }
        DateTime AddedAt { get; }
    }
}