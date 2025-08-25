namespace Domain.ValueObjects;

public class Retry
{
    public int RetryAfterMinutes { get; set; }
    public int RetryCount { get; set; }
    public string RetryStrategy { get; set; } = string.Empty;
    public DateTime ResponseDeadline { get; set; }
}
