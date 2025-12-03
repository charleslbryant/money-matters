using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MoneyMatters.Core.Entities;
using MoneyMatters.Infrastructure.Tests.Helpers;

namespace MoneyMatters.Infrastructure.Tests.Data;

/// <summary>
/// Tests to validate database schema configuration including tables, indexes, foreign keys, and constraints.
/// Verifies that EF Core entity configurations match expected database schema design.
/// </summary>
public class SchemaValidationTests : DatabaseTestBase
{
    [SkipOnInMemoryDatabase]
    public void Schema_HasAllRequiredTables()
    {
        // Arrange: Get all entity types from the model
        var entityTypes = Context.Model.GetEntityTypes();

        // Act: Get table names
        var tableNames = entityTypes
            .Select(e => e.GetTableName())
            .Where(n => n != null)
            .ToList();

        // Assert: Verify all 10 expected tables exist
        tableNames.Should().Contain("Users");
        tableNames.Should().Contain("Accounts");
        tableNames.Should().Contain("Transactions");
        tableNames.Should().Contain("Bills");
        tableNames.Should().Contain("IncomeStreams");
        tableNames.Should().Contain("Goals");
        tableNames.Should().Contain("GoalAccounts");
        tableNames.Should().Contain("Alerts");
        tableNames.Should().Contain("ForecastSnapshots");
        tableNames.Should().Contain("Settings");
        tableNames.Should().HaveCount(10);
    }

    [Fact]
    public void User_HasEmailUniqueIndex()
    {
        // Arrange
        var userEntity = Context.Model.FindEntityType(typeof(User));

        // Act
        var emailIndex = userEntity!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        // Assert
        emailIndex.Should().NotBeNull();
        emailIndex!.IsUnique.Should().BeTrue();
    }

    [Fact]
    public void Setting_HasCompositeUniqueIndex()
    {
        // Arrange
        var settingEntity = Context.Model.FindEntityType(typeof(Setting));

        // Act
        var compositeIndex = settingEntity!.GetIndexes()
            .FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "UserId") &&
                i.Properties.Any(p => p.Name == "SettingKey"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.IsUnique.Should().BeTrue();
        compositeIndex.Properties.Should().HaveCount(2);
    }

    [Fact]
    public void GoalAccount_HasCompositeUniqueIndex()
    {
        // Arrange
        var goalAccountEntity = Context.Model.FindEntityType(typeof(GoalAccount));

        // Act
        var compositeIndex = goalAccountEntity!.GetIndexes()
            .FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "GoalId") &&
                i.Properties.Any(p => p.Name == "AccountId"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.IsUnique.Should().BeTrue();
        compositeIndex.Properties.Should().HaveCount(2);
    }

    [Fact]
    public void Transaction_HasAccountIdDateIndex()
    {
        // Arrange
        var transactionEntity = Context.Model.FindEntityType(typeof(Transaction));

        // Act
        var compositeIndex = transactionEntity!.GetIndexes()
            .FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "AccountId") &&
                i.Properties.Any(p => p.Name == "Date"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.Properties.Should().HaveCount(2);
    }

