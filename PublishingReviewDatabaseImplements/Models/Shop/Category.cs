using System.ComponentModel.DataAnnotations;
namespace PublishingReviewDatabaseImplements.Models.Shop;
public class Category { public int Id { get; set; } [Required, StringLength(100)] public string Name { get; set; } = string.Empty; public List<ProductCategory> ProductCategories { get; set; } = new(); }
