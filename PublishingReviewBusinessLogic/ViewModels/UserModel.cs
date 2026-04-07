namespace PublishingReviewBusinessLogic.ViewModels
{
    public class UserModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Роль пользователя: обычный пользователь, сотрудник, админ
        public string Role { get; set; } = "User";
    }
}
