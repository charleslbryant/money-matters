using MoneyMatters.Core.Entities;
using MoneyMatters.Core.Enums;

namespace MoneyMatters.Infrastructure.Data;

/// <summary>
/// Provides seed data for development database
/// </summary>
public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (context.Users.Any())
        {
            return; // Database has been seeded
        }

        // Create development user
        var user = new User
        {
            Email = "dev@moneymatters.local",
            Name = "Development User",
            TimeZone = "America/New_York",
            DefaultForecastHorizonDays = 30
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Create accounts
        var personalChecking = new Account
        {
            UserId = user.Id,
            Name = "Personal Checking",
            Institution = "Chase Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 5000m,
            SafeMinimumBalance = 1000m,
            IncludeInForecast = true,
            IsActive = true
        };

        var personalSavings = new Account
        {
            UserId = user.Id,
            Name = "Personal Savings",
            Institution = "Chase Bank",
            AccountType = "Savings",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 15000m,
            SafeMinimumBalance = 5000m,
            IncludeInForecast = true,
            IsActive = true
        };

        var businessChecking = new Account
        {
            UserId = user.Id,
            Name = "Business Checking",
            Institution = "Bank of America",
            AccountType = "Checking",
            Domain = FinancialDomain.Business,
            CurrentBalance = 25000m,
            SafeMinimumBalance = 10000m,
            IncludeInForecast = true,
            IsActive = true
        };

        var businessSavings = new Account
        {
            UserId = user.Id,
            Name = "Business Savings",
            Institution = "Bank of America",
            AccountType = "Savings",
            Domain = FinancialDomain.Business,
            CurrentBalance = 50000m,
            SafeMinimumBalance = 20000m,
            IncludeInForecast = true,
            IsActive = true
        };

        var creditCard = new Account
        {
            UserId = user.Id,
            Name = "Credit Card",
            Institution = "Chase",
            AccountType = "Credit Card",
            Domain = FinancialDomain.Personal,
            CurrentBalance = -2500m,
            SafeMinimumBalance = 0m,
            IncludeInForecast = true,
            IsActive = true
        };

        context.Accounts.AddRange(personalChecking, personalSavings, businessChecking, businessSavings, creditCard);
        await context.SaveChangesAsync();

        // Create bills
        var rent = new Bill
        {
            UserId = user.Id,
            Name = "Rent",
            Amount = 2000m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 1,
            NextDueDate = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1), DateTimeKind.Utc),
            Domain = FinancialDomain.Personal,
            DefaultAccountId = personalChecking.Id,
            Priority = 1,
            IsAutoPay = false,
            IsActive = true
        };

        var electric = new Bill
        {
            UserId = user.Id,
            Name = "Electric Bill",
            Amount = 150m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 15,
            NextDueDate = GetNextDueDate(15),
            Domain = FinancialDomain.Personal,
            DefaultAccountId = personalChecking.Id,
            Priority = 2,
            IsAutoPay = true,
            IsActive = true
        };

        var internet = new Bill
        {
            UserId = user.Id,
            Name = "Internet",
            Amount = 80m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 10,
            NextDueDate = GetNextDueDate(10),
            Domain = FinancialDomain.Personal,
            DefaultAccountId = personalChecking.Id,
            Priority = 2,
            IsAutoPay = true,
            IsActive = true
        };

        var phone = new Bill
        {
            UserId = user.Id,
            Name = "Phone Bill",
            Amount = 60m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 5,
            NextDueDate = GetNextDueDate(5),
            Domain = FinancialDomain.Personal,
            DefaultAccountId = personalChecking.Id,
            Priority = 3,
            IsAutoPay = true,
            IsActive = true
        };

        var saasSubscriptions = new Bill
        {
            UserId = user.Id,
            Name = "SaaS Subscriptions",
            Amount = 200m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 1,
            NextDueDate = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1), DateTimeKind.Utc),
            Domain = FinancialDomain.Business,
            DefaultAccountId = businessChecking.Id,
            Priority = 3,
            IsAutoPay = true,
            IsActive = true
        };

        var officeRent = new Bill
        {
            UserId = user.Id,
            Name = "Office Rent",
            Amount = 1500m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 1,
            NextDueDate = DateTime.SpecifyKind(new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).AddMonths(1), DateTimeKind.Utc),
            Domain = FinancialDomain.Business,
            DefaultAccountId = businessChecking.Id,
            Priority = 1,
            IsAutoPay = false,
            IsActive = true
        };

        context.Bills.AddRange(rent, electric, internet, phone, saasSubscriptions, officeRent);
        await context.SaveChangesAsync();

        // Create income streams
        var salary = new IncomeStream
        {
            UserId = user.Id,
            Name = "Salary",
            TypicalAmount = 6000m,
            Frequency = IncomeFrequency.SemiMonthly,
            Domain = FinancialDomain.Personal,
            AccountId = personalChecking.Id,
            NextExpectedDate = GetNextSemiMonthlyPayday(),
            NextExpectedWindowStart = GetNextSemiMonthlyPayday().AddDays(-2),
            NextExpectedWindowEnd = GetNextSemiMonthlyPayday().AddDays(2),
            IsActive = true
        };

        var clientRevenue = new IncomeStream
        {
            UserId = user.Id,
            Name = "Client Revenue",
            TypicalAmount = 10000m,
            Frequency = IncomeFrequency.Irregular,
            Domain = FinancialDomain.Business,
            AccountId = businessChecking.Id,
            IsActive = true
        };

        context.IncomeStreams.AddRange(salary, clientRevenue);
        await context.SaveChangesAsync();

        // Create goals
        var emergencyFund = new Goal
        {
            UserId = user.Id,
            Name = "Emergency Fund",
            TargetAmount = 20000m,
            CurrentAmount = 15000m,
            TargetDate = DateTime.UtcNow.AddMonths(12),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            FixedContributionAmount = 500m,
            Priority = 1,
            IsActive = true
        };

        var newLaptop = new Goal
        {
            UserId = user.Id,
            Name = "New Laptop",
            TargetAmount = 3000m,
            CurrentAmount = 1000m,
            TargetDate = DateTime.UtcNow.AddDays(90),
            Domain = FinancialDomain.Business,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            FixedContributionAmount = 300m,
            Priority = 2,
            IsActive = true
        };

        var vacation = new Goal
        {
            UserId = user.Id,
            Name = "Vacation Fund",
            TargetAmount = 5000m,
            CurrentAmount = 1500m,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.Surplus,
            Priority = 3,
            IsActive = true
        };

        context.Goals.AddRange(emergencyFund, newLaptop, vacation);
        await context.SaveChangesAsync();

        // Link goals to accounts
        var emergencyFundAccount = new GoalAccount
        {
            GoalId = emergencyFund.Id,
            AccountId = personalSavings.Id
        };

        var laptopAccount = new GoalAccount
        {
            GoalId = newLaptop.Id,
            AccountId = businessSavings.Id
        };

        var vacationAccount = new GoalAccount
        {
            GoalId = vacation.Id,
            AccountId = personalSavings.Id
        };

        context.GoalAccounts.AddRange(emergencyFundAccount, laptopAccount, vacationAccount);
        await context.SaveChangesAsync();

        // Create sample transactions (last 30 days)
        var transactions = new List<Transaction>();
        var now = DateTime.UtcNow;

        // Salary deposits (2 in last 30 days)
        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = 6000m,
            Date = now.AddDays(-15),
            Description = "Salary Deposit",
            Category = "Income",
            IncomeStreamId = salary.Id,
            IsReconciled = true
        });

        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = 6000m,
            Date = now.AddDays(-30),
            Description = "Salary Deposit",
            Category = "Income",
            IncomeStreamId = salary.Id,
            IsReconciled = true
        });

        // Bill payments
        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = -2000m,
            Date = now.AddDays(-25),
            Description = "Rent Payment",
            Category = "Housing",
            BillId = rent.Id,
            IsReconciled = true
        });

        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = -150m,
            Date = now.AddDays(-12),
            Description = "Electric Bill",
            Category = "Utilities",
            BillId = electric.Id,
            IsReconciled = true
        });

        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = -80m,
            Date = now.AddDays(-18),
            Description = "Internet Bill",
            Category = "Utilities",
            BillId = internet.Id,
            IsReconciled = true
        });

        // Goal contributions
        transactions.Add(new Transaction
        {
            AccountId = personalSavings.Id,
            Amount = 500m,
            Date = now.AddDays(-15),
            Description = "Emergency Fund Contribution",
            Category = "Savings",
            GoalId = emergencyFund.Id,
            IsReconciled = true
        });

        transactions.Add(new Transaction
        {
            AccountId = businessSavings.Id,
            Amount = 300m,
            Date = now.AddDays(-10),
            Description = "Laptop Fund Contribution",
            Category = "Savings",
            GoalId = newLaptop.Id,
            IsReconciled = true
        });

        // Miscellaneous expenses
        transactions.Add(new Transaction
        {
            AccountId = personalChecking.Id,
            Amount = -120m,
            Date = now.AddDays(-5),
            Description = "Grocery Store",
            Category = "Food & Dining",
            IsReconciled = true
        });

        transactions.Add(new Transaction
        {
            AccountId = creditCard.Id,
            Amount = -75m,
            Date = now.AddDays(-3),
            Description = "Gas Station",
            Category = "Transportation",
            IsReconciled = true
        });

        context.Transactions.AddRange(transactions);
        await context.SaveChangesAsync();

        // Create sample settings
        var settings = new List<Setting>
        {
            new Setting
            {
                UserId = user.Id,
                SettingKey = "AlertThreshold.CashShortfall",
                SettingValue = "7"
            },
            new Setting
            {
                UserId = user.Id,
                SettingKey = "AlertThreshold.BillRisk",
                SettingValue = "5"
            },
            new Setting
            {
                UserId = user.Id,
                SettingKey = "AlertChannels.Email",
                SettingValue = "true"
            },
            new Setting
            {
                UserId = user.Id,
                SettingKey = "AlertChannels.InApp",
                SettingValue = "true"
            },
            new Setting
            {
                UserId = user.Id,
                SettingKey = "DashboardRefreshTime",
                SettingValue = "08:00"
            }
        };

        context.Settings.AddRange(settings);
        await context.SaveChangesAsync();
    }

    private static DateTime GetNextDueDate(int dayOfMonth)
    {
        var now = DateTime.UtcNow;
        var dueDate = new DateTime(now.Year, now.Month, Math.Min(dayOfMonth, DateTime.DaysInMonth(now.Year, now.Month)), 0, 0, 0, DateTimeKind.Utc);

        if (dueDate <= now)
        {
            dueDate = dueDate.AddMonths(1);
            dueDate = new DateTime(dueDate.Year, dueDate.Month, Math.Min(dayOfMonth, DateTime.DaysInMonth(dueDate.Year, dueDate.Month)), 0, 0, 0, DateTimeKind.Utc);
        }

        return dueDate;
    }

    private static DateTime GetNextSemiMonthlyPayday()
    {
        var now = DateTime.UtcNow;
        var day = now.Day;

        if (day < 15)
        {
            return new DateTime(now.Year, now.Month, 15, 0, 0, 0, DateTimeKind.Utc);
        }
        else if (day < DateTime.DaysInMonth(now.Year, now.Month))
        {
            return new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 0, 0, 0, DateTimeKind.Utc);
        }
        else
        {
            var nextMonth = now.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 15, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
