using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents a recurring financial obligation
/// </summary>
public class Bill : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public BillFrequency Frequency { get; set; }
    public int? DayOfMonth { get; set; }
    public int? DayOfWeek { get; set; }
    public DateTime NextDueDate { get; set; }
    public FinancialDomain Domain { get; set; }
    public Guid? DefaultAccountId { get; set; }
    public int Priority { get; set; } = 5;
    public bool IsAutoPay { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Account? DefaultAccount { get; set; }
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
