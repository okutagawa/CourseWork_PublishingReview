using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class UserStorage : IUserStorage
    {
        public UserBindingModel? Delete(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Users.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Users.Remove(element);
                context.SaveChanges();
                return element.GetUser;
            }
            return null;
        }

        public UserBindingModel? GetElement(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Users
                .Include(x => x.AuthoredPublications).ThenInclude(pa => pa.Publication)
                .Include(x => x.Reviews)
                .Include(x => x.Comments)
                .FirstOrDefault(x => (model.Id != 0 && x.Id == model.Id) || (!string.IsNullOrEmpty(model.Email) && x.Email == model.Email))?
                .GetUser;
        }

        public List<UserBindingModel> GetFilteredList(UserBindingModel model)
        {
            using var context = new PublishingDatabase();
            return context.Users
                .Include(x => x.Reviews)
                .Include(x => x.AuthoredPublications)
                .Select(x => x.GetUser)
                .ToList();
        }

        public List<UserBindingModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Users
                .Include(x => x.Reviews)
                .Include(x => x.AuthoredPublications)
                .Include(x => x.FavoritePublications)
                .Select(x => x.GetUser)
                .ToList();
        }

        public UserBindingModel? Insert(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var newUser = User.Create(model);
            if (newUser == null) return null;
            context.Users.Add(newUser);
            context.SaveChanges();
            return newUser.GetUser;
        }

        public UserBindingModel? Update(UserBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Users.FirstOrDefault(x => x.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetUser;
        }
    }
}
