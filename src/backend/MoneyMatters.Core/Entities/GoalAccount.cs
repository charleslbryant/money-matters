namespace MoneyMatters.Core.Entities;

/// <summary>
/// Join table linking Goals to source Accounts
/// </summary>
public class GoalAccount : BaseEntity
{
    public Guid GoalId { get; set; }
    public Guid AccountId { get; set; }

    // Navigation properties
    public Goal Goal { get; set; } = null!;
    public Account Account { get; set; } = null!;
}
