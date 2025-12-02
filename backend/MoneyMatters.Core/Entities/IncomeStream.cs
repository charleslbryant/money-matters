using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents an expected income source
/// </summary>
public class IncomeStream : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TypicalAmount { get; set; }
    public IncomeFrequency Frequency { get; set; }
    public FinancialDomain Domain { get; set; }
    public Guid AccountId { get; set; }
    public DateTime? LastReceivedDate { get; set; }
    public decimal? LastReceivedAmount { get; set; }
    public DateTime? NextExpectedDate { get; set; }
    public DateTime? NextExpectedWindowStart { get; set; }
    public DateTime? NextExpectedWindowEnd { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Account Account { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
