using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.ViewModels
{
    public class EmployeeSearchModel
    {
        public int Id { get; set; }

        [DisplayName("ФИО сотрудника")]
        public string FullName { get; set; } = string.Empty;

        [DisplayName("Логин")]
        public string Login { get; set; } = string.Empty;

        [DisplayName("Пароль")]
        public string Password { get; set; } = string.Empty;

        [DisplayName("Почта")]
        public string Email { get; set; } = string.Empty;

        [DisplayName("Номер телефона")]
        public string Phone { get; set; } = string.Empty;
    }
}