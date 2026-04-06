using PublishingReviewDataModels.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        [DisplayName("Ник")]
        public string Username { get; set; } = string.Empty;

        [DisplayName("Пароль")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Почта")]
        public string Email { get; set; } = string.Empty;

        [DisplayName("ФИО")]
        public string FullName { get; set; } = string.Empty;

        [DisplayName("Роль")]
        public UserRole Role { get; set; }
    }
}