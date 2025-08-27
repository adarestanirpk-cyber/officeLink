namespace Domain.Enums;

public enum TransitionType
{
    Normal = 1,
    Exception = 2,
    Compensation = 3,
    Cancel = 4,
    Error = 5,
    Relation = 6,
    TimerEvent = 7,
    Event = 8,
    MessageEvent = 9,
    SignalEvent = 10
}
