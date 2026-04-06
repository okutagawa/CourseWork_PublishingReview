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
    public interface IFavoriteStorage
    {
        List<FavoriteViewModel> GetFullList();
        List<FavoriteViewModel> GetFilteredList(FavoriteSearchModel model);
        FavoriteViewModel? GetElement(FavoriteSearchModel model);
        FavoriteViewModel? Insert(FavoriteBindingModel model);
        FavoriteViewModel? Update(FavoriteBindingModel model);
        FavoriteViewModel? Delete(FavoriteBindingModel model);
    }
}