    [SkipOnInMemoryDatabase]
    public void Alert_HasTriggeredAtIndex()
    {
        // Arrange
        var alertEntity = Context.Model.FindEntityType(typeof(Alert));

        // Act
        var triggeredAtIndex = alertEntity!.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "TriggeredAt"));

        // Assert
        triggeredAtIndex.Should().NotBeNull();
        triggeredAtIndex!.IsDescending.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ForecastSnapshot_HasCompositeIndex()
    {
        // Arrange
        var forecastEntity = Context.Model.FindEntityType(typeof(ForecastSnapshot));

        // Act
        var compositeIndex = forecastEntity!.GetIndexes()
            .FirstOrDefault(i =>
                i.Properties.Any(p => p.Name == "UserId") &&
                i.Properties.Any(p => p.Name == "Domain") &&
                i.Properties.Any(p => p.Name == "HorizonDays") &&
                i.Properties.Any(p => p.Name == "GeneratedAt"));

        // Assert
        compositeIndex.Should().NotBeNull();
        compositeIndex!.Properties.Should().HaveCount(4);
    }

    [Fact]
    public void Account_HasCascadeDeleteToTransactions()
    {
        // Arrange
        var transactionEntity = Context.Model.FindEntityType(typeof(Transaction));

        // Act
        var accountFK = transactionEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Account));

        // Assert
        accountFK.Should().NotBeNull();
        accountFK!.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
    }

    [Fact]
    public void User_HasCascadeDeleteToAccounts()
    {
        // Arrange
        var accountEntity = Context.Model.FindEntityType(typeof(Account));

        // Act
        var userFK = accountEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        // Assert
        userFK.Should().NotBeNull();
        userFK!.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
    }

    [Fact]
    public void User_HasCascadeDeleteToBills()
    {
        // Arrange
        var billEntity = Context.Model.FindEntityType(typeof(Bill));

        // Act
        var userFK = billEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        // Assert
        userFK.Should().NotBeNull();
        userFK!.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
    }

    [Fact]
    public void User_HasCascadeDeleteToGoals()
    {
        // Arrange
        var goalEntity = Context.Model.FindEntityType(typeof(Goal));

        // Act
        var userFK = goalEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(User));

        // Assert
        userFK.Should().NotBeNull();
        userFK!.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);
    }

    [SkipOnInMemoryDatabase]
    public void MonetaryFields_UseCorrectDecimalPrecision()
    {
        // Arrange & Act: Check all entities with monetary decimal fields
        var accountBalanceProperty = Context.Model.FindEntityType(typeof(Account))!
            .FindProperty("Balance");
        var transactionAmountProperty = Context.Model.FindEntityType(typeof(Transaction))!
            .FindProperty("Amount");
        var billAmountProperty = Context.Model.FindEntityType(typeof(Bill))!
            .FindProperty("Amount");
        var goalTargetAmountProperty = Context.Model.FindEntityType(typeof(Goal))!
            .FindProperty("TargetAmount");

        // Assert: All monetary fields should use decimal(18,2)
        accountBalanceProperty.Should().NotBeNull();
        accountBalanceProperty!.GetPrecision().Should().Be(18);
        accountBalanceProperty.GetScale().Should().Be(2);

        transactionAmountProperty.Should().NotBeNull();
        transactionAmountProperty!.GetPrecision().Should().Be(18);
        transactionAmountProperty.GetScale().Should().Be(2);

        billAmountProperty.Should().NotBeNull();
        billAmountProperty!.GetPrecision().Should().Be(18);
        billAmountProperty.GetScale().Should().Be(2);

        goalTargetAmountProperty.Should().NotBeNull();
        goalTargetAmountProperty!.GetPrecision().Should().Be(18);
        goalTargetAmountProperty.GetScale().Should().Be(2);
    }

    [Fact]
    public void BaseEntity_PropertiesExistOnAllEntities()
    {
        // Arrange: Get all entity types
        var entityTypes = Context.Model.GetEntityTypes()
            .Where(e => e.ClrType != typeof(object));

        // Act & Assert: Verify each entity has Id, CreatedAt, UpdatedAt
        foreach (var entityType in entityTypes)
        {
            var idProperty = entityType.FindProperty("Id");
            var createdAtProperty = entityType.FindProperty("CreatedAt");
            var updatedAtProperty = entityType.FindProperty("UpdatedAt");

            idProperty.Should().NotBeNull($"{entityType.ClrType.Name} should have Id");
            createdAtProperty.Should().NotBeNull($"{entityType.ClrType.Name} should have CreatedAt");
            updatedAtProperty.Should().NotBeNull($"{entityType.ClrType.Name} should have UpdatedAt");
        }
    }

    [Fact]
    public void Context_HasAllExpectedDbSets()
    {
        // Assert: Verify all DbSet properties exist
        Context.Users.Should().NotBeNull();
        Context.Accounts.Should().NotBeNull();
        Context.Transactions.Should().NotBeNull();
        Context.Bills.Should().NotBeNull();
        Context.IncomeStreams.Should().NotBeNull();
        Context.Goals.Should().NotBeNull();
        Context.GoalAccounts.Should().NotBeNull();
        Context.Alerts.Should().NotBeNull();
        Context.ForecastSnapshots.Should().NotBeNull();
        Context.Settings.Should().NotBeNull();
    }

    [Fact]
    public void User_EmailIsRequired()
    {
        // Arrange
        var userEntity = Context.Model.FindEntityType(typeof(User));

        // Act
        var emailProperty = userEntity!.FindProperty("Email");

        // Assert
        emailProperty.Should().NotBeNull();
        emailProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Account_NameIsRequired()
    {
        // Arrange
        var accountEntity = Context.Model.FindEntityType(typeof(Account));

        // Act
        var nameProperty = accountEntity!.FindProperty("Name");

        // Assert
        nameProperty.Should().NotBeNull();
        nameProperty!.IsNullable.Should().BeFalse();
    }

    [Fact]
    public void Transaction_BillIdIsOptional()
    {
        // Arrange
        var transactionEntity = Context.Model.FindEntityType(typeof(Transaction));

        // Act
        var billIdProperty = transactionEntity!.FindProperty("BillId");

        // Assert
        billIdProperty.Should().NotBeNull();
        billIdProperty!.IsNullable.Should().BeTrue("BillId should be optional");
    }

    [Fact]
    public void Bill_HasSetNullForTransactionOnDelete()
    {
        // Arrange
        var transactionEntity = Context.Model.FindEntityType(typeof(Transaction));

        // Act
        var billFK = transactionEntity!.GetForeignKeys()
            .FirstOrDefault(fk => fk.PrincipalEntityType.ClrType == typeof(Bill));

        // Assert
        billFK.Should().NotBeNull();
        billFK!.DeleteBehavior.Should().Be(DeleteBehavior.SetNull);
    }
}
