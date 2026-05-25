using System.ComponentModel.DataAnnotations;
using PublishingReviewDatabaseImplements.Models;
namespace PublishingReviewDatabaseImplements.Models.Shop;
public class ProductRating { public int Id { get; set; } public int ProductId { get; set; } public Product Product { get; set; } = null!; public int UserId { get; set; } public User User { get; set; } = null!; [Range(1,5)] public int Value { get; set; } public DateTime CreatedAt { get; set; } = DateTime.UtcNow; }
