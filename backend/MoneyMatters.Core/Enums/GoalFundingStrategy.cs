namespace MoneyMatters.Core.Enums;

/// <summary>
/// Strategy for funding savings goals
/// </summary>
public enum GoalFundingStrategy
{
    /// <summary>
    /// Contribute a fixed amount per period
    /// </summary>
    FixedAmount = 0,

    /// <summary>
    /// Contribute a percentage of income
    /// </summary>
    PercentOfIncome = 1,

    /// <summary>
    /// Contribute excess funds after bills and minimums
    /// </summary>
    Surplus = 2
}
