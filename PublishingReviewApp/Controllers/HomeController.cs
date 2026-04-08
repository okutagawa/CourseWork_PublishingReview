using System.Text;
using Microsoft.AspNetCore.Mvc;
using PublishingReviewApp.Models;

namespace PublishingReviewApp.Controllers;

[Route("[controller]/[action]")]
public class HomeController : Controller
{
    private const string SessionAuthKey = "IsAuthenticated";
    private const string SessionEmailKey = "UserEmail";
    private const string SessionFullNameKey = "UserFullName";
    private const string SessionRoleKey = "UserRole";

    private static readonly object SyncRoot = new();

    private static readonly List<UserAccountModel> Users =
    [
        new UserAccountModel("Главный редактор", "editor@publisher.local", "editor123", "editor", "Редакция"),
        new UserAccountModel("Тестовый рецензент", "reviewer@publisher.local", "reviewer123", "reviewer", "Внешний эксперт"),
        new UserAccountModel("Тестовый автор", "author@publisher.local", "author123", "author", "Университет")
    ];


    private static readonly List<ReviewTaskModel> ReviewQueue =
    [
        new ReviewTaskModel(1, 101, "Методы автоматической вёрстки", "И.И. Иванов", "Научная статья", ReviewWorkflowState.WaitingForReviewer, null, null, null, DateTime.UtcNow.AddDays(-8)),
        new ReviewTaskModel(2, 102, "Редакционный цикл издательства", "П.П. Петров", "Монография", ReviewWorkflowState.InReview, "expert@publisher.local", DateTime.UtcNow.AddDays(5), null, DateTime.UtcNow.AddDays(-4)),
        new ReviewTaskModel(3, 103, "Проверка корректуры", "А.А. Сидоров", "Учебное пособие", ReviewWorkflowState.RequiresRevision, "reviewer@publisher.local", DateTime.UtcNow.AddDays(-2), "Нужно доработать ссылки и библиографию", DateTime.UtcNow.AddDays(-2))
    ];

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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

