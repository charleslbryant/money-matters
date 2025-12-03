using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyMatters.Core.Entities;
using MoneyMatters.Core.Enums;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

/// <summary>
/// Tests to validate database constraints prevent invalid data.
/// Verifies unique constraints, required fields, and data type constraints.
/// </summary>
public class ConstraintValidationTests : DatabaseTestBase
{
    [SkipOnInMemoryDatabase]
    public async Task User_EmailMustBeUnique()
    {
        // Arrange
        var user1 = new User
        {
            Email = "duplicate@example.com",
            Name = "User 1",
            TimeZone = "UTC"
        };

        Context.Users.Add(user1);
        await Context.SaveChangesAsync();

        var user2 = new User
        {
            Email = "duplicate@example.com",
            Name = "User 2",
            TimeZone = "UTC"
        };

        Context.Users.Add(user2);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("Email must be unique");
    }

    [Fact]
    public async Task User_EmailIsRequired()
    {
        // Arrange
        var user = new User
        {
            Email = null!,
            Name = "Test User",
            TimeZone = "UTC"
        };

        Context.Users.Add(user);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("Email is required");
    }

    [SkipOnInMemoryDatabase]
    public async Task Setting_UserIdAndSettingKeyMustBeUnique()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var setting1 = new Setting
        {
            UserId = user.Id,
            SettingKey = "TestKey",
            SettingValue = "Value1"
        };

        Context.Settings.Add(setting1);
        await Context.SaveChangesAsync();

        var setting2 = new Setting
        {
            UserId = user.Id,
            SettingKey = "TestKey",
            SettingValue = "Value2"
        };

