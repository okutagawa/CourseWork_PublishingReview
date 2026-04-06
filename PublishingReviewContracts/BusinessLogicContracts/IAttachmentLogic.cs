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
    public interface IAttachmentLogic
    {
        List<AttachmentViewModel>? ReadList(AttachmentSearchModel? model);
        AttachmentViewModel? ReadElement(AttachmentSearchModel model);
        bool Create(AttachmentBindingModel model);
        bool Update(AttachmentBindingModel model);
        bool Delete(AttachmentBindingModel model);
    }
}