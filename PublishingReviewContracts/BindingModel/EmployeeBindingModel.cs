using PublishingReviewDataModels.Models;
using System;

namespace PublishingReviewContracts.BindingModel
{
    public class EmployeeBindingModel : IEmployeeModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Position { get; set; } = "Editor";
    }
}
