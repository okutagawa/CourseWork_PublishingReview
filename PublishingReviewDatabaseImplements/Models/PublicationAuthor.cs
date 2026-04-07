using PublishingReviewDatabase.Models;

namespace PublishingReviewDatabaseImplements.Models
{
    // Явная join-таблица Publication <-> User (Authors)
    public class PublicationAuthor
    {
        public int PublicationId { get; set; }
        public Publication Publication { get; set; } = null!;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
