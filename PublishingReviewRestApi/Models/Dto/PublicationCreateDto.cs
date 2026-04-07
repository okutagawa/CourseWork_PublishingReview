public class PublicationCreateDto
{
    public string Name { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public DateTime? Date { get; set; }
    public int Volume { get; set; }
}
