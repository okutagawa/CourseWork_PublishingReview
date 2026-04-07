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
    public class PublicationStorage : IPublicationStorage
    {
        public PublicationViewModel? Delete(PublicationBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var element = context.Publications.FirstOrDefault(x => x.Id == model.Id);
            if (element != null)
            {
                context.Publications.Remove(element);
                context.SaveChanges();
                return element.GetPublicationViewModel;
            }
            return null;
        }

        public PublicationViewModel? GetElement(PublicationSearchModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            return context.Publications
                .Include(x => x.Authors).ThenInclude(pa => pa.User)
                .Include(x => x.Reviews)
                .Include(x => x.Favorites)
                .FirstOrDefault(x => (model.Id != 0 && x.Id == model.Id) || (!string.IsNullOrEmpty(model.Title) && x.Title == model.Title))?
                .GetPublicationViewModel;
        }

        public List<PublicationViewModel> GetFilteredList(PublicationSearchModel model)
        {
            using var context = new PublishingDatabase();
            var query = context.Publications
                .Include(x => x.Authors).ThenInclude(pa => pa.User)
                .Include(x => x.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(model.Title))
            {
                query = query.Where(x => x.Title.Contains(model.Title));
            }

            return query.Select(x => x.GetPublicationViewModel).ToList();
        }

        public List<PublicationViewModel> GetFullList()
        {
            using var context = new PublishingDatabase();
            return context.Publications
                .Include(x => x.Authors).ThenInclude(pa => pa.User)
                .Include(x => x.Reviews)
                .Include(x => x.Favorites)
                .Select(x => x.GetPublicationViewModel)
                .ToList();
        }

        public PublicationViewModel? Insert(PublicationBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            var newPublication = Publication.Create(model, context);
            if (newPublication == null) return null;
            context.Publications.Add(newPublication);
            context.SaveChanges();
            return newPublication.GetPublicationViewModel;
        }

        public PublicationViewModel? Update(PublicationBindingModel model)
        {
            if (model == null) return null;
            using var context = new PublishingDatabase();
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var elem = context.Publications.FirstOrDefault(x => x.Id == model.Id);
                if (elem == null) return null;

                elem.Update(model);
                context.SaveChanges();

                if (model.PublicationAuthors != null && model.PublicationAuthors.Count > 0)
                {
                    elem.UpdateAuthors(context, model);
                }

                transaction.Commit();
                return elem.GetPublicationViewModel;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
