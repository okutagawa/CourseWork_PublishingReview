using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.SearchModels;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class ReviewStorage : IReviewStorage
    {
        public ReviewViewModel? Delete(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Reviews.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Reviews.Remove(element);
                context.SaveChanges();
                return element.GetReviewViewModel;
            }
            return null;
        }

        public ReviewViewModel? GetElement(ReviewSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .Include(r => r.Comments)
                .Include(r => r.Attachments)
                .FirstOrDefault(r => (model.Id.HasValue && r.Id == model.Id.Value))?
                .GetReviewViewModel;
        }

        public List<ReviewViewModel> GetFilteredList(ReviewSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .AsQueryable();

            if (model.PublicationId.HasValue)
            {
                query = query.Where(r => r.PublicationId == model.PublicationId.Value);
            }

            if (model.ReviewerId.HasValue)
            {
                query = query.Where(r => r.UserId == model.ReviewerId.Value);
            }
            if (model.Status.HasValue)
            {
                query = model.Status.Value == PublishingReviewDataModels.Enums.ReviewStatus.Confirmed
                    ? query.Where(r => r.IsApproved)
                    : query.Where(r => !r.IsApproved);
            }

            return query.Select(r => r.GetReviewViewModel).ToList();
        }

        public List<ReviewViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .Include(r => r.Comments)
                .Include(r => r.Attachments)
                .Select(r => r.GetReviewViewModel)
                .ToList();
        }

        public ReviewViewModel? Insert(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Review.Create(model);
            if (entity == null) return null;
            context.Reviews.Add(entity);
            context.SaveChanges();
            return entity.GetReviewViewModel;
        }

        public ReviewViewModel? Update(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Reviews.FirstOrDefault(r => r.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetReviewViewModel;
        }
    }
}
