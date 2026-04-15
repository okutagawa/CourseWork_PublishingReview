using Microsoft.AspNetCore.Mvc;
using PublishingReviewApp.Models;
using PublishingReviewContracts.BindingModel;
using PublishingReviewContracts.BusinessLogicContracts;
using PublishingReviewContracts.SearchModels;
using PublishingReviewDatabase;
using PublishingReviewDatabase.Models;
using PublishingReviewDatabaseImplements.Models;
using PublishingReviewDataModels.Enums;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace PublishingReviewApp.Controllers;

[Route("[controller]/[action]")]
public class HomeController : Controller
{
    private const string SessionAuthKey = "IsAuthenticated";
    private const string SessionEmailKey = "UserEmail";
    private const string SessionFullNameKey = "UserFullName";
    private const string SessionRoleKey = "UserRole";

    private readonly ILogger<HomeController> _logger;

    private readonly IUserLogic _userLogic;
    private readonly IPublicationLogic _publicationLogic;
    private readonly IReviewLogic _reviewLogic;
    private readonly ICommentLogic _commentLogic;
    private readonly IAttachmentLogic _attachmentLogic;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly PublishingDatabase _db;

    public HomeController(
        ILogger<HomeController> logger,
        IUserLogic userLogic,
        IPublicationLogic publicationLogic,
        IReviewLogic reviewLogic,
        ICommentLogic commentLogic,
        IAttachmentLogic attachmentLogic,
        IWebHostEnvironment webHostEnvironment,
        PublishingDatabase db)
    {
        _logger = logger;
        _userLogic = userLogic;
        _publicationLogic = publicationLogic;
        _reviewLogic = reviewLogic;
        _commentLogic = commentLogic;
        _attachmentLogic = attachmentLogic;
        _webHostEnvironment = webHostEnvironment;
        _db = db;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Enter()
    {
        if (IsAuthenticated())
        {
            return RedirectToAction(nameof(DashboardPage));
        }

        ViewBag.Error = TempData["Error"];
        ViewBag.Success = TempData["Success"];
        return View();
    }

    [HttpPost]
    public IActionResult Enter(LoginRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            TempData["Error"] = "Введите email и пароль.";
            return RedirectToAction(nameof(Enter));
        }

        var user = _userLogic.ReadElement(new UserSearchModel { Email = request.Email.Trim() });

        if (user is null)
        {
            TempData["Error"] = "Неверный email или пароль.";
            return RedirectToAction(nameof(Enter));
        }

        HttpContext.Session.SetString(SessionAuthKey, bool.TrueString);
        HttpContext.Session.SetString(SessionEmailKey, user.Email);
        HttpContext.Session.SetString(SessionFullNameKey, user.FullName);
        HttpContext.Session.SetString(SessionRoleKey, user.Role == UserRole.Employee ? "employee" : "user");

        return RedirectToAction(nameof(DashboardPage));
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (IsAuthenticated())
        {
            return RedirectToAction(nameof(DashboardPage));
        }

        ViewBag.Error = TempData["Error"];
        ViewBag.Success = TempData["Success"];
        return View();
    }

