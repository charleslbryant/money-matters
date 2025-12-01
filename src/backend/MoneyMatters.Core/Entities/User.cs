namespace MoneyMatters.Core.Entities;

/// <summary>
/// Represents a user of the Money Matters application
/// </summary>
public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "America/New_York";
    public int DefaultForecastHorizonDays { get; set; } = 30;

    // Navigation properties
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<IncomeStream> IncomeStreams { get; set; } = new List<IncomeStream>();
    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    public ICollection<ForecastSnapshot> ForecastSnapshots { get; set; } = new List<ForecastSnapshot>();
    public ICollection<Setting> Settings { get; set; } = new List<Setting>();
}
