using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyMatters.Core.Enums;
using MoneyMatters.Infrastructure.Data;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

/// <summary>
/// Tests to validate seed data populates correctly with expected counts and relationships.
/// Verifies the development database seed data matches the expected specification.
/// </summary>
public class SeedDataValidationTests : DatabaseTestBase
{
    public SeedDataValidationTests()
    {
        // Seed data for these tests
        SeedData.SeedAsync(Context).Wait();
    }

    [Fact]
    public async Task SeedData_CreatesExactlyOneUser()
    {
        // Act
        var userCount = await Context.Users.CountAsync();
        var user = await Context.Users.FirstOrDefaultAsync();

        // Assert
        userCount.Should().Be(1);
        user.Should().NotBeNull();
        user!.Email.Should().Be("dev@moneymatters.local");
        user.Name.Should().Be("Development User");
        user.TimeZone.Should().Be("America/New_York");
        user.DefaultForecastHorizonDays.Should().Be(30);
    }

    [Fact]
    public async Task SeedData_CreatesExactlyFiveAccounts()
    {
        // Act
        var accountCount = await Context.Accounts.CountAsync();
        var accounts = await Context.Accounts.ToListAsync();

        // Assert
        accountCount.Should().Be(5);

        var personalCheckingCount = accounts.Count(a => a.Name == "Personal Checking" && a.Domain == FinancialDomain.Personal);
        var personalSavingsCount = accounts.Count(a => a.Name == "Personal Savings" && a.Domain == FinancialDomain.Personal);
        var businessCheckingCount = accounts.Count(a => a.Name == "Business Checking" && a.Domain == FinancialDomain.Business);
        var businessSavingsCount = accounts.Count(a => a.Name == "Business Savings" && a.Domain == FinancialDomain.Business);
        var creditCardCount = accounts.Count(a => a.Name == "Credit Card" && a.AccountType == "Credit Card" && a.Domain == FinancialDomain.Personal);

        personalCheckingCount.Should().Be(1);
        personalSavingsCount.Should().Be(1);
        businessCheckingCount.Should().Be(1);
        businessSavingsCount.Should().Be(1);
        creditCardCount.Should().Be(1);
    }

    [Fact]
    public async Task SeedData_AccountsHaveCorrectDomainDistribution()
    {
        // Act
        var personalAccounts = await Context.Accounts
            .Where(a => a.Domain == FinancialDomain.Personal)
            .CountAsync();
        var businessAccounts = await Context.Accounts
            .Where(a => a.Domain == FinancialDomain.Business)
            .CountAsync();

        // Assert
        personalAccounts.Should().Be(3); // Personal Checking, Personal Savings, Credit Card
        businessAccounts.Should().Be(2); // Business Checking, Business Savings
    }

    [Fact]
    public async Task SeedData_CreatesExactlySixBills()
    {
        // Act
        var billCount = await Context.Bills.CountAsync();
        var bills = await Context.Bills.ToListAsync();

        // Assert
        billCount.Should().Be(6);

        var billNames = bills.Select(b => b.Name).ToList();
        billNames.Should().Contain("Rent");
        billNames.Should().Contain("Electric Bill");
        billNames.Should().Contain("Internet");
        billNames.Should().Contain("Phone Bill");
        billNames.Should().Contain("SaaS Subscriptions");
        billNames.Should().Contain("Office Rent");
    }

    [Fact]
    public async Task SeedData_BillsHaveCorrectDomainDistribution()
    {
        // Act
        var personalBills = await Context.Bills
            .Where(b => b.Domain == FinancialDomain.Personal)
            .CountAsync();
        var businessBills = await Context.Bills
            .Where(b => b.Domain == FinancialDomain.Business)
            .CountAsync();

        // Assert
        personalBills.Should().Be(4); // Rent, Electric, Internet, Phone
        businessBills.Should().Be(2); // SaaS, Office Rent
    }

    [Fact]
    public async Task SeedData_CreatesExactlyTwoIncomeStreams()
    {
        // Act
        var incomeStreamCount = await Context.IncomeStreams.CountAsync();
        var incomeStreams = await Context.IncomeStreams.ToListAsync();

        // Assert
        incomeStreamCount.Should().Be(2);

        var salary = incomeStreams.FirstOrDefault(i => i.Name == "Salary");
        var clientRevenue = incomeStreams.FirstOrDefault(i => i.Name == "Client Revenue");

        salary.Should().NotBeNull();
        salary!.TypicalAmount.Should().Be(6000m);
        salary.Frequency.Should().Be(IncomeFrequency.SemiMonthly);
        salary.Domain.Should().Be(FinancialDomain.Personal);

        clientRevenue.Should().NotBeNull();
        clientRevenue!.TypicalAmount.Should().Be(10000m);
        clientRevenue.Frequency.Should().Be(IncomeFrequency.Irregular);
        clientRevenue.Domain.Should().Be(FinancialDomain.Business);
    }

