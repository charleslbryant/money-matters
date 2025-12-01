using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents a savings goal or planned purchase
/// </summary>
public class Goal : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; } = 0;
    public DateTime TargetDate { get; set; }
    public FinancialDomain Domain { get; set; }
    public GoalFundingStrategy FundingStrategy { get; set; }
    public decimal? FixedContributionAmount { get; set; }
    public decimal? PercentOfIncome { get; set; }
    public int Priority { get; set; } = 5;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<GoalAccount> GoalAccounts { get; set; } = new List<GoalAccount>();
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
