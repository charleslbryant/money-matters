using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents a financial account (checking, savings, credit card, etc.)
/// </summary>
public class Account : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Institution { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public FinancialDomain Domain { get; set; }
    public decimal CurrentBalance { get; set; }
    public decimal SafeMinimumBalance { get; set; } = 0;
    public bool IncludeInForecast { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public string? ExternalAccountId { get; set; }
    public DateTime? LastSyncedAt { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<IncomeStream> IncomeStreams { get; set; } = new List<IncomeStream>();
    public ICollection<GoalAccount> GoalAccounts { get; set; } = new List<GoalAccount>();
}
