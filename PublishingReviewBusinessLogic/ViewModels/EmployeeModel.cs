namespace PublishingReviewBusinessLogic.ViewModels
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = "Editor"; // или ReviewerChecker
        public string Email { get; set; } = string.Empty;
    }
}