    [Fact]
    public async Task SeedData_CreatesExactlyThreeGoals()
    {
        // Act
        var goalCount = await Context.Goals.CountAsync();
        var goals = await Context.Goals.ToListAsync();

        // Assert
        goalCount.Should().Be(3);

        var emergencyFund = goals.FirstOrDefault(g => g.Name == "Emergency Fund");
        var newLaptop = goals.FirstOrDefault(g => g.Name == "New Laptop");
        var vacation = goals.FirstOrDefault(g => g.Name == "Vacation Fund");

        emergencyFund.Should().NotBeNull();
        emergencyFund!.TargetAmount.Should().Be(20000m);
        emergencyFund.CurrentAmount.Should().Be(15000m);
        emergencyFund.Domain.Should().Be(FinancialDomain.Personal);
        emergencyFund.FundingStrategy.Should().Be(GoalFundingStrategy.FixedAmount);

        newLaptop.Should().NotBeNull();
        newLaptop!.TargetAmount.Should().Be(3000m);
        newLaptop.CurrentAmount.Should().Be(1000m);
        newLaptop.Domain.Should().Be(FinancialDomain.Business);

        vacation.Should().NotBeNull();
        vacation!.TargetAmount.Should().Be(5000m);
        vacation.FundingStrategy.Should().Be(GoalFundingStrategy.Surplus);
    }

    [Fact]
    public async Task SeedData_CreatesExactlyThreeGoalAccountLinks()
    {
        // Act
        var goalAccountCount = await Context.GoalAccounts.CountAsync();
        var goalAccounts = await Context.GoalAccounts
            .Include(ga => ga.Goal)
            .Include(ga => ga.Account)
            .ToListAsync();

        // Assert
        goalAccountCount.Should().Be(3);

        var emergencyFundLink = goalAccounts.FirstOrDefault(ga => ga.Goal.Name == "Emergency Fund");
        var laptopLink = goalAccounts.FirstOrDefault(ga => ga.Goal.Name == "New Laptop");
        var vacationLink = goalAccounts.FirstOrDefault(ga => ga.Goal.Name == "Vacation Fund");

        emergencyFundLink.Should().NotBeNull();
        emergencyFundLink!.Account.Name.Should().Be("Personal Savings");

        laptopLink.Should().NotBeNull();
        laptopLink!.Account.Name.Should().Be("Business Savings");

        vacationLink.Should().NotBeNull();
        vacationLink!.Account.Name.Should().Be("Personal Savings");
    }

    [Fact]
    public async Task SeedData_CreatesAtLeastNineTransactions()
    {
        // Act
        var transactionCount = await Context.Transactions.CountAsync();

        // Assert
        transactionCount.Should().BeGreaterThanOrEqualTo(9);
    }

