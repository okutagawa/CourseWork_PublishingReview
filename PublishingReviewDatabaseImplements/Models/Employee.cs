using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace PublishingReviewDatabase.Models
{
    public class Employee
    {
        [Required]
        public string FullName { get; private set; } = string.Empty;

        [Required]
        public string Position { get; private set; } = "Editor";

        [Required]
        public string Email { get; private set; } = string.Empty;

        public int Id { get; private set; }

        public static Employee? Create(EmployeeBindingModel model)
        {
            if (model == null) return null;
            return new Employee
            {
                FullName = model.FullName,
                Position = model.Position ?? "Editor",
                Email = model.Email
            };
        }

        public void Update(EmployeeBindingModel model)
        {
            if (model == null) return;
            FullName = model.FullName;
            Position = model.Position ?? Position;
            Email = model.Email;
        }

        public EmployeeBindingModel GetEmployee => new EmployeeBindingModel
        {
            Id = Id,
            FullName = FullName,
            Login = string.Empty,
            Password = string.Empty,
            Phone = string.Empty,
            Position = Position,
            Email = Email
        };

        public EmployeeViewModel GetEmployeeViewModel => new EmployeeViewModel
        {
            Id = Id,
            FullName = FullName,
            Login = string.Empty,
            Password = string.Empty,
            Phone = string.Empty,
            Email = Email
        };
    }
}
