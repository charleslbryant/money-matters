using MoneyMatters.Core.Enums;

namespace MoneyMatters.Core.Entities;

/// <summary>
/// Stores cached forecast calculations for performance
/// </summary>
public class ForecastSnapshot : BaseEntity
{
    public Guid UserId { get; set; }
    public FinancialDomain Domain { get; set; }
    public int HorizonDays { get; set; }
    public DateTime GeneratedAt { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ForecastData { get; set; } = string.Empty;
    public int? RunwayDays { get; set; }
    public StatusIndicator Status { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
