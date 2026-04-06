using PublishingReviewDataModels.Enums;
using PublishingReviewDataModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewContracts.SearchModels
{
    public class UserSearchModel
    {
        public int? Id { get; set; }
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}