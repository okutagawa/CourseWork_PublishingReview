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
    public interface IPublicationLogic
    {
        List<PublicationViewModel>? ReadList(PublicationSearchModel? model);
        PublicationViewModel? ReadElement(PublicationSearchModel model);
        bool Create(PublicationBindingModel model);
        bool Update(PublicationBindingModel model);
        bool Delete(PublicationBindingModel model);
    }
}
