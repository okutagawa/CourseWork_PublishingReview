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
    public class CommentStorage : ICommentStorage
    {
        public CommentViewModel? Delete(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Comments.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Comments.Remove(element);
                context.SaveChanges();
                return element.GetCommentViewModel;
            }
            return null;
        }

        public CommentViewModel? GetElement(CommentSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .FirstOrDefault(c => model.Id.HasValue && c.Id == model.Id.Value)?
                .GetCommentViewModel;
        }

        public List<CommentViewModel> GetFilteredList(CommentSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .AsQueryable();

            if (model.ReviewId.HasValue)
            {
                query = query.Where(c => c.ReviewId == model.ReviewId.Value);
            }

            return query.Select(c => c.GetCommentViewModel).ToList();
        }

        public List<CommentViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Comments
                .Include(c => c.User)
                .Include(c => c.Review)
                .Select(c => c.GetCommentViewModel)
                .ToList();
        }

        public CommentViewModel? Insert(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Comment.Create(model);
            if (entity == null) return null;
            context.Comments.Add(entity);
            context.SaveChanges();
            return entity.GetCommentViewModel;
        }

        public CommentViewModel? Update(CommentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Comments.FirstOrDefault(c => c.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetCommentViewModel;
        }
    }
}
