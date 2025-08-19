namespace Domain.Entities;

public class ProcessMetaData
{
    public List<IncidentDetails> IncidentDetails { get; set; } = new List<IncidentDetails>();
    public Retry Retry { get; set; } = new Retry();
}

