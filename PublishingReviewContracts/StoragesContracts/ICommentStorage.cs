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
    public interface ICommentStorage
    {
        List<CommentViewModel> GetFullList();
        List<CommentViewModel> GetFilteredList(CommentSearchModel model);
        CommentViewModel? GetElement(CommentSearchModel model);
        CommentViewModel? Insert(CommentBindingModel model);
        CommentViewModel? Update(CommentBindingModel model);
        CommentViewModel? Delete(CommentBindingModel model);
    }
}
