using System.ComponentModel.DataAnnotations;
using PublishingReviewDatabaseImplements.Models;
namespace PublishingReviewDatabaseImplements.Models.Shop;
public class ProductReview { public int Id { get; set; } public int ProductId { get; set; } public Product Product { get; set; } = null!; public int UserId { get; set; } public User User { get; set; } = null!; [Required, StringLength(1500)] public string Text { get; set; } = string.Empty; public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
