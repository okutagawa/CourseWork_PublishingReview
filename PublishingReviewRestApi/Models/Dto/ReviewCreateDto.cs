public class ReviewCreateDto
{
    public int PublicationId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool Approved { get; set; }
}
