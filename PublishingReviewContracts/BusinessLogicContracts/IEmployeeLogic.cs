using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.StoragesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.BusinessLogicContracts
{
    public interface IEmployeeLogic
    {
        List<EmployeeViewModel>? ReadList(EmployeeSearchModel? model);
        EmployeeViewModel? ReadElement(EmployeeSearchModel model);
        bool Create(EmployeeBindingModel model);
        bool Update(EmployeeBindingModel model);
        bool Delete(EmployeeBindingModel model);
    }
}
