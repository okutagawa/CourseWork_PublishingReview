using Microsoft.EntityFrameworkCore;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using System.Collections.Generic;
using System.Linq;

namespace PublishingReviewDatabaseImplements.Implements
{
    public class AttachmentStorage : IAttachmentStorage
    {
        public AttachmentBindingModel? Delete(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Attachments.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Attachments.Remove(element);
                context.SaveChanges();
                return element.GetAttachment;
            }
            return null;
        }

        public AttachmentBindingModel? GetElement(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Attachments
                .Include(a => a.Review)
                .FirstOrDefault(a => (model.Id != 0 && a.Id == model.Id))?
                .GetAttachment;
        }

        public List<AttachmentBindingModel> GetFilteredList(AttachmentBindingModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Attachments
                .Include(a => a.Review)
                .AsQueryable();

            if (model.ReviewId != 0)
                query = query.Where(a => a.ReviewId == model.ReviewId);

            return query.Select(a => a.GetAttachment).ToList();
        }

        public List<AttachmentBindingModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Attachments
                .Include(a => a.Review)
                .Select(a => a.GetAttachment)
                .ToList();
        }

        public AttachmentBindingModel? Insert(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var entity = Attachment.Create(model);
            if (entity == null) return null;
            context.Attachments.Add(entity);
            context.SaveChanges();
            return entity.GetAttachment;
        }

        public AttachmentBindingModel? Update(AttachmentBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var elem = context.Attachments.FirstOrDefault(a => a.Id == model.Id);
            if (elem == null) return null;
            elem.Update(model);
            context.SaveChanges();
            return elem.GetAttachment;
        }
    }
}
