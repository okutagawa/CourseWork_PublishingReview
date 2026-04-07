using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDataModels.Enums;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Models
{
    public class User
    {
        [Required]
        public string FullName { get; private set; } = string.Empty;

        [Required]
        public string Login { get; private set; } = string.Empty;

        [Required]
        public string Email { get; private set; } = string.Empty;

        // Роль: "User", "Employee", "Admin"
        public UserRole Role { get; private set; } = UserRole.Customer;

        public int Id { get; private set; }

        // Навигация
        public virtual List<Review> Reviews { get; private set; } = new();
        public virtual List<Comment> Comments { get; private set; } = new();
        public virtual List<PublicationAuthor> AuthoredPublications { get; private set; } = new();
        public virtual List<PublicationFavorite> FavoritePublications { get; private set; } = new();

        // Фабрика
        public static User? Create(UserBindingModel model)
        {
            if (model == null) return null;
            return new User
            {
                FullName = model.FullName,
                Login = model.Username,
                Email = model.Email,
                Role = model.Role
            };
        }

        public void Update(UserBindingModel model)
        {
            if (model == null) return;
            FullName = model.FullName;
            Login = model.Username;
            Email = model.Email;
            Role = model.Role;
        }

        public UserBindingModel GetUser => new UserBindingModel
        {
            Id = Id,
            FullName = FullName,
            Username = Login,
            Email = Email,
            Role = Role
        };
        public UserViewModel GetUserViewModel => new UserViewModel
        {
            Id = Id,
            FullName = FullName,
            Username = Login,
            Email = Email,
            Role = Role,
            Password = string.Empty
        };
    }
}
