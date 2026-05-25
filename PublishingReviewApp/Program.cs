using Microsoft.AspNetCore.Identity;
using PublishingReviewBusinessLogic.BusinessLogics;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromHours(8);
});

builder.Services.AddScoped<IUserStorage, UserStorage>();
builder.Services.AddScoped<IPublicationStorage, PublicationStorage>();
builder.Services.AddScoped<IReviewStorage, ReviewStorage>();
builder.Services.AddScoped<ICommentStorage, CommentStorage>();
builder.Services.AddScoped<IAttachmentStorage, AttachmentStorage>();
builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<IPublicationLogic, PublicationLogic>();
builder.Services.AddScoped<IReviewLogic, ReviewLogic>();
builder.Services.AddScoped<ICommentLogic, CommentLogic>();
builder.Services.AddScoped<IAttachmentLogic, AttachmentLogic>();
builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
builder.Services.AddDbContext<PublishingDatabase>();
builder.Services.AddScoped<PublishingReviewApp.Services.ProductService>();
builder.Services.AddScoped<PublishingReviewApp.Services.ReviewService>();
builder.Services.AddScoped<PublishingReviewApp.Services.RatingService>();
builder.Services.AddScoped<PublishingReviewApp.Services.CategoryService>();

var app = builder.Build();
using(var scope=app.Services.CreateScope()){var db=scope.ServiceProvider.GetRequiredService<PublishingDatabase>(); db.Database.EnsureCreated(); if(!db.Categories.Any()){db.Categories.AddRange(new PublishingReviewDatabaseImplements.Models.Shop.Category{Name="Электроника"},new PublishingReviewDatabaseImplements.Models.Shop.Category{Name="Книги"},new PublishingReviewDatabaseImplements.Models.Shop.Category{Name="Дом"}); db.SaveChanges();} if(!db.Products.Any()){var cats=db.Categories.ToList(); for(int i=1;i<=6;i++){db.Products.Add(new PublishingReviewDatabaseImplements.Models.Shop.Product{Name=$"Товар {i}",Description=$"Описание {i}",Price=10*i,ImageUrl="https://via.placeholder.com/120"});} db.SaveChanges(); var ps=db.Products.ToList(); db.ProductCategories.AddRange(new PublishingReviewDatabaseImplements.Models.Shop.ProductCategory{ProductId=ps[0].Id,CategoryId=cats[0].Id},new PublishingReviewDatabaseImplements.Models.Shop.ProductCategory{ProductId=ps[1].Id,CategoryId=cats[1].Id},new PublishingReviewDatabaseImplements.Models.Shop.ProductCategory{ProductId=ps[2].Id,CategoryId=cats[2].Id}); db.SaveChanges(); }}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
