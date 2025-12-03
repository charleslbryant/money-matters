using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyMatters.Core.Entities;
using MoneyMatters.Core.Enums;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

/// <summary>
/// Tests to validate basic CRUD operations on all entities and verify foreign key relationships work correctly.
/// Ensures data persistence, cascade delete behaviors, and relationship integrity.
/// </summary>
public class CrudOperationTests : DatabaseTestBase
{
    [Fact]
    public async Task User_CanPerformFullCrudCycle()
    {
        // Create
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            TimeZone = "UTC",
            DefaultForecastHorizonDays = 30
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        user.Id.Should().NotBeEmpty();

        // Read
        var readUser = await Context.Users.FindAsync(user.Id);
        readUser.Should().NotBeNull();
        readUser!.Email.Should().Be("test@example.com");
        readUser.Name.Should().Be("Test User");

        // Update
        readUser.Name = "Updated User";
        await Context.SaveChangesAsync();

        var updatedUser = await Context.Users.FindAsync(user.Id);
        updatedUser!.Name.Should().Be("Updated User");

        // Delete
        Context.Users.Remove(updatedUser);
        await Context.SaveChangesAsync();

        var deletedUser = await Context.Users.FindAsync(user.Id);
        deletedUser.Should().BeNull();
    }

    [Fact]
    public async Task Account_CanPerformFullCrudCycle()
    {
        // Setup: Create user first
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Create
        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 500m,
            IncludeInForecast = true,
            IsActive = true
        };

        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        // Read
        var readAccount = await Context.Accounts.FindAsync(account.Id);
        readAccount.Should().NotBeNull();
        readAccount!.Name.Should().Be("Test Account");
        readAccount.CurrentBalance.Should().Be(1000m);

        // Update
        readAccount.CurrentBalance = 2000m;
        await Context.SaveChangesAsync();

        var updatedAccount = await Context.Accounts.FindAsync(account.Id);
        updatedAccount!.CurrentBalance.Should().Be(2000m);

        // Delete
        Context.Accounts.Remove(updatedAccount);
        await Context.SaveChangesAsync();

