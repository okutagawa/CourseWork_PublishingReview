using Microsoft.AspNetCore.Mvc;
using PublishingReviewApp.Models;
using PublishingReviewDataModels.Enums;

namespace PublishingReviewApp.Controllers;

[Route("[controller]/[action]")]
public class HomeController : Controller
{
    private static readonly object SyncRoot = new();

    private static readonly List<ReviewTaskModel> ReviewQueue =
    [
        new ReviewTaskModel(1, 101, "Методы автоматической вёрстки", "И.И. Иванов", "Научная статья", ReviewWorkflowState.WaitingForReviewer, null, null, null),
        new ReviewTaskModel(2, 102, "Редакционный цикл издательства", "П.П. Петров", "Монография", ReviewWorkflowState.InReview, "expert@publisher.local", DateTime.UtcNow.AddDays(5), null),
        new ReviewTaskModel(3, 103, "Проверка корректуры", "А.А. Сидоров", "Учебное пособие", ReviewWorkflowState.RequiresRevision, "reviewer@publisher.local", DateTime.UtcNow.AddDays(-2), "Нужно доработать ссылки и библиографию")
    ];

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet]
    public IActionResult Dashboard()
    {
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

    [HttpPost]
    public IActionResult CreatePublicationForReview([FromBody] PublicationReviewCreateModel request)
    {
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
                request.EditorComment?.Trim());

            ReviewQueue.Add(item);

            _logger.LogInformation("Publication {PublicationId} created and added to review queue", nextPublicationId);
            return CreatedAtAction(nameof(GetReviewTaskById), new { id = item.Id }, item);
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetReviewTaskById(int id)
    {
        lock (SyncRoot)
        {
            var item = ReviewQueue.FirstOrDefault(x => x.Id == id);
            return item is null ? NotFound() : Ok(item);
        }
    }

    [HttpPost]
    public IActionResult AssignReviewer([FromBody] ReviewerAssignmentModel request)
    {
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
}