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
    public interface IAttachmentStorage
    {
        List<AttachmentViewModel> GetFullList();
        List<AttachmentViewModel> GetFilteredList(AttachmentSearchModel model);
        AttachmentViewModel? GetElement(AttachmentSearchModel model);
        AttachmentViewModel? Insert(AttachmentBindingModel model);
        AttachmentViewModel? Update(AttachmentBindingModel model);
        AttachmentViewModel? Delete(AttachmentBindingModel model);
    }
}
