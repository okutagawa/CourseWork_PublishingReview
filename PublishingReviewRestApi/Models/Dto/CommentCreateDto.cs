public class CommentCreateDto
{
    public int ReviewId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = string.Empty;
}
