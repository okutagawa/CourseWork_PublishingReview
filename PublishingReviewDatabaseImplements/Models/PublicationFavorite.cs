using PublishingReviewDatabase.Models;

namespace PublishingReviewDatabaseImplements.Models
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
