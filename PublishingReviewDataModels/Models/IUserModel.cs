using PublishingReviewDataModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IUserModel : IId
    {
        string Username { get; }
        string Password { get; }
        string Email { get; }
        string FullName { get; }
        UserRole Role { get; }
    }
}