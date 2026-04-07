using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PublishingReviewBusinessLogic.BusinessLogics;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.StoragesContracts;
using PublishingReviewDatabase;
using PublishingReviewDatabaseImplements.Implements;

var builder = WebApplication.CreateBuilder(args);

// --- Logging: log4net provider (файл log4net.config должен быть в корне проекта)
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();
builder.Logging.AddLog4Net("log4net.config");

// --- Конфигурация: строка подключения берётся из appsettings или переменной окружения
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? Environment.GetEnvironmentVariable("PUBLISHING_DB_CONNECTION");

// --- Регистрация DbContext делаем опциональной: если connectionString пустой — пропускаем регистрацию БД
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured. Set ConnectionStrings:Default or PUBLISHING_DB_CONNECTION.");
}
builder.Services.AddDbContext<PublishingDatabase>(options =>
    options.UseNpgsql(connectionString));

// Регистрация storage/logic, которые зависят от DbContext
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

builder.Services.AddScoped(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

// NoOpMailLogic можно зарегистрировать всегда (чтобы не требовать почту)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PublishingDatabase>();
    db.Database.Migrate();
}

// Middleware для ошибок (короткая JSON-обработка)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var msg = feature?.Error?.Message ?? "Unhandled error";
        await context.Response.WriteAsJsonAsync(new { error = "Request processing failed", details = msg });
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