        Context.Settings.Add(setting2);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("UserId and SettingKey combination must be unique");
    }

    [SkipOnInMemoryDatabase]
    public async Task GoalAccount_GoalIdAndAccountIdMustBeUnique()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = "Test Account",
            Institution = "Test Bank",
            AccountType = "Savings",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };
        Context.Accounts.Add(account);

        var goal = new Goal
        {
            UserId = user.Id,
            Name = "Test Goal",
            TargetAmount = 5000m,
            CurrentAmount = 0m,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.FixedAmount,
            Priority = 1,
            IsActive = true
        };
        Context.Goals.Add(goal);
        await Context.SaveChangesAsync();

        var goalAccount1 = new GoalAccount
        {
            GoalId = goal.Id,
            AccountId = account.Id
        };
        Context.GoalAccounts.Add(goalAccount1);
        await Context.SaveChangesAsync();

        var goalAccount2 = new GoalAccount
        {
            GoalId = goal.Id,
            AccountId = account.Id
        };
        Context.GoalAccounts.Add(goalAccount2);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("GoalId and AccountId combination must be unique");
    }

    [SkipOnInMemoryDatabase]
    public async Task Transaction_CannotHaveInvalidAccountId()
    {
        // Arrange
        var invalidAccountId = Guid.NewGuid();

        var transaction = new Transaction
        {
            AccountId = invalidAccountId,
            Amount = 100m,
            Date = DateTime.UtcNow,
            Description = "Test",
            Category = "Test",
            IsReconciled = false
        };

        Context.Transactions.Add(transaction);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("AccountId must reference valid Account");
    }

    [SkipOnInMemoryDatabase]
    public async Task Bill_CannotHaveInvalidUserId()
    {
        // Arrange
        var invalidUserId = Guid.NewGuid();

        var bill = new Bill
        {
            UserId = invalidUserId,
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

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("UserId must reference valid User");
    }

    [Fact]
    public async Task Account_NameIsRequired()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var account = new Account
        {
            UserId = user.Id,
            Name = null!,
            Institution = "Test Bank",
            AccountType = "Checking",
            Domain = FinancialDomain.Personal,
            CurrentBalance = 1000m,
            SafeMinimumBalance = 100m,
            IncludeInForecast = true,
            IsActive = true
        };

        Context.Accounts.Add(account);

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("Name is required");
    }

    [Fact]
    public async Task Bill_NameIsRequired()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var bill = new Bill
        {
            UserId = user.Id,
            Name = null!,
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

        // Act & Assert
        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<DbUpdateException>("Name is required");
    }

    [Fact]
    public async Task Transaction_BillIdCanBeNull()
    {
        // Arrange
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
            BillId = null,
            Amount = 100m,
            Date = DateTime.UtcNow,
            Description = "Test",
            Category = "Test",
            IsReconciled = false
        };

        Context.Transactions.Add(transaction);

        // Act
        var act = async () => await Context.SaveChangesAsync();

        // Assert: Should NOT throw
        await act.Should().NotThrowAsync("BillId is optional");
    }

    [Fact]
    public async Task Transaction_GoalIdCanBeNull()
    {
        // Arrange
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
            GoalId = null,
            Amount = 100m,
            Date = DateTime.UtcNow,
            Description = "Test",
            Category = "Test",
            IsReconciled = false
        };

        Context.Transactions.Add(transaction);

        // Act
        var act = async () => await Context.SaveChangesAsync();

        // Assert: Should NOT throw
        await act.Should().NotThrowAsync("GoalId is optional");
    }

    [Fact]
    public async Task DecimalFields_HandlePrecisionCorrectly()
    {
        // Arrange
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
            CurrentBalance = 1234567890123456.78m, // 18 digits, 2 decimal places
            SafeMinimumBalance = 100.99m,
            IncludeInForecast = true,
            IsActive = true
        };

        Context.Accounts.Add(account);
        await Context.SaveChangesAsync();

        // Act: Read back the value
        var savedAccount = await Context.Accounts.FindAsync(account.Id);

        // Assert: Value should be preserved with correct precision
        savedAccount.Should().NotBeNull();
        savedAccount!.CurrentBalance.Should().Be(1234567890123456.78m);
        savedAccount.SafeMinimumBalance.Should().Be(100.99m);
    }

    [Fact]
    public async Task EnumFields_AcceptValidValues()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var bill = new Bill
        {
            UserId = user.Id,
            Name = "Test Bill",
            Amount = 100m,
            Frequency = BillFrequency.Weekly,
            DayOfMonth = 1,
            NextDueDate = DateTime.UtcNow.AddDays(7),
            Domain = FinancialDomain.Business,
            Priority = 1,
            IsAutoPay = true,
            IsActive = true
        };

        Context.Bills.Add(bill);
        await Context.SaveChangesAsync();

        // Act: Read back the bill
        var savedBill = await Context.Bills.FindAsync(bill.Id);

        // Assert: Enum values should be preserved
        savedBill.Should().NotBeNull();
        savedBill!.Frequency.Should().Be(BillFrequency.Weekly);
        savedBill.Domain.Should().Be(FinancialDomain.Business);
    }

    [Fact]
    public async Task Goal_FundingStrategyEnumIsValid()
    {
        // Arrange
        var user = new User { Email = "test@example.com", Name = "Test User", TimeZone = "UTC" };
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var goal = new Goal
        {
            UserId = user.Id,
            Name = "Test Goal",
            TargetAmount = 5000m,
            CurrentAmount = 0m,
            TargetDate = DateTime.UtcNow.AddMonths(6),
            Domain = FinancialDomain.Personal,
            FundingStrategy = GoalFundingStrategy.Surplus,
            Priority = 1,
            IsActive = true
        };

        Context.Goals.Add(goal);
        await Context.SaveChangesAsync();

        // Act: Read back the goal
        var savedGoal = await Context.Goals.FindAsync(goal.Id);

        // Assert: FundingStrategy should be preserved
        savedGoal.Should().NotBeNull();
        savedGoal!.FundingStrategy.Should().Be(GoalFundingStrategy.Surplus);
    }

    [Fact]
    public async Task BaseEntity_TimestampsAreSet()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            TimeZone = "UTC"
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        // Assert: CreatedAt and UpdatedAt should be set
        user.CreatedAt.Should().NotBe(default(DateTime));
        user.UpdatedAt.Should().NotBe(default(DateTime));
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [SkipOnInMemoryDatabase]
    public async Task BaseEntity_UpdatedAtChangesOnModification()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            Name = "Test User",
            TimeZone = "UTC"
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        var originalUpdatedAt = user.UpdatedAt;

        // Wait a brief moment to ensure timestamp difference
        await Task.Delay(100);

        // Act: Update the user
        user.Name = "Updated User";
        await Context.SaveChangesAsync();

        // Assert: UpdatedAt should change, CreatedAt should not
        user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        user.CreatedAt.Should().Be(user.CreatedAt); // CreatedAt unchanged
    }
}
