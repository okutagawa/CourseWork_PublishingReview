using Microsoft.EntityFrameworkCore;
using PublishingReviewDatabase.Models;
using PublishingReviewDatabaseImplements.Models;
using System;

namespace PublishingReviewDatabase
{
    public class PublishingDatabase : DbContext
    {
        public PublishingDatabase() { }

        public PublishingDatabase(DbContextOptions<PublishingDatabase> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Замените строку подключения на свою при необходимости
                // Пример для локального SQLEXPRESS:
                var connectionString = Environment.GetEnvironmentVariable("PUBLISHING_DB_CONNECTION")
                                       ?? @"Data Source=.\SQLEXPRESS;Initial Catalog=PublishingReviewDatabase;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True";

                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        // DbSet'ы для моделей (проверь, что имена классов совпадают с теми, что у тебя в Models)
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Publication> Publications { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Attachment> Attachments { get; set; } = null!;
        public DbSet<PublicationAuthor> PublicationAuthors { get; set; } = null!;
        public DbSet<PublicationFavorite> PublicationFavorites { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite keys для join-таблиц
            modelBuilder.Entity<PublicationAuthor>()
                .HasKey(pa => new { pa.PublicationId, pa.UserId });

            modelBuilder.Entity<PublicationAuthor>()
                .HasOne(pa => pa.Publication)
                .WithMany(p => p.Authors)
                .HasForeignKey(pa => pa.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PublicationAuthor>()
                .HasOne(pa => pa.User)
                .WithMany(u => u.AuthoredPublications)
                .HasForeignKey(pa => pa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PublicationFavorite>()
                .HasKey(pf => new { pf.UserId, pf.PublicationId });

            modelBuilder.Entity<PublicationFavorite>()
                .HasOne(pf => pf.User)
                .WithMany(u => u.FavoritePublications)
                .HasForeignKey(pf => pf.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PublicationFavorite>()
                .HasOne(pf => pf.Publication)
                .WithMany(p => p.Favorites)
                .HasForeignKey(pf => pf.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review -> Publication, Review -> User
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Publication)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PublicationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment -> Review, Comment -> User
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Review)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attachment -> Review
            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Review)
                .WithMany(r => r.Attachments)
                .HasForeignKey(a => a.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // Review approved by Employee (optional)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.ApprovedByEmployee)
                .WithMany()
                .HasForeignKey(r => r.ApprovedByEmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            base.OnModelCreating(modelBuilder);
        }
    }
}
