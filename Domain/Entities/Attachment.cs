namespace Domain.Entities;

public class Attachment
{
    public string FileId { get; set; } = "";
    public string FileName { get; set; } = "";
    public string ContentType { get; set; } = "";
    public string Hash { get; set; } = "";
}
