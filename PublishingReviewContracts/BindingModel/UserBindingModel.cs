using PublishingReviewDataModels.Enums;
using PublishingReviewDataModels.Models;
using System;

namespace PublishingReviewContracts.BindingModel
{
    public class UserBindingModel : IUserModel
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // plain при создании/обновлении
        public string? PasswordHash { get; set; } // хранится в БД
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
