using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// System-generated alerts for cash shortfalls, bill risks, etc.
/// </summary>
public class Alert : BaseEntity
{
    public Guid UserId { get; set; }
    public AlertType Type { get; set; }
    public AlertSeverity Severity { get; set; }
    public AlertState State { get; set; } = AlertState.New;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? RecommendedAction { get; set; }
    public FinancialDomain? Domain { get; set; }
    public Guid? RelatedAccountId { get; set; }
    public Guid? RelatedBillId { get; set; }
    public Guid? RelatedGoalId { get; set; }
    public Guid? RelatedIncomeStreamId { get; set; }
    public DateTime TriggeredAt { get; set; }
    public DateTime? AcknowledgedAt { get; set; }
    public DateTime? SnoozedUntil { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Account? RelatedAccount { get; set; }
    public Bill? RelatedBill { get; set; }
    public Goal? RelatedGoal { get; set; }
    public IncomeStream? RelatedIncomeStream { get; set; }
}
