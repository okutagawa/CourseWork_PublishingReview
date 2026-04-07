using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PublishingReviewBusinessLogic.BusinessLogics;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Implements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<PublishingDatabase>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default")
        ?? Environment.GetEnvironmentVariable("PUBLISHING_DB_CONNECTION")
        ?? "Host=localhost;Port=5432;Database=PublishingReview;Username=postgres;Password=izotov04";

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IUserStorage, UserStorage>();
builder.Services.AddScoped<IPublicationStorage, PublicationStorage>();
builder.Services.AddScoped<IReviewStorage, ReviewStorage>();
builder.Services.AddScoped<ICommentStorage, CommentStorage>();
builder.Services.AddScoped<IAttachmentStorage, AttachmentStorage>();
builder.Services.AddScoped<IEmployeeStorage, EmployeeStorage>();

builder.Services.AddScoped<IUserLogic, UserLogic>();
builder.Services.AddScoped<IPublicationLogic, PublicationLogic>();
builder.Services.AddScoped<IReviewLogic, ReviewLogic>();
builder.Services.AddScoped<ICommentLogic, CommentLogic>();
builder.Services.AddScoped<IAttachmentLogic, AttachmentLogic>();
builder.Services.AddScoped<IEmployeeLogic, EmployeeLogic>();

builder.Services.AddScoped<IPasswordHasher<UserBindingModel>, PasswordHasher<UserBindingModel>>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = "Request processing failed", details = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error.Message });
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
