using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class CommentStorage : ICommentStorage
    {
        public CommentBindingModel? Delete(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Comments.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Comments.Remove(element);
                context.SaveChanges();
                return element.GetComment;
            }
            return null;
        }

        public CommentBindingModel? GetElement(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .FirstOrDefault(c => (model.Id != 0 && c.Id == model.Id))?
                .GetComment;
        }

        public List<CommentBindingModel> GetFilteredList(CommentBindingModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .AsQueryable();

            if (model.ReviewId != 0)
                query = query.Where(c => c.ReviewId == model.ReviewId);

            return query.Select(c => c.GetComment).ToList();
        }

        public List<CommentBindingModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .Select(c => c.GetComment)
                .ToList();
        }

        public CommentBindingModel? Insert(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Comment.Create(model);
            if (entity == null) return null;
            context.Comments.Add(entity);
            context.SaveChanges();
            return entity.GetComment;
        }

        public CommentBindingModel? Update(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Comments.FirstOrDefault(c => c.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetComment;
        }
    }
}
