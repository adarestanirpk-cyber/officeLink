namespace Application.DTOs;

public class InvokeProcessDto
{
    public string Domain { get; set; } = "";
    public string UserName { get; set; } = "";
    public int IdCase { get; set; }
    public int TaskId { get; set; }
}
