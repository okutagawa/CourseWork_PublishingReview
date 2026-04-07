using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class UserStorage : IUserStorage
    {
        public UserViewModel? Delete(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Users.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Users.Remove(element);
                context.SaveChanges();
                return element.GetUserViewModel;
            }
            return null;
        }

        public UserViewModel? GetElement(UserSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Users               
                .FirstOrDefault(x => (model.Id != 0 && x.Id == model.Id) || (!string.IsNullOrEmpty(model.Email) && x.Email == model.Email))?
                .GetUserViewModel;
        }

        public List<UserViewModel> GetFilteredList(UserSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                query = query.Where(x => x.Login.Contains(model.Username));
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                query = query.Where(x => x.Email.Contains(model.Email));
            }

            return query.Select(x => x.GetUserViewModel).ToList();
        }

        public List<UserViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Users             
                .Select(x => x.GetUserViewModel)
                .ToList();
        }

        public UserViewModel? Insert(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var newUser = User.Create(model);
            if (newUser == null) return null;
            context.Users.Add(newUser);
            context.SaveChanges();
            return newUser.GetUserViewModel;
        }

        public UserViewModel? Update(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Users.FirstOrDefault(x => x.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetUserViewModel;
        }
    }
}
