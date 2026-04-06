using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IPublicationModel : IId
    {
        string Title { get; }
        string Authors { get; }
        string Publisher { get; }
        DateTime PublishDate { get; }
        string Description { get; }
    }
}