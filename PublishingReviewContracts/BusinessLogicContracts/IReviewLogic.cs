using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BusinessLogicContracts
{
    public interface IReviewLogic
    {
        List<ReviewViewModel>? ReadList(ReviewSearchModel? model);
        ReviewViewModel? ReadElement(ReviewSearchModel model);
        bool Create(ReviewBindingModel model);
        bool Update(ReviewBindingModel model);
        bool Delete(ReviewBindingModel model);
    }
}
