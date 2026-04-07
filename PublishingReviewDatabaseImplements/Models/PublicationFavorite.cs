// PublishingReviewDatabase/Models/PublicationFavorite.cs
using PublishingReviewDatabaseImplements.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublishingReviewDatabase.Models
{
    // Явная join-таблица Publication <-> User (Favorites)
    public class PublicationFavorite
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int PublicationId { get; set; }
        public Publication Publication { get; set; } = null!;
    }
}
