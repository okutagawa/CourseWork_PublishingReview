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
    public class AttachmentStorage : IAttachmentStorage
    {
        public AttachmentViewModel? Delete(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Attachments.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Attachments.Remove(element);
                context.SaveChanges();
                return element.GetAttachmentViewModel;
            }
            return null;
        }

        public AttachmentViewModel? GetElement(AttachmentSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Attachments
                .Include(a => a.Review)
                .FirstOrDefault(a => model.Id.HasValue && a.Id == model.Id.Value)?
                .GetAttachmentViewModel;
        }

        public List<AttachmentViewModel> GetFilteredList(AttachmentSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Attachments
                .Include(a => a.Review)
                .AsQueryable();

            if (model.ReviewId.HasValue)
            {
                query = query.Where(a => a.ReviewId == model.ReviewId.Value);
            }

            return query.Select(a => a.GetAttachmentViewModel).ToList();
        }

        public List<AttachmentViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Attachments
                .Include(a => a.Review)
                .Select(a => a.GetAttachmentViewModel)
                .ToList();
        }

        public AttachmentViewModel? Insert(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Attachment.Create(model);
            if (entity == null) return null;
            context.Attachments.Add(entity);
            context.SaveChanges();
            return entity.GetAttachmentViewModel;
        }

        public AttachmentViewModel? Update(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Attachments.FirstOrDefault(a => a.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetAttachmentViewModel;
        }
    }
}