    [Fact]
    public async Task SeedData_TransactionsHaveCorrectTypes()
    {
        // Act
        var transactions = await Context.Transactions.ToListAsync();

        var salaryDeposits = transactions.Where(t => t.Category == "Income" && t.IncomeStreamId != null).ToList();
        var billPayments = transactions.Where(t => t.BillId != null).ToList();
        var goalContributions = transactions.Where(t => t.GoalId != null).ToList();
        var miscExpenses = transactions.Where(t => t.BillId == null && t.GoalId == null && t.IncomeStreamId == null).ToList();

        // Assert
        salaryDeposits.Should().HaveCountGreaterThanOrEqualTo(2);
        billPayments.Should().HaveCountGreaterThanOrEqualTo(3);
        goalContributions.Should().HaveCountGreaterThanOrEqualTo(2);
        miscExpenses.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task SeedData_CreatesExactlyFiveSettings()
    {
        // Act
        var settingCount = await Context.Settings.CountAsync();
        var settings = await Context.Settings.ToListAsync();

        // Assert
        settingCount.Should().Be(5);

        var settingKeys = settings.Select(s => s.SettingKey).ToList();
        settingKeys.Should().Contain("AlertThreshold.CashShortfall");
        settingKeys.Should().Contain("AlertThreshold.BillRisk");
        settingKeys.Should().Contain("AlertChannels.Email");
        settingKeys.Should().Contain("AlertChannels.InApp");
        settingKeys.Should().Contain("DashboardRefreshTime");
    }

    [Fact]
    public async Task SeedData_AllRelationshipsEstablished()
    {
        // Act
        var user = await Context.Users.FirstOrDefaultAsync();
        var accounts = await Context.Accounts.Where(a => a.UserId == user!.Id).CountAsync();
        var bills = await Context.Bills.Where(b => b.UserId == user!.Id).CountAsync();
        var goals = await Context.Goals.Where(g => g.UserId == user!.Id).CountAsync();
        var incomeStreams = await Context.IncomeStreams.Where(i => i.UserId == user!.Id).CountAsync();
        var settings = await Context.Settings.Where(s => s.UserId == user!.Id).CountAsync();

        // Assert: All entities belong to the seeded user
        accounts.Should().Be(5);
        bills.Should().Be(6);
        goals.Should().Be(3);
        incomeStreams.Should().Be(2);
        settings.Should().Be(5);
    }

    [Fact]
    public async Task SeedData_DoesNotDuplicateOnMultipleCalls()
    {
        // Act: Call seed data again
        await SeedData.SeedAsync(Context);

        // Assert: Counts should remain the same (idempotent seeding)
        var userCount = await Context.Users.CountAsync();
        var accountCount = await Context.Accounts.CountAsync();
        var billCount = await Context.Bills.CountAsync();

        userCount.Should().Be(1);
        accountCount.Should().Be(5);
        billCount.Should().Be(6);
    }

    [Fact]
    public async Task SeedData_AccountBalancesMatchExpectedValues()
    {
        // Act
        var personalChecking = await Context.Accounts.FirstOrDefaultAsync(a => a.Name == "Personal Checking");
        var personalSavings = await Context.Accounts.FirstOrDefaultAsync(a => a.Name == "Personal Savings");
        var businessChecking = await Context.Accounts.FirstOrDefaultAsync(a => a.Name == "Business Checking");
        var businessSavings = await Context.Accounts.FirstOrDefaultAsync(a => a.Name == "Business Savings");
        var creditCard = await Context.Accounts.FirstOrDefaultAsync(a => a.Name == "Credit Card");

        // Assert
        personalChecking.Should().NotBeNull();
        personalChecking!.CurrentBalance.Should().Be(5000m);
        personalChecking.SafeMinimumBalance.Should().Be(1000m);

        personalSavings.Should().NotBeNull();
        personalSavings!.CurrentBalance.Should().Be(15000m);
        personalSavings.SafeMinimumBalance.Should().Be(5000m);

        businessChecking.Should().NotBeNull();
        businessChecking!.CurrentBalance.Should().Be(25000m);
        businessChecking.SafeMinimumBalance.Should().Be(10000m);

        businessSavings.Should().NotBeNull();
        businessSavings!.CurrentBalance.Should().Be(50000m);
        businessSavings.SafeMinimumBalance.Should().Be(20000m);

        creditCard.Should().NotBeNull();
        creditCard!.CurrentBalance.Should().Be(-2500m);
        creditCard.SafeMinimumBalance.Should().Be(0m);
    }

    [Fact]
    public async Task SeedData_BillsHaveValidDueDates()
    {
        // Act
        var bills = await Context.Bills.ToListAsync();

        // Assert: All bills should have NextDueDate in the future or present
        foreach (var bill in bills)
        {
            bill.NextDueDate.Should().BeOnOrAfter(DateTime.UtcNow.Date.AddDays(-1),
                $"Bill '{bill.Name}' should have a valid due date");
        }
    }

    [Fact]
    public async Task SeedData_AllEntitiesHaveBaseEntityProperties()
    {
        // Act
        var user = await Context.Users.FirstOrDefaultAsync();
        var account = await Context.Accounts.FirstOrDefaultAsync();
        var bill = await Context.Bills.FirstOrDefaultAsync();
        var goal = await Context.Goals.FirstOrDefaultAsync();

        // Assert: All entities should have Id, CreatedAt, UpdatedAt populated
        user.Should().NotBeNull();
        user!.Id.Should().NotBeEmpty();
        user.CreatedAt.Should().NotBe(default);
        user.UpdatedAt.Should().NotBe(default);

        account.Should().NotBeNull();
        account!.Id.Should().NotBeEmpty();
        account.CreatedAt.Should().NotBe(default);
        account.UpdatedAt.Should().NotBe(default);

        bill.Should().NotBeNull();
        bill!.Id.Should().NotBeEmpty();
        bill.CreatedAt.Should().NotBe(default);
        bill.UpdatedAt.Should().NotBe(default);

        goal.Should().NotBeNull();
        goal!.Id.Should().NotBeEmpty();
        goal.CreatedAt.Should().NotBe(default);
        goal.UpdatedAt.Should().NotBe(default);
    }
}
