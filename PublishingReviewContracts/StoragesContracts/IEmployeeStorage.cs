using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.StoragesContracts
{
    public interface IEmployeeStorage
    {
        List<EmployeeViewModel> GetFullList();
        List<EmployeeViewModel> GetFilteredList(EmployeeSearchModel model);
        EmployeeViewModel? GetElement(EmployeeSearchModel model);
        EmployeeViewModel? Insert(EmployeeBindingModel model);
        EmployeeViewModel? Update(EmployeeBindingModel model);
        EmployeeViewModel? Delete(EmployeeBindingModel model);
    }
}
