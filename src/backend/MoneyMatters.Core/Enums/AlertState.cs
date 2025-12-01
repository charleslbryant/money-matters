namespace MoneyMatters.Core.Enums;

/// <summary>
/// Current state of an alert
/// </summary>
public enum AlertState
{
    New = 0,
    Acknowledged = 1,
    Snoozed = 2,
    Resolved = 3
}
