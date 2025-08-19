namespace Application.Interfaces;

public interface ICorrelationIdProvider
{
    string CorrelationId { get; }
}
