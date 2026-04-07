using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class ReviewStorage : IReviewStorage
    {
        public ReviewBindingModel? Delete(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Reviews.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Reviews.Remove(element);
                context.SaveChanges();
                return element.GetReview;
            }
            return null;
        }

        public ReviewBindingModel? GetElement(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .Include(r => r.Comments)
                .Include(r => r.Attachments)
                .FirstOrDefault(r => (model.Id != 0 && r.Id == model.Id))?
                .GetReview;
        }

        public List<ReviewBindingModel> GetFilteredList(ReviewBindingModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .AsQueryable();

            if (model.PublicationId != 0)
                query = query.Where(r => r.PublicationId == model.PublicationId);

            return query.Select(r => r.GetReview).ToList();
        }

        public List<ReviewBindingModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Reviews
                .Include(r => r.Publication)
                .Include(r => r.User)
                .Include(r => r.Comments)
                .Include(r => r.Attachments)
                .Select(r => r.GetReview)
                .ToList();
        }

        public ReviewBindingModel? Insert(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Review.Create(model);
            if (entity == null) return null;
            context.Reviews.Add(entity);
            context.SaveChanges();
            return entity.GetReview;
        }

        public ReviewBindingModel? Update(ReviewBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Reviews.FirstOrDefault(r => r.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetReview;
        }
    }
}
