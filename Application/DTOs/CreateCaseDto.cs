namespace Application.DTOs;

public class CreateCaseDto
{
    public string Domain { get; set; }
    public string UserName { get; set; }
    public string Process { get; set; }
    public Dictionary<string, Dictionary<string, string>> EntityFields { get; set; } = new Dictionary<string, Dictionary<string, string>>();
}
