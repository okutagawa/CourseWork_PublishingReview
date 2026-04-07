// PublishingReviewDatabase/Implements/EmployeeStorage.cs
using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class EmployeeStorage : IEmployeeStorage
    {
        public EmployeeViewModel? Delete(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Employees.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Employees.Remove(element);
                context.SaveChanges();
                return element.GetEmployeeViewModel;
            }
            return null;
        }

        public EmployeeViewModel? GetElement(EmployeeSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Employees
                .FirstOrDefault(x => (model.Id != 0 && x.Id == model.Id) || (!string.IsNullOrEmpty(model.Email) && x.Email == model.Email))?
                .GetEmployeeViewModel;
        }

        public List<EmployeeViewModel> GetFilteredList(EmployeeSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Employees.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.FullName))
            {
                query = query.Where(x => x.FullName.Contains(model.FullName));
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                query = query.Where(x => x.Email.Contains(model.Email));
            }

            return query.Select(x => x.GetEmployeeViewModel).ToList();
        }

        public List<EmployeeViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Employees
                .Select(x => x.GetEmployeeViewModel)
                .ToList();
        }

        public EmployeeViewModel? Insert(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var newEmployee = Employee.Create(model);
            if (newEmployee == null) return null;
            context.Employees.Add(newEmployee);
            context.SaveChanges();
            return newEmployee.GetEmployeeViewModel;
        }

        public EmployeeViewModel? Update(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Employees.FirstOrDefault(x => x.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetEmployeeViewModel;
        }
    }
}
