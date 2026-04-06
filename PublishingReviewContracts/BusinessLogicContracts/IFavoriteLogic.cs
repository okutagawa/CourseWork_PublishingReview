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
    public interface IFavoriteLogic
    {
        List<FavoriteViewModel>? ReadList(FavoriteSearchModel? model);
        FavoriteViewModel? ReadElement(FavoriteSearchModel model);
        bool Create(FavoriteBindingModel model);
        bool Update(FavoriteBindingModel model);
        bool Delete(FavoriteBindingModel model);
    }
}