        UserAccountModel? user;
        lock (SyncRoot)
        {
            user = Users.FirstOrDefault(x =>
                x.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase)
                && x.Password == request.Password);
        }

        if (user is null)
        {
            TempData["Error"] = "Неверный email или пароль.";
            return RedirectToAction(nameof(Enter));
        }

        HttpContext.Session.SetString(SessionAuthKey, bool.TrueString);
        HttpContext.Session.SetString(SessionEmailKey, user.Email);
        HttpContext.Session.SetString(SessionFullNameKey, user.FullName);
        HttpContext.Session.SetString(SessionRoleKey, user.Role);

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

        var allowedRoles = new[] { "author", "reviewer", "editor" };
        var normalizedRole = request.Role.Trim().ToLowerInvariant();
        if (!allowedRoles.Contains(normalizedRole))
        {
            TempData["Error"] = "Некорректная роль пользователя.";
            return RedirectToAction(nameof(Register));
        }

        lock (SyncRoot)
        {
            if (Users.Any(x => x.Email.Equals(request.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Error"] = "Пользователь с таким email уже зарегистрирован.";
                return RedirectToAction(nameof(Register));
            }

            Users.Add(new UserAccountModel(
                request.FullName.Trim(),
                request.Email.Trim(),
                request.Password,
                normalizedRole,
                request.Organization?.Trim()));
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
        ViewBag.UserRole = HttpContext.Session.GetString(SessionRoleKey);

        return View();
    }

    [HttpGet]
    public IActionResult DashboardPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        lock (SyncRoot)
        {
            ViewBag.TotalPublications = ReviewQueue.Select(x => x.PublicationId).Distinct().Count();
            ViewBag.WaitingForReviewer = ReviewQueue.Count(x => x.State == ReviewWorkflowState.WaitingForReviewer);
            ViewBag.InReview = ReviewQueue.Count(x => x.State == ReviewWorkflowState.InReview);
            ViewBag.RequiresRevision = ReviewQueue.Count(x => x.State == ReviewWorkflowState.RequiresRevision);
            ViewBag.Approved = ReviewQueue.Count(x => x.State == ReviewWorkflowState.Approved);
            ViewBag.Rejected = ReviewQueue.Count(x => x.State == ReviewWorkflowState.Rejected);
        }

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
    public IActionResult CreatePublicationPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
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

        return View();
    }

    [HttpGet]
    public IActionResult SubmitDecisionPage()
    {
        if (TryUnauthorizedPageResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
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

        return View();
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        lock (SyncRoot)
        {
            var model = new ReviewDashboardModel
            {
                TotalPublications = ReviewQueue.Select(x => x.PublicationId).Distinct().Count(),
                WaitingForReviewer = ReviewQueue.Count(x => x.State == ReviewWorkflowState.WaitingForReviewer),
                InReview = ReviewQueue.Count(x => x.State == ReviewWorkflowState.InReview),
                RequiresRevision = ReviewQueue.Count(x => x.State == ReviewWorkflowState.RequiresRevision),
                Approved = ReviewQueue.Count(x => x.State == ReviewWorkflowState.Approved),
                Rejected = ReviewQueue.Count(x => x.State == ReviewWorkflowState.Rejected),
                NearestDeadlines = ReviewQueue
                    .Where(x => x.DeadlineUtc.HasValue)
                    .OrderBy(x => x.DeadlineUtc)
                    .Take(5)
                    .ToList()
            };

            return Ok(model);
        }
    }

    [HttpGet]
    public IActionResult ReviewQueueList([FromQuery] ReviewWorkflowState? state)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        lock (SyncRoot)
        {
            var query = ReviewQueue.AsEnumerable();
            if (state.HasValue)
            {
                query = query.Where(x => x.State == state.Value);
            }

            return Ok(query.OrderBy(x => x.State).ThenBy(x => x.DeadlineUtc ?? DateTime.MaxValue).ToList());
        }
    }

    [HttpGet]
    public IActionResult ReportData([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] string format = "json")
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value.Date > dateTo.Value.Date)
        {
            return BadRequest("Дата начала периода не может быть позже даты окончания.");
        }

        lock (SyncRoot)
        {
            var from = dateFrom?.Date;
            var to = dateTo?.Date.AddDays(1).AddTicks(-1);

            var data = ReviewQueue
                .Where(x => !from.HasValue || x.CreatedUtc >= from.Value)
                .Where(x => !to.HasValue || x.CreatedUtc <= to.Value)
                .OrderByDescending(x => x.CreatedUtc)
                .ToList();

            if (string.Equals(format, "csv", StringComparison.OrdinalIgnoreCase))
            {
                var csv = new StringBuilder();
                csv.AppendLine("TaskId;PublicationId;Title;Author;Type;State;Reviewer;DeadlineUtc;CreatedUtc");
                foreach (var item in data)
                {
                    csv.AppendLine($"{item.Id};{item.PublicationId};{item.Title};{item.AuthorFullName};{item.PublicationType};{item.State};{item.ReviewerEmail ?? string.Empty};{item.DeadlineUtc:O};{item.CreatedUtc:O}");
                }

                var bytes = Encoding.UTF8.GetBytes(csv.ToString());
                return File(bytes, "text/csv", $"review-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
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

            return Ok(new { summary, items = data });
        }
    }

    [HttpPost]
    public IActionResult CreatePublicationForReview([FromBody] PublicationReviewCreateModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.AuthorFullName) || string.IsNullOrWhiteSpace(request.PublicationType))
        {
            return BadRequest("Не заполнены обязательные поля: Title, AuthorFullName, PublicationType.");
        }

        lock (SyncRoot)
        {
            var nextId = ReviewQueue.Count == 0 ? 1 : ReviewQueue.Max(x => x.Id) + 1;
            var nextPublicationId = ReviewQueue.Count == 0 ? 100 : ReviewQueue.Max(x => x.PublicationId) + 1;

            var item = new ReviewTaskModel(
                nextId,
                nextPublicationId,
                request.Title.Trim(),
                request.AuthorFullName.Trim(),
                request.PublicationType.Trim(),
                ReviewWorkflowState.WaitingForReviewer,
                null,
                null,
                request.EditorComment?.Trim(),
                DateTime.UtcNow);

            ReviewQueue.Add(item);

            _logger.LogInformation("Publication {PublicationId} created and added to review queue", nextPublicationId);
            return CreatedAtAction(nameof(GetReviewTaskById), new { id = item.Id }, item);
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetReviewTaskById(int id)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        lock (SyncRoot)
        {
            var item = ReviewQueue.FirstOrDefault(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }
    }

    [HttpPost]
    public IActionResult AssignReviewer([FromBody] ReviewerAssignmentModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (request.TaskId <= 0 || string.IsNullOrWhiteSpace(request.ReviewerEmail))
        {
            return BadRequest("TaskId и ReviewerEmail обязательны.");
        }

        lock (SyncRoot)
        {
            var idx = ReviewQueue.FindIndex(x => x.Id == request.TaskId);
            if (idx < 0)
            {
                return NotFound($"Задача рецензирования #{request.TaskId} не найдена.");
            }

            var old = ReviewQueue[idx];
            var updated = old with
            {
                ReviewerEmail = request.ReviewerEmail.Trim(),
                DeadlineUtc = request.DeadlineUtc ?? DateTime.UtcNow.AddDays(7),
                State = ReviewWorkflowState.InReview
            };

            ReviewQueue[idx] = updated;
            return Ok(updated);
        }
    }

    [HttpPost]
    public IActionResult SubmitReviewResult([FromBody] ReviewDecisionModel request)
    {
        if (TryUnauthorizedApiResult(out var unauthorizedResult))
        {
            return unauthorizedResult;
        }

        if (request.TaskId <= 0)
        {
            return BadRequest("TaskId обязателен.");
        }

        lock (SyncRoot)
        {
            var idx = ReviewQueue.FindIndex(x => x.Id == request.TaskId);
            if (idx < 0)
            {
                return NotFound($"Задача рецензирования #{request.TaskId} не найдена.");
            }

            var old = ReviewQueue[idx];
            var updated = old with
            {
                State = request.Decision,
                EditorComment = string.IsNullOrWhiteSpace(request.Comment) ? old.EditorComment : request.Comment.Trim()
            };

            ReviewQueue[idx] = updated;
            return Ok(updated);
        }
    }
    private bool IsAuthenticated() =>
            bool.TryParse(HttpContext.Session.GetString(SessionAuthKey), out var isAuth) && isAuth;

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
}