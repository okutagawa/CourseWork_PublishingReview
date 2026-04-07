// PublishingReviewDatabase/Implements/EmployeeStorage.cs
using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class EmployeeStorage : IEmployeeStorage
    {
        public EmployeeBindingModel? Delete(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Employees.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Employees.Remove(element);
                context.SaveChanges();
                return element.GetEmployee;
            }
            return null;
        }

        public EmployeeBindingModel? GetElement(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Employees
                .FirstOrDefault(x => (model.Id != 0 && x.Id == model.Id) || (!string.IsNullOrEmpty(model.Email) && x.Email == model.Email))?
                .GetEmployee;
        }

        public List<EmployeeBindingModel> GetFilteredList(EmployeeBindingModel model)
        {
            using var context = new PublishingDatabase();
            return context.Employees
                .Select(x => x.GetEmployee)
                .ToList();
        }

        public List<EmployeeBindingModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Employees
                .Select(x => x.GetEmployee)
                .ToList();
        }

        public EmployeeBindingModel? Insert(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var newEmployee = Employee.Create(model);
            if (newEmployee == null) return null;
            context.Employees.Add(newEmployee);
            context.SaveChanges();
            return newEmployee.GetEmployee;
        }

        public EmployeeBindingModel? Update(EmployeeBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Employees.FirstOrDefault(x => x.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetEmployee;
        }
    }
}
