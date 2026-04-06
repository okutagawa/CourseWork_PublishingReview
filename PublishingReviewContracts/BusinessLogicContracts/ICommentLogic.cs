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
    public interface ICommentLogic
    {
        List<CommentViewModel>? ReadList(CommentSearchModel? model);
        CommentViewModel? ReadElement(CommentSearchModel model);
        bool Create(CommentBindingModel model);
        bool Update(CommentBindingModel model);
        bool Delete(CommentBindingModel model);
    }
}
