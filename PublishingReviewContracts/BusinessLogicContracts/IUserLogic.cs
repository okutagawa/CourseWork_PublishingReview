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
    public interface IUserLogic
    {
        List<UserViewModel>? ReadList(UserSearchModel? model);
        UserViewModel? ReadElement(UserSearchModel model);
        bool Create(UserBindingModel model);
        bool Update(UserBindingModel model);
        bool Delete(UserBindingModel model);
    }
}