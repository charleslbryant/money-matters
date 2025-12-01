namespace MoneyMatters.Core.Entities;

/// <summary>
/// User-specific application settings and preferences
/// </summary>
public class Setting : BaseEntity
{
    public Guid UserId { get; set; }
    public string SettingKey { get; set; } = string.Empty;
    public string SettingValue { get; set; } = string.Empty;

    // Navigation properties
    public User User { get; set; } = null!;
}
