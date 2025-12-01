namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents an individual financial transaction
/// </summary>
public class Transaction : BaseEntity
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? NormalizedMerchant { get; set; }
    public string? Category { get; set; }
    public bool IsReconciled { get; set; } = false;
    public Guid? BillId { get; set; }
    public Guid? IncomeStreamId { get; set; }
    public Guid? GoalId { get; set; }
    public Guid? TransferAccountId { get; set; }
    public string? Notes { get; set; }
    public string? ExternalTransactionId { get; set; }

    // Navigation properties
    public Account Account { get; set; } = null!;
    public Bill? Bill { get; set; }
    public IncomeStream? IncomeStream { get; set; }
    public Goal? Goal { get; set; }
    public Account? TransferAccount { get; set; }
}
