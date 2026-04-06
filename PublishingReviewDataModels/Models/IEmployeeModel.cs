using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishingReviewDataModels.Models
{
    public interface IEmployeeModel : IId
    {
        string FullName { get; }
        string Login { get; }
        string Password { get; }
        string Email { get; }
        string Phone { get; }
    }
}