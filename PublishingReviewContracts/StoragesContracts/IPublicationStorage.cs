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
    public interface IPublicationStorage
    {
        List<PublicationViewModel> GetFullList();
        List<PublicationViewModel> GetFilteredList(PublicationSearchModel model);
        PublicationViewModel? GetElement(PublicationSearchModel model);
        PublicationViewModel? Insert(PublicationBindingModel model);
        PublicationViewModel? Update(PublicationBindingModel model);
        PublicationViewModel? Delete(PublicationBindingModel model);
    }
}