    [HttpPost]
    public IActionResult Register(RegisterRequestModel request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName)
            || string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.Role))
        {
            TempData["Error"] = "Заполните обязательные поля: ФИО, Email, Пароль и Роль.";
            return RedirectToAction(nameof(Register));
        }

        if (request.Password.Trim().Length < 6)
        {
            TempData["Error"] = "Пароль должен содержать минимум 6 символов.";
            return RedirectToAction(nameof(Register));
        }

        var allowedRoles = new[] { "user", "employee" };
        var normalizedRole = request.Role.Trim().ToLowerInvariant();
        if (!allowedRoles.Contains(normalizedRole))
        {
            TempData["Error"] = "Некорректная роль пользователя.";
            return RedirectToAction(nameof(Register));
        }

        if (_userLogic.ReadElement(new UserSearchModel { Email = request.Email.Trim() }) != null)
        {
            TempData["Error"] = "Пользователь с таким email уже зарегистрирован.";
            return RedirectToAction(nameof(Register));
        }

        try
        {
            _userLogic.Create(new UserBindingModel
            {
                FullName = request.FullName.Trim(),
                Username = request.Email.Trim(),
                Email = request.Email.Trim(),
                Password = request.Password,
                Role = normalizedRole == "employee" ? UserRole.Employee : UserRole.Customer
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cannot persist user {Email} to database", request.Email);
            TempData["Error"] = $"Не удалось сохранить пользователя в БД: {ex.Message}";
            return RedirectToAction(nameof(Register));
        }

        TempData["Success"] = "Регистрация выполнена. Теперь войдите в систему.";
        return RedirectToAction(nameof(Enter));
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Enter));
    }

    [HttpGet]
    public IActionResult Privacy()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        ViewBag.UserEmail = HttpContext.Session.GetString(SessionEmailKey);
        ViewBag.UserFullName = HttpContext.Session.GetString(SessionFullNameKey);
        ViewBag.UserRole = GetRoleDisplayName();

        return View();
    }

    [HttpGet]
    public IActionResult DashboardPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var publications = _publicationLogic.ReadList(null) ?? new();
        var reviews = _reviewLogic.ReadList(null) ?? new();
        ViewBag.TotalPublications = publications.Count;
        ViewBag.WaitingForReviewer = publications.Count(p => reviews.All(r => r.PublicationId != p.Id));
        ViewBag.InReview = reviews.Count(r => r.Status == ReviewStatus.Pending);
        ViewBag.RequiresRevision = 0;
        ViewBag.Approved = reviews.Count(r => r.Status == ReviewStatus.Confirmed);
        ViewBag.Rejected = 0;

        return View();
    }

    [HttpGet]
    public IActionResult ReviewQueuePage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        return View();
    }

    [HttpGet]
    public IActionResult CatalogPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        ViewBag.IsEmployee = IsEmployee();
        return View();
    }

    [HttpGet]
    public IActionResult FavoritesPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View("FavoritePage");
    }

    [HttpGet]
    public IActionResult UserReviewsPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View();
    }


    [HttpGet]
    public IActionResult CreatePublicationPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View();
    }

    [HttpGet]
    public IActionResult AssignReviewerPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View();
    }

    [HttpGet]
    public IActionResult SubmitDecisionPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View();
    }

    [HttpGet]
    public IActionResult ReportPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        return View();
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var publications = _publicationLogic.ReadList(null) ?? new();
        var reviews = _reviewLogic.ReadList(null) ?? new();
        var queue = BuildReviewQueue(publications, reviews);
        var model = new ReviewDashboardModel
        {
            TotalPublications = publications.Count,
            WaitingForReviewer = queue.Count(x => x.State == ReviewWorkflowState.WaitingForReviewer),
            InReview = queue.Count(x => x.State == ReviewWorkflowState.InReview),
            RequiresRevision = queue.Count(x => x.State == ReviewWorkflowState.RequiresRevision),
            Approved = queue.Count(x => x.State == ReviewWorkflowState.Approved),
            Rejected = queue.Count(x => x.State == ReviewWorkflowState.Rejected),
            NearestDeadlines = queue.Where(x => x.DeadlineUtc.HasValue).OrderBy(x => x.DeadlineUtc).Take(5).ToList()
        };

        return Ok(model);
    }

    [HttpGet]
    public IActionResult ReviewQueueList([FromQuery] ReviewWorkflowState? state)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var queue = BuildReviewQueue(_publicationLogic.ReadList(null) ?? new(), _reviewLogic.ReadList(null) ?? new()).AsEnumerable();
        if (state.HasValue)
        {
            queue = queue.Where(x => x.State == state.Value);
        }
        return Ok(queue.OrderBy(x => x.State).ThenBy(x => x.DeadlineUtc ?? DateTime.MaxValue).Select(MapReviewTaskForOutput).ToList());
    }

    [HttpGet]
    public IActionResult GetCatalog([FromQuery] string? search, [FromQuery] string? publicationType)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var publications = _publicationLogic.ReadList(null) ?? new();
        var reviews = _reviewLogic.ReadList(null) ?? new();

        var query = publications.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var searchText = search.Trim();
            query = query.Where(x => x.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)
                || x.Authors.Contains(searchText, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(publicationType))
        {
            query = query.Where(x => x.Publisher.Equals(publicationType.Trim(), StringComparison.OrdinalIgnoreCase));
        }
        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var currentUser = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        var favoriteIds = currentUser == null
            ? new HashSet<int>()
            : _db.PublicationFavorites.Where(x => x.UserId == currentUser.Id).Select(x => x.PublicationId).ToHashSet();

        return Ok(query.OrderBy(x => x.Id).Select(x => new
        {
            x.Id,
            x.Title,
            AuthorFullName = x.Authors,
            PublicationType = x.Publisher,
            ReviewSummary = x.Description,
            IsFavorite = favoriteIds.Contains(x.Id),
            ReviewCount = reviews.Count(r => r.PublicationId == x.Id)
        }));
    }

    [HttpPost]
    public IActionResult CreatePublication([FromBody] PublicationReviewCreateModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.AuthorFullName) || string.IsNullOrWhiteSpace(request.PublicationType))
        {
            return BadRequest("Не заполнены обязательные поля: Title, AuthorFullName, PublicationType.");
        }

        var created = _publicationLogic.Create(new PublicationBindingModel
        {
            Title = request.Title.Trim(),
            Authors = request.AuthorFullName.Trim(),
            Publisher = request.PublicationType.Trim(),
            PublishDate = DateTime.UtcNow,
            Description = "Без итоговой рецензии",
            Volume = 1,
            SubjectText = request.PublicationType.Trim()
        });

        if (!created)
        {
            return BadRequest("Не удалось сохранить публикацию в БД.");
        }

        var publication = _publicationLogic.ReadList(new PublicationSearchModel { Title = request.Title.Trim() })?
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        if (publication is null)
        {
            return BadRequest("Публикация создана, но не найдена при повторном чтении.");
        }
        _logger.LogInformation("Publication {PublicationId} added to catalog", publication.Id);
        return Ok(new PublicationCatalogItemModel(
            publication.Id,
            publication.Title,
            publication.Authors,
            publication.Publisher,
            publication.Description));
    }

    [HttpPut]
    public IActionResult UpdatePublication([FromBody] PublicationCatalogItemModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (request.Id <= 0 || string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.AuthorFullName) || string.IsNullOrWhiteSpace(request.PublicationType))
        {
            return BadRequest("Передайте корректные данные публикации.");
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = request.Id });
        if (publication is null)
        {
            return NotFound("Публикация не найдена.");
        }

        var updated = _publicationLogic.Update(new PublicationBindingModel
        {
            Id = request.Id,
            Title = request.Title.Trim(),
            Authors = request.AuthorFullName.Trim(),
            Publisher = request.PublicationType.Trim(),
            PublishDate = publication.PublishDate ?? DateTime.UtcNow,
            Description = string.IsNullOrWhiteSpace(publication.Description) ? "Без итоговой рецензии" : publication.Description,
            Volume = publication.Volume,
            SubjectId = publication.SubjectId,
            ResourcesRate = publication.ResourcesRate,
            SubjectText = request.PublicationType.Trim()
        });

        if (!updated)
        {
            return BadRequest("Не удалось обновить публикацию.");
        }
        return Ok(request with
        {
            Title = request.Title.Trim(),
            AuthorFullName = request.AuthorFullName.Trim(),
            PublicationType = request.PublicationType.Trim(),
            ReviewSummary = publication.Description ?? "Без итоговой рецензии"
        });
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePublication(int id)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var removed = _publicationLogic.Delete(new PublicationBindingModel { Id = id });
        return !removed ? NotFound("Публикация не найдена.") : Ok();
    }

    [HttpPost]
    public IActionResult ToggleFavorite([FromBody] FavoriteToggleModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (request.PublicationId <= 0)
        {
            return BadRequest("PublicationId обязателен.");
        }

        var currentUserEmail = GetCurrentUserEmail();
        if (string.IsNullOrWhiteSpace(currentUserEmail))
        {
            return Unauthorized();
        }

        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return NotFound("Пользователь не найден.");
        }

        var publicationExists = _publicationLogic.ReadElement(new PublicationSearchModel { Id = request.PublicationId }) != null;
        if (!publicationExists)
        {
            return NotFound("Публикация не найдена.");
        }

        var existing = _db.PublicationFavorites.FirstOrDefault(x => x.UserId == user.Id && x.PublicationId == request.PublicationId);
        if (existing is null)
        {
            _db.PublicationFavorites.Add(new PublicationFavorite { UserId = user.Id, PublicationId = request.PublicationId });
            _db.SaveChanges();
            return Ok(new { isFavorite = true });
        }
        _db.PublicationFavorites.Remove(existing);
        _db.SaveChanges();
        return Ok(new { isFavorite = false });
    }

    [HttpGet]
    public IActionResult FavoriteList()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return Ok(new List<PublicationCatalogItemModel>());
        }

        var publicationIds = _db.PublicationFavorites.Where(x => x.UserId == user.Id).Select(x => x.PublicationId).ToHashSet();
        var data = _publicationLogic.ReadList(null)?
            .Where(x => publicationIds.Contains(x.Id))
            .OrderBy(x => x.Title)
            .Select(x => new PublicationCatalogItemModel(x.Id, x.Title, x.Authors, x.Publisher, x.Description))
            .ToList() ?? new List<PublicationCatalogItemModel>();
        return Ok(data);
    }

    [HttpGet]
    public IActionResult UserReviewList()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return Ok(new List<UserReviewRecordModel>());
        }
        var reviews = _reviewLogic.ReadList(new ReviewSearchModel { ReviewerId = user.Id }) ?? new();
        var attachments = _attachmentLogic.ReadList(null)?
            .GroupBy(x => x.ReviewId)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(a => a.UploadedAt).FirstOrDefault(), EqualityComparer<int>.Default)
            ?? new Dictionary<int, PublishingReviewContracts.ViewModels.AttachmentViewModel?>();
        var publications = _publicationLogic.ReadList(null)?.ToDictionary(x => x.Id, x => x.Title, EqualityComparer<int>.Default) ?? new Dictionary<int, string>();
        var data = reviews
            .OrderByDescending(x => x.CreatedAt)
            .Select(x =>
            {
                attachments.TryGetValue(x.Id, out var attachment);
                var attachmentUrl = attachment is null || string.IsNullOrWhiteSpace(attachment.StoragePath)
                    ? null
                    : attachment.StoragePath;
                return new UserReviewRecordModel(
                    x.Id,
                    currentUserEmail,
                    x.PublicationId,
                    publications.TryGetValue(x.PublicationId, out var title) ? title : $"Публикация #{x.PublicationId}",
                    x.Content,
                    attachment?.FileName,
                    attachmentUrl,
                    x.CreatedAt);
            })
            .ToList();
        return Ok(data);
    }

    [HttpGet("{publicationId:int}")]
    public IActionResult PublicationReviewList(int publicationId)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = publicationId });
        if (publication is null)
        {
            return NotFound("Издание не найдено.");
        }

        var users = _userLogic.ReadList(null)?.ToDictionary(x => x.Id, x => x.Email, EqualityComparer<int>.Default) ?? new Dictionary<int, string>();
        var attachments = _attachmentLogic.ReadList(null)?
            .GroupBy(x => x.ReviewId)
            .ToDictionary(x => x.Key, x => x.OrderByDescending(a => a.UploadedAt).FirstOrDefault(), EqualityComparer<int>.Default)
            ?? new Dictionary<int, PublishingReviewContracts.ViewModels.AttachmentViewModel?>();
        var comments = _commentLogic.ReadList(null) ?? new();
        var data = (_reviewLogic.ReadList(new ReviewSearchModel { PublicationId = publicationId }) ?? new())
            .OrderByDescending(x => x.CreatedAt)
            .Select(x =>
            {
                attachments.TryGetValue(x.Id, out var attachment);
                return new
                {
                    x.Id,
                    x.PublicationId,
                    PublicationTitle = publication.Title,
                    ReviewText = x.Content,
                    AttachmentFileName = attachment?.FileName,
                    AttachmentUrl = attachment is null ? null : $"/Home/ReadAttachment/{x.Id}",
                    CreatedUtc = x.CreatedAt,
                    UserEmail = users.TryGetValue(x.ReviewerId, out var email) ? email : $"user-{x.ReviewerId}",
                    Comments = comments
                    .Where(c => c.ReviewId == x.Id)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new ReviewCommentItemModel(
                        c.Id,
                        c.ReviewId,
                        users.TryGetValue(c.AuthorId, out var authorEmail) ? authorEmail : $"user-{c.AuthorId}",
                        c.Content,
                        c.CreatedAt))
                    .ToList()
                };
            })
            .ToList();

        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserReview([FromForm] UserReviewUpsertModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (request.PublicationId <= 0 || string.IsNullOrWhiteSpace(request.ReviewText))
        {
            return BadRequest("PublicationId и ReviewText обязательны.");
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return NotFound("Пользователь не найден.");
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = request.PublicationId });
        if (publication is null)
        {
            return NotFound("Издание не найдено.");
        }

        var created = _reviewLogic.Create(new ReviewBindingModel
        {
            PublicationId = publication.Id,
            ReviewerId = user.Id,
            Content = request.ReviewText.Trim(),
            Rating = 0,
            Status = ReviewStatus.Pending,
            CreatedAt = DateTime.UtcNow
        });

        if (!created)
        {
            return BadRequest("Не удалось сохранить рецензию.");
        }

        var review = _reviewLogic.ReadList(new ReviewSearchModel { PublicationId = publication.Id })?
            .Where(x => x.ReviewerId == user.Id && x.Content == request.ReviewText.Trim())
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        string? attachmentUrl = null;
        string? attachmentName = null;
        if (review is not null && request.Attachment is not null && request.Attachment.Length > 0)
        {
            (attachmentName, attachmentUrl) = await SaveReviewAttachmentAsync(review.Id, request.Attachment);
        }

        return Ok(new UserReviewRecordModel(
            review?.Id ?? 0,
            currentUserEmail,
            publication.Id,
            publication.Title,
            request.ReviewText.Trim(),
            attachmentName,
            attachmentUrl,
            DateTime.UtcNow));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUserReview([FromForm] UserReviewUpsertModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (request.Id <= 0 || request.PublicationId <= 0 || string.IsNullOrWhiteSpace(request.ReviewText))
        {
            return BadRequest("Id, PublicationId и ReviewText обязательны.");
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return NotFound("Пользователь не найден.");
        }

        var existingReview = _reviewLogic.ReadElement(new ReviewSearchModel { Id = request.Id });
        if (existingReview is null || existingReview.ReviewerId != user.Id)
        {
            return NotFound("Рецензия не найдена.");
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = request.PublicationId });
        if (publication is null)
        {
            return NotFound("Издание не найдено.");
        }
        if (existingReview.PublicationId != request.PublicationId)
        {
            return BadRequest("Нельзя менять издание при редактировании существующей рецензии. Выберите корректное издание.");
        }

        var normalizedReviewText = request.ReviewText.Trim();
        var isTextChanged = !string.Equals(existingReview.Content, normalizedReviewText, StringComparison.Ordinal);

        var updated = _reviewLogic.Update(new ReviewBindingModel
        {
            Id = request.Id,
            PublicationId = publication.Id,
            ReviewerId = user.Id,
            Content = normalizedReviewText,
            Rating = existingReview.Rating,
            Status = isTextChanged ? ReviewStatus.Pending : existingReview.Status,
            CreatedAt = existingReview.CreatedAt,
            DeadlineUtc = existingReview.DeadlineUtc,
            ConfirmedById = isTextChanged ? null : existingReview.ConfirmedById
        });

        if (!updated)
        {
            return BadRequest("Не удалось обновить рецензию.");
        }

        string? attachmentUrl = null;
        string? attachmentName = null;
        if (request.Attachment is not null && request.Attachment.Length > 0)
        {
            (attachmentName, attachmentUrl) = await SaveReviewAttachmentAsync(request.Id, request.Attachment);
        }
        else
        {
            var existingAttachment = (_attachmentLogic.ReadList(new AttachmentSearchModel { ReviewId = request.Id }) ?? new())
                .OrderByDescending(x => x.UploadedAt)
                .FirstOrDefault();
            attachmentName = existingAttachment?.FileName;
            attachmentUrl = existingAttachment?.StoragePath;
        }

        return Ok(new UserReviewRecordModel(
            request.Id,
            currentUserEmail,
            publication.Id,
            publication.Title,
            normalizedReviewText,
            attachmentName,
            attachmentUrl,
            existingReview.CreatedAt));
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeleteUserReview(int id)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenUserApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return NotFound("Пользователь не найден.");
        }

        var review = _reviewLogic.ReadElement(new ReviewSearchModel { Id = id });
        if (review is null || review.ReviewerId != user.Id)
        {
            return NotFound("Рецензия не найдена.");
        }
        var deleted = _reviewLogic.Delete(new ReviewBindingModel { Id = id });
        return !deleted ? NotFound("Рецензия не найдена.") : Ok();
    }

    [HttpGet("{reviewId:int}")]
    public IActionResult CommentList(int reviewId)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var users = _userLogic.ReadList(null)?.ToDictionary(x => x.Id, x => x.Email, EqualityComparer<int>.Default) ?? new Dictionary<int, string>();
        var data = (_commentLogic.ReadList(new CommentSearchModel { ReviewId = reviewId }) ?? new())
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ReviewCommentItemModel(
                x.Id,
                x.ReviewId,
                users.TryGetValue(x.AuthorId, out var author) ? author : $"user-{x.AuthorId}",
                x.Content,
                x.CreatedAt))
            .ToList();
        return Ok(data);
    }

    [HttpPost]
    public IActionResult AddComment([FromBody] ReviewCommentCreateModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (request.ReviewId <= 0 || string.IsNullOrWhiteSpace(request.Text))
        {
            return BadRequest("ReviewId и Text обязательны.");
        }

        var currentUserEmail = GetCurrentUserEmail() ?? string.Empty;
        var user = _userLogic.ReadElement(new UserSearchModel { Email = currentUserEmail });
        if (user is null)
        {
            return NotFound("Пользователь не найден.");
        }

        var review = _reviewLogic.ReadElement(new ReviewSearchModel { Id = request.ReviewId });
        if (review is null)
        {
            return NotFound("Рецензия не найдена.");
        }

        var created = _commentLogic.Create(new CommentBindingModel
        {
            ReviewId = request.ReviewId,
            AuthorId = user.Id,
            Content = request.Text.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        if (!created)
        {
            return BadRequest("Не удалось сохранить комментарий.");
        }

        var comment = _commentLogic.ReadList(new CommentSearchModel { ReviewId = request.ReviewId })?
            .Where(x => x.AuthorId == user.Id && x.Content == request.Text.Trim())
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();

        var author = HttpContext.Session.GetString(SessionFullNameKey) ?? currentUserEmail;
        return Ok(new ReviewCommentItemModel(comment?.Id ?? 0, request.ReviewId, author, request.Text.Trim(), DateTime.UtcNow));
    }

    [HttpGet]
    public IActionResult ReportData([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] string format = "json")
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
        {
            return BadRequest("Дата начала периода не может быть позже даты окончания.");
        }

        var from = dateFrom?.Date;
        var to = dateTo?.Date.AddDays(1).AddTicks(-1);
        var data = BuildReviewQueue(_publicationLogic.ReadList(null) ?? new(), _reviewLogic.ReadList(null) ?? new())
            .Where(x => !from.HasValue || x.CreatedUtc >= from.Value)
            .Where(x => !to.HasValue || x.CreatedUtc <= to.Value)
            .OrderByDescending(x => x.CreatedUtc)
            .ToList();

        if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
        {
            var csv = new StringBuilder();
            csv.AppendLine("Идентификатор задачи;Идентификатор издания;Название;Автор;Тип издания;Статус;Рецензент;Срок;Дата создания");
            foreach (var item in data)
            {
                csv.AppendLine($"{item.Id};{item.PublicationId};{item.Title};{item.AuthorFullName};{item.PublicationType};{ToRussianState(item.State)};{item.ReviewerEmail ?? string.Empty};{item.DeadlineUtc:dd.MM.yyyy HH:mm};{item.CreatedUtc:dd.MM.yyyy HH:mm}");
            }

            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv.ToString())).ToArray();
            return File(bytes, "text/csv; charset=utf-8", $"review-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }

        var summary = new
        {
            total = data.Count,
            waiting = data.Count(x => x.State == ReviewWorkflowState.WaitingForReviewer),
            inReview = data.Count(x => x.State == ReviewWorkflowState.InReview),
            needsRevision = data.Count(x => x.State == ReviewWorkflowState.RequiresRevision),
            approved = data.Count(x => x.State == ReviewWorkflowState.Approved),
            rejected = data.Count(x => x.State == ReviewWorkflowState.Rejected)
        };

        return Ok(new { summary, items = data.Select(MapReviewTaskForOutput).ToList() });

    }

    [HttpPost]
    public IActionResult CreatePublicationForReview([FromBody] PublicationReviewCreateModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.AuthorFullName) || string.IsNullOrWhiteSpace(request.PublicationType))
        {
            return BadRequest("Не заполнены обязательные поля: Title, AuthorFullName, PublicationType.");
        }

        var created = _publicationLogic.Create(new PublicationBindingModel
        {
            Title = request.Title.Trim(),
            Authors = request.AuthorFullName.Trim(),
            Publisher = request.PublicationType.Trim(),
            PublishDate = DateTime.UtcNow,
            Description = request.EditorComment?.Trim() ?? "Передано на рецензирование",
            Volume = 1,
            SubjectText = request.PublicationType.Trim()
        });
        if (!created)
        {
            return BadRequest("Не удалось создать публикацию.");
        }

        var publication = _publicationLogic.ReadList(new PublicationSearchModel { Title = request.Title.Trim() })?
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();
        if (publication is null)
        {
            return BadRequest("Публикация создана, но не найдена.");
        }

        var item = new ReviewTaskModel(
            publication.Id,
            publication.Id,
            publication.Title,
            publication.Authors,
            publication.Publisher,
            ReviewWorkflowState.WaitingForReviewer,
            null,
            null,
            request.EditorComment?.Trim(),
            DateTime.UtcNow);

        _logger.LogInformation("Publication {PublicationId} created and added to review queue", publication.Id);
        return CreatedAtAction(nameof(GetReviewTaskById), new { id = item.Id }, item);
    }


    [HttpGet("{id:int}")]
    public IActionResult GetReviewTaskById(int id)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var review = _reviewLogic.ReadElement(new ReviewSearchModel { Id = id });
        if (review is null)
        {
            return NotFound();
        }
        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = review.PublicationId });
        if (publication is null)
        {
            return NotFound();
        }

        var reviewer = _userLogic.ReadElement(new UserSearchModel { Id = review.ReviewerId });
        var state = review.Status == ReviewStatus.Confirmed ? ReviewWorkflowState.Approved : ReviewWorkflowState.InReview;
        var item = new ReviewTaskModel(
            review.Id,
            publication.Id,
            publication.Title,
            publication.Authors,
            publication.Publisher,
            state,
            reviewer?.Email,
            null,
            null,
            review.CreatedAt);
        return Ok(MapReviewTaskForOutput(item));
    }

    [HttpPost]
    public async Task<IActionResult> AssignReviewer()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var request = await TryReadRequestModelAsync<ReviewerAssignmentModel>();
        if (request is null)
        {
            return BadRequest("Тело запроса не передано.");
        }

        if (request.TaskId <= 0 || string.IsNullOrWhiteSpace(request.ReviewerEmail))
        {
            return BadRequest("TaskId и ReviewerEmail обязательны.");
        }

        var reviewer = _userLogic.ReadElement(new UserSearchModel { Email = request.ReviewerEmail.Trim() });
        if (reviewer is null)
        {
            return NotFound("Рецензент не найден.");
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = request.TaskId });
        if (publication is null)
        {
            return NotFound($"Задача рецензирования #{request.TaskId} не найдена.");
        }

        var deadlineUtc = NormalizeUtcDateTime(request.DeadlineUtc);

        _reviewLogic.Create(new ReviewBindingModel
        {
            PublicationId = publication.Id,
            ReviewerId = reviewer.Id,
            Content = "Назначено на рецензирование",
            Rating = 0,
            Status = ReviewStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            DeadlineUtc = deadlineUtc
        });
        var createdReview = _reviewLogic.ReadList(new ReviewSearchModel { PublicationId = publication.Id })?
            .Where(x => x.ReviewerId == reviewer.Id)
            .OrderByDescending(x => x.Id)
            .FirstOrDefault();
        if (createdReview is null)
        {
            return BadRequest("Не удалось назначить рецензента.");
        }

        var updated = new ReviewTaskModel(
            createdReview.Id,
            publication.Id,
            publication.Title,
            publication.Authors,
            publication.Publisher,
            ReviewWorkflowState.InReview,
            reviewer.Email,
            deadlineUtc ?? DateTime.UtcNow.AddDays(7),
            null,
            createdReview.CreatedAt);
        return Ok(MapReviewTaskForOutput(updated));
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReviewResult()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (TryForbiddenEmployeeApiResult(out var forbiddenResult))
        {
            return forbiddenResult;
        }

        var request = await TryReadRequestModelAsync<ReviewDecisionModel>();

        if (request is null)
        {
            return BadRequest("Тело запроса не передано.");
        }

        if (request.TaskId <= 0)
        {
            return BadRequest("TaskId обязателен.");
        }

        var review = _reviewLogic.ReadElement(new ReviewSearchModel { Id = request.TaskId });
        if (review is null)
        {
            return NotFound($"Задача рецензирования #{request.TaskId} не найдена.");
        }

        var status = request.Decision switch
        {
            ReviewWorkflowState.Approved => ReviewStatus.Confirmed,
            ReviewWorkflowState.Rejected => ReviewStatus.Rejected,
            _ => ReviewStatus.Pending
        };
        var updated = _reviewLogic.Update(new ReviewBindingModel
        {
            Id = review.Id,
            PublicationId = review.PublicationId,
            ReviewerId = review.ReviewerId,
            Content = string.IsNullOrWhiteSpace(request.Comment) ? review.Content : request.Comment.Trim(),
            Rating = review.Rating,
            Status = status,
            CreatedAt = review.CreatedAt,
            DeadlineUtc = review.DeadlineUtc,
            ConfirmedById = review.ConfirmedById
        });
        if (!updated)
        {
            return BadRequest("Не удалось сохранить решение по рецензии.");
        }

        var publication = _publicationLogic.ReadElement(new PublicationSearchModel { Id = review.PublicationId });
        var user = _userLogic.ReadElement(new UserSearchModel { Id = review.ReviewerId });

        if (publication is not null)
        {
            var automaticSummary = request.Decision switch
            {
                ReviewWorkflowState.Approved => "Одобрено",
                ReviewWorkflowState.RequiresRevision => "Требует доработки",
                ReviewWorkflowState.Rejected => "Отклонено",
                _ => "Без итоговой рецензии"
            };

            if (!string.IsNullOrWhiteSpace(request.Comment))
            {
                automaticSummary = $"{automaticSummary}: {request.Comment.Trim()}";
            }

            _publicationLogic.Update(new PublicationBindingModel
            {
                Id = publication.Id,
                Title = publication.Title,
                Authors = publication.Authors,
                Publisher = publication.Publisher,
                PublishDate = publication.PublishDate ?? DateTime.UtcNow,
                Description = automaticSummary,
                Volume = publication.Volume,
                SubjectText = publication.SubjectText
            });
        }

        var response = new ReviewTaskModel(
            review.Id,
            review.PublicationId,
            publication?.Title ?? $"Публикация #{review.PublicationId}",
            publication?.Authors ?? string.Empty,
            publication?.Publisher ?? string.Empty,
            request.Decision,
            user?.Email,
            review.DeadlineUtc,
            string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            review.CreatedAt);
        return Ok(MapReviewTaskForOutput(response));
    }

    private static DateTime? NormalizeUtcDateTime(DateTime? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var date = value.Value;
        return date.Kind switch
        {
            DateTimeKind.Utc => date,
            DateTimeKind.Local => date.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date, DateTimeKind.Utc)
        };
    }

    private async Task<T?> TryReadRequestModelAsync<T>() where T : class
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        options.Converters.Add(new JsonStringEnumConverter());

        if (Request.HasJsonContentType())
        {
            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            Request.Body.Position = 0;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    return JsonSerializer.Deserialize<T>(body, options);
                }
                catch
                {
                    return null;
                }
            }
        }

        var fromForm = await TryReadFromFormAsync<T>(options);
        if (fromForm is not null)
        {
            return fromForm;
        }

        if (Request.Query.Count == 0)
        {
            return null;
        }

        var queryMap = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in Request.Query)
        {
            queryMap[item.Key] = item.Value.Count > 1 ? item.Value.ToArray() : item.Value.ToString();
        }
        try
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(queryMap), options);
        }
        catch
        {
            return null;
        }
    }

    private async Task<T?> TryReadFromFormAsync<T>(JsonSerializerOptions options) where T : class
    {
        if (!Request.HasFormContentType)
        {
            return null;
        }

        var form = await Request.ReadFormAsync();
        if (form.Count == 0)
        {
            return null;
        }

        var map = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        foreach (var item in form)
        {
            map[item.Key] = item.Value.Count > 1 ? item.Value.ToArray() : item.Value.ToString();
        }

        try
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(map), options);
        }
        catch
        {
            return null;
        }
    }

    private async Task<(string FileName, string PublicUrl)> SaveReviewAttachmentAsync(int reviewId, IFormFile file)
    {
        var safeFileName = Path.GetFileName(file.FileName);
        var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "reviews", reviewId.ToString());
        Directory.CreateDirectory(uploadsRoot);
        var uniqueFileName = $"{Guid.NewGuid():N}_{safeFileName}";
        var physicalPath = Path.Combine(uploadsRoot, uniqueFileName);

        await using (var stream = System.IO.File.Create(physicalPath))
        {
            await file.CopyToAsync(stream);
        }

        var publicUrl = $"/uploads/reviews/{reviewId}/{uniqueFileName}";
        _attachmentLogic.Create(new AttachmentBindingModel
        {
            ReviewId = reviewId,
            FileName = safeFileName,
            MimeType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            SizeBytes = file.Length,
            StoragePath = publicUrl,
            UploadedAt = DateTime.UtcNow
        });

        return (safeFileName, publicUrl);
    }

    private object MapReviewTaskForOutput(ReviewTaskModel task) => new
    {
        task.Id,
        TaskId = task.Id,
        task.PublicationId,
        task.Title,
        task.AuthorFullName,
        task.PublicationType,
        task.ReviewerEmail,
        task.DeadlineUtc,
        task.EditorComment,
        task.CreatedUtc,
        StateCode = task.State.ToString(),
        State = ToRussianState(task.State)
    };

    [HttpGet("{reviewId:int}")]
    public IActionResult ReadAttachment(int reviewId)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        var attachment = (_attachmentLogic.ReadList(new AttachmentSearchModel { ReviewId = reviewId }) ?? new())
            .OrderByDescending(x => x.UploadedAt)
            .FirstOrDefault();
        if (attachment is null || string.IsNullOrWhiteSpace(attachment.StoragePath))
        {
            return NotFound("Файл не найден.");
        }

        var relativePath = attachment.StoragePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);
        if (!System.IO.File.Exists(fullPath))
        {
            return NotFound("Файл не найден в хранилище.");
        }

        var sourceBytes = System.IO.File.ReadAllBytes(fullPath);
        var mimeType = string.IsNullOrWhiteSpace(attachment.MimeType) ? "application/octet-stream" : attachment.MimeType;
        if (!mimeType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
        {
            return File(sourceBytes, mimeType, attachment.FileName);
        }

        var normalizedText = DecodeTextToUtf8(sourceBytes);
        var normalizedBytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true).GetBytes(normalizedText);
        return File(normalizedBytes, "text/plain; charset=utf-8", attachment.FileName);
    }

    private List<ReviewTaskModel> BuildReviewQueue(
        List<PublishingReviewContracts.ViewModels.PublicationViewModel> publications,
        List<PublishingReviewContracts.ViewModels.ReviewViewModel> reviews)
    {
        var users = _userLogic.ReadList(null)?.ToDictionary(x => x.Id, x => x.Email, EqualityComparer<int>.Default) ?? new Dictionary<int, string>();
        var result = new List<ReviewTaskModel>();

        foreach (var publication in publications)
        {
            var publicationReviews = reviews.Where(x => x.PublicationId == publication.Id).OrderByDescending(x => x.CreatedAt).ToList();
            if (publicationReviews.Count == 0)
            {
                result.Add(new ReviewTaskModel(
                    publication.Id,
                    publication.Id,
                    publication.Title,
                    publication.Authors,
                    publication.Publisher,
                    ReviewWorkflowState.WaitingForReviewer,
                    null,
                    null,
                    publication.Description,
                    publication.PublishDate ?? DateTime.UtcNow));
                continue;
            }

            foreach (var review in publicationReviews)
            {
                result.Add(new ReviewTaskModel(
                    review.Id,
                    publication.Id,
                    publication.Title,
                    publication.Authors,
                    publication.Publisher,
                    ResolveWorkflowState(review.Status, publication.Description, review.Content),
                    users.TryGetValue(review.ReviewerId, out var reviewerEmail) ? reviewerEmail : null,
                    review.DeadlineUtc,
                    publication.Description,
                    review.CreatedAt));
            }
        }

        return result;
    }

    private static string ToRussianState(ReviewWorkflowState state) => state switch
    {
        ReviewWorkflowState.WaitingForReviewer => "Ожидает назначения рецензента",
        ReviewWorkflowState.InReview => "В рецензировании",
        ReviewWorkflowState.RequiresRevision => "Требует доработки",
        ReviewWorkflowState.Approved => "Одобрено",
        ReviewWorkflowState.Rejected => "Отклонено",
        _ => "Не определён"
    };

    private static ReviewWorkflowState ResolveWorkflowState(ReviewStatus status, string? publicationDescription, string? reviewContent)
    {
        if (status == ReviewStatus.Confirmed)
        {
            return ReviewWorkflowState.Approved;
        }

        if (status == ReviewStatus.Rejected)
        {
            return ReviewWorkflowState.Rejected;
        }

        var text = $"{publicationDescription} {reviewContent}".ToLowerInvariant();
        if (Regex.IsMatch(text, @"\b(отклонен[аоы]?|отклонить|отклонено)\b"))
        {
            return ReviewWorkflowState.Rejected;
        }
        if (Regex.IsMatch(text, @"\bтребует\s+доработк"))
        {
            return ReviewWorkflowState.RequiresRevision;
        }

        return ReviewWorkflowState.InReview;
    }

    private static string DecodeTextToUtf8(byte[] sourceBytes)
    {
        try
        {
            return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true).GetString(sourceBytes);
        }
        catch (DecoderFallbackException)
        {
            return Encoding.GetEncoding(1251).GetString(sourceBytes);
        }
    }

    private string GetRoleDisplayName() => IsEmployee() ? "Сотрудник" : "Пользователь";

    private string? GetCurrentUserEmail() => HttpContext.Session.GetString(SessionEmailKey);

    private bool IsAuthenticated() =>
            bool.TryParse(HttpContext.Session.GetString(SessionAuthKey), out var isAuth) && isAuth;

    private bool IsEmployee() => string.Equals(HttpContext.Session.GetString(SessionRoleKey), "employee", StringComparison.OrdinalIgnoreCase);

    private bool IsUser() => string.Equals(HttpContext.Session.GetString(SessionRoleKey), "user", StringComparison.OrdinalIgnoreCase);

    private bool TryUnauthorizedPageResult(out IActionResult result)
    {
        if (IsAuthenticated())
        {
            result = default!;
            return false;
        }

        TempData["Error"] = "Сначала зарегистрируйтесь или войдите в систему.";
        result = RedirectToAction(nameof(Enter));
        return true;
    }

    private bool TryUnauthorizedApiResult(out IActionResult result)
    {
        if (IsAuthenticated())
        {
            result = default!;
            return false;
        }

        result = Unauthorized(new { message = "Требуется авторизация." });
        return true;
    }
    private bool TryForbiddenEmployeeResult(out IActionResult result)
    {
        if (IsEmployee())
        {
            result = default!;
            return false;
        }

        TempData["Error"] = "Раздел доступен только сотруднику.";
        result = RedirectToAction(nameof(DashboardPage));
        return true;
    }

    private bool TryForbiddenUserResult(out IActionResult result)
    {
        if (IsUser())
        {
            result = default!;
            return false;
        }

        TempData["Error"] = "Раздел доступен только пользователю.";
        result = RedirectToAction(nameof(DashboardPage));
        return true;
    }

    private bool TryForbiddenEmployeeApiResult(out IActionResult result)
    {
        if (IsEmployee())
        {
            result = default!;
            return false;
        }

        result = Forbid();
        return true;
    }

    private bool TryForbiddenUserApiResult(out IActionResult result)
    {
        if (IsUser())
        {
            result = default!;
            return false;
        }

        result = Forbid();
        return true;
    }
}
