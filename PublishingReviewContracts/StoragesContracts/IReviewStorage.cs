using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.StoragesContracts
{
    public interface IReviewStorage
    {
        List<ReviewViewModel> GetFullList();
        List<ReviewViewModel> GetFilteredList(ReviewSearchModel model);
        ReviewViewModel? GetElement(ReviewSearchModel model);
        ReviewViewModel? Insert(ReviewBindingModel model);
        ReviewViewModel? Update(ReviewBindingModel model);
        ReviewViewModel? Delete(ReviewBindingModel model);
    }
}