        var deletedAccount = await Context.Accounts.FindAsync(account.Id);
        deletedAccount.Should().BeNull();
    }

    [Fact]
    public async Task Transaction_CanPerformFullCrudCycle()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        // Create
        var transaction = new Transaction
        {
            AccountId = account.Id,
            Amount = 100m,
            Date = DateTime.UtcNow,
            Description = "Test Transaction",
            Category = "Test",
            IsReconciled = false
        };

        Context.Transactions.Add(transaction);
        await Context.SaveChangesAsync();

        // Read
        var readTransaction = await Context.Transactions.FindAsync(transaction.Id);
        readTransaction.Should().NotBeNull();
        readTransaction!.Amount.Should().Be(100m);

        // Update
        readTransaction.Amount = 200m;
        await Context.SaveChangesAsync();

        var updatedTransaction = await Context.Transactions.FindAsync(transaction.Id);
        updatedTransaction!.Amount.Should().Be(200m);

        // Delete
        Context.Transactions.Remove(updatedTransaction);
        await Context.SaveChangesAsync();

        var deletedTransaction = await Context.Transactions.FindAsync(transaction.Id);
        deletedTransaction.Should().BeNull();
    }

    [Fact]
    public async Task Bill_CanPerformFullCrudCycle()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Create
        var bill = new Bill
        {
            UserId = user.Id,
            Name = "Test Bill",
            Amount = 100m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 15,
            NextDueDate = DateTime.UtcNow.AddDays(15),
            Domain = FinancialDomain.Personal,
            Priority = 1,
            IsAutoPay = false,
            IsActive = true
        };

        Context.Bills.Add(bill);
        await Context.SaveChangesAsync();

        // Read
        var readBill = await Context.Bills.FindAsync(bill.Id);
        readBill.Should().NotBeNull();
        readBill!.Name.Should().Be("Test Bill");

        // Update
        readBill.Amount = 200m;
        await Context.SaveChangesAsync();

        var updatedBill = await Context.Bills.FindAsync(bill.Id);
        updatedBill!.Amount.Should().Be(200m);

        // Delete
        Context.Bills.Remove(updatedBill);
        await Context.SaveChangesAsync();

        var deletedBill = await Context.Bills.FindAsync(bill.Id);
        deletedBill.Should().BeNull();
    }

    [Fact]
    public async Task Goal_CanPerformFullCrudCycle()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Create
        var goal = new Goal
        {
            UserId = user.Id,
            Name = "Test Goal",
            TargetAmount = 5000m,
            CurrentAmount = 1000m,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            FixedContributionAmount = 500m,
            Priority = 1,
            IsActive = true
        };

        Context.Goals.Add(goal);
        await Context.SaveChangesAsync();

        // Read
        var readGoal = await Context.Goals.FindAsync(goal.Id);
        readGoal.Should().NotBeNull();
        readGoal!.Name.Should().Be("Test Goal");

        // Update
        readGoal.CurrentAmount = 2000m;
        await Context.SaveChangesAsync();

        var updatedGoal = await Context.Goals.FindAsync(goal.Id);
        updatedGoal!.CurrentAmount.Should().Be(2000m);

        // Delete
        Context.Goals.Remove(updatedGoal);
        await Context.SaveChangesAsync();

        var deletedGoal = await Context.Goals.FindAsync(goal.Id);
        deletedGoal.Should().BeNull();
    }

    [Fact]
    public async Task UserDeletion_CascadesToAccounts()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        var accountId = account.Id;

        // Act: Delete user
        Context.Users.Remove(user);
        await Context.SaveChangesAsync();

        // Assert: Account should be deleted via cascade
        var deletedAccount = await Context.Accounts.FindAsync(accountId);
        deletedAccount.Should().BeNull();
    }

    [Fact]
    public async Task UserDeletion_CascadesToBills()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var bill = new Bill
        {
            UserId = user.Id,
            Name = "Test Bill",
            Amount = 100m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 1,
            NextDueDate = DateTime.UtcNow.AddMonths(1),
            Domain = FinancialDomain.Personal,
            Priority = 1,
            IsAutoPay = false,
            IsActive = true
        };
        Context.Bills.Add(bill);
        await Context.SaveChangesAsync();

        var billId = bill.Id;

        // Act: Delete user
        Context.Users.Remove(user);
        await Context.SaveChangesAsync();

        // Assert: Bill should be deleted via cascade
        var deletedBill = await Context.Bills.FindAsync(billId);
        deletedBill.Should().BeNull();
    }

    [Fact]
    public async Task UserDeletion_CascadesToGoals()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var goal = new Goal
        {
            UserId = user.Id,
            Name = "Test Goal",
            TargetAmount = 1000m,
            CurrentAmount = 0m,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            Priority = 1,
            IsActive = true
        };
        Context.Goals.Add(goal);
        await Context.SaveChangesAsync();

        var goalId = goal.Id;

        // Act: Delete user
        Context.Users.Remove(user);
        await Context.SaveChangesAsync();

        // Assert: Goal should be deleted via cascade
        var deletedGoal = await Context.Goals.FindAsync(goalId);
        deletedGoal.Should().BeNull();
    }

    [Fact]
    public async Task AccountDeletion_CascadesToTransactions()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        var transaction = new Transaction
        {
            AccountId = account.Id,
            Amount = 100m,
            Date = DateTime.UtcNow,
            Description = "Test",
            Category = "Test",
            IsReconciled = false
        };
        Context.Transactions.Add(transaction);
        await Context.SaveChangesAsync();

        var transactionId = transaction.Id;

        // Act: Delete account
        Context.Accounts.Remove(account);
        await Context.SaveChangesAsync();

        // Assert: Transaction should be deleted via cascade
        var deletedTransaction = await Context.Transactions.FindAsync(transactionId);
        deletedTransaction.Should().BeNull();
    }

    [Fact]
    public async Task BillDeletion_SetsTransactionBillIdToNull()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        var bill = new Bill
        {
            UserId = user.Id,
            Name = "Test Bill",
            Amount = 100m,
            Frequency = BillFrequency.Monthly,
            DayOfMonth = 1,
            NextDueDate = DateTime.UtcNow.AddMonths(1),
            Domain = FinancialDomain.Personal,
            Priority = 1,
            IsAutoPay = false,
            IsActive = true
        };
        Context.Bills.Add(bill);
        await Context.SaveChangesAsync();

        var transaction = new Transaction
        {
            AccountId = account.Id,
            BillId = bill.Id,
            Amount = -100m,
            Date = DateTime.UtcNow,
            Description = "Bill Payment",
            Category = "Bills",
            IsReconciled = true
        };
        Context.Transactions.Add(transaction);
        await Context.SaveChangesAsync();

        var transactionId = transaction.Id;

        // Act: Delete bill
        Context.Bills.Remove(bill);
        await Context.SaveChangesAsync();

        // Assert: Transaction should still exist but BillId should be null
        var updatedTransaction = await Context.Transactions.FindAsync(transactionId);
        updatedTransaction.Should().NotBeNull();
        updatedTransaction!.BillId.Should().BeNull();
    }

    [Fact]
    public async Task NavigationProperties_LoadCorrectly()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        // Act: Load with navigation properties
        var loadedUser = await Context.Users
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        loadedUser.Should().NotBeNull();
        loadedUser!.Accounts.Should().NotBeNull();
        loadedUser.Accounts.Should().HaveCount(1);
        loadedUser.Accounts.First().Name.Should().Be("Test Account");
    }

    [Fact]
    public async Task GoalAccount_LinkCreationAndDeletion()
    {
        // Setup
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Savings",
            Institution = "Test Bank",
            AccountType = "Savings",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 5000m,
            SafeMinimumBalance = 1000m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);

        var goal = new Goal
        {
            UserId = user.Id,
            Name = "Emergency Fund",
            TargetAmount = 10000m,
            CurrentAmount = 5000m,
            TargetDate = DateTime.UtcNow.AddMonths(12),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            Priority = 1,
            IsActive = true
        };
        Context.Goals.Add(goal);
        await Context.SaveChangesAsync();

        // Create link
        var goalAccount = new GoalAccount
        {
            GoalId = goal.Id,
            AccountId = account.Id
        };
        Context.GoalAccounts.Add(goalAccount);
        await Context.SaveChangesAsync();

        // Verify link exists
        var link = await Context.GoalAccounts
            .FirstOrDefaultAsync(ga => ga.GoalId == goal.Id && ga.AccountId == account.Id);
        link.Should().NotBeNull();

        // Delete link
        Context.GoalAccounts.Remove(link!);
        await Context.SaveChangesAsync();

        var deletedLink = await Context.GoalAccounts
            .FirstOrDefaultAsync(ga => ga.GoalId == goal.Id && ga.AccountId == account.Id);
        deletedLink.Should().BeNull();
    }
}
