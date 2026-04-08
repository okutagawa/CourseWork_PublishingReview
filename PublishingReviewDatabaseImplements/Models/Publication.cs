using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.ViewModels;
using PublishingReviewDatabaseImplements.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace PublishingReviewDatabase.Models
{
    public class Publication
    {
        [Required]
        public string Title { get; private set; } = string.Empty;

        public int? SubjectId { get; private set; }

        public string SubjectText { get; private set; } = string.Empty;

        public DateTime? PublishDate { get; private set; }

        [Required]
        public int Volume { get; private set; }

        public int Id { get; private set; }

        public string AuthorsText { get; private set; } = string.Empty;

        public decimal ResourcesRate { get; private set; } = 0m;

        public string? Description { get; private set; }

        public virtual List<PublicationAuthor> Authors { get; private set; } = new();
        public virtual List<PublicationFavorite> Favorites { get; private set; } = new();
        public virtual List<Review> Reviews { get; private set; } = new();

        private Dictionary<int, User>? _publicationAuthors = null;

        [NotMapped]
        public Dictionary<int, User>? PublicationAuthors
        {
            get
            {
                if (_publicationAuthors == null && Authors != null)
                {
                    _publicationAuthors = Authors.ToDictionary(rec => rec.UserId, rec => rec.User);
                }
                return _publicationAuthors;
            }
        }

        public static Publication? Create(PublicationBindingModel model, PublishingDatabase context)
        {
            if (model == null) return null;

            var publication = new Publication
            {
                Title = model.Title,
                SubjectId = model.SubjectId,
                SubjectText = model.SubjectText ?? string.Empty,
                PublishDate = model.PublishDate,
                Volume = model.Volume,
                AuthorsText = model.AuthorsText ?? string.Empty,
                ResourcesRate = model.ResourcesRate,
                Description = model.Description
            };

            if (model.PublicationAuthors != null && model.PublicationAuthors.Count > 0)
            {
                publication.Authors = model.PublicationAuthors.Select(x => new PublicationAuthor
                {
                    User = context.Users.First(y => y.Id == x.Key)
                }).ToList();
            }

            return publication;
        }

        public void Update(PublicationBindingModel model)
        {
            if (model == null) return;

            Title = model.Title;
            SubjectId = model.SubjectId;
            SubjectText = model.SubjectText ?? string.Empty;
            PublishDate = model.PublishDate;
            Volume = model.Volume;
            AuthorsText = model.AuthorsText ?? string.Empty;
            ResourcesRate = model.ResourcesRate;
            Description = model.Description;
        }

        public void UpdateAuthors(PublishingDatabase context, PublicationBindingModel model)
        {
            var publicationAuthors = context.PublicationAuthors.Where(rec => rec.PublicationId == model.Id).ToList();

            if (publicationAuthors != null && publicationAuthors.Count > 0)
            {
                context.PublicationAuthors.RemoveRange(publicationAuthors.Where(rec => !model.PublicationAuthors.ContainsKey(rec.UserId)));
                context.SaveChanges();

                foreach (var upd in publicationAuthors)
                {
                    model.PublicationAuthors.Remove(upd.UserId);
                }
                context.SaveChanges();
            }

            var publication = context.Publications.First(x => x.Id == Id);
            foreach (var pa in model.PublicationAuthors)
            {
                context.PublicationAuthors.Add(new PublicationAuthor
                {
                    User = context.Users.First(x => x.Id == pa.Key),
                    Publication = publication
                });
                context.SaveChanges();
            }

            _publicationAuthors = null;
        }

        public PublicationBindingModel GetPublication => new PublicationBindingModel
        {
            Id = Id,
            Title = Title,
            PublishDate = PublishDate ?? DateTime.UtcNow,
            Authors = AuthorsText,
            Publisher = SubjectText,
            Description = Description,
            SubjectId = SubjectId,
            SubjectText = SubjectText,
            Volume = Volume,
            AuthorsText = AuthorsText,
            ResourcesRate = ResourcesRate,
            PublicationAuthors = PublicationAuthors?.ToDictionary(
                x => x.Key,
                x => new ReviewerBindingModel
                {
                    Id = x.Value.Id
                }) ?? new Dictionary<int, ReviewerBindingModel>()
        };
        public PublicationViewModel GetPublicationViewModel => new PublicationViewModel
        {
            Id = Id,
            Title = Title,
            PublishDate = PublishDate,
            Authors = AuthorsText,
            Publisher = SubjectText,
            Description = Description ?? string.Empty,
            Volume = Volume,
            SubjectText = SubjectText
        };
    }
}
