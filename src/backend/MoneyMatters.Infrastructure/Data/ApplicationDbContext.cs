using Microsoft.EntityFrameworkCore;
using MoneyMatters.Core.Entities;
using MoneyMatters.Core.Enums;

namespace MoneyMatters.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users => Set<User>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Bill> Bills => Set<Bill>();
    public DbSet<IncomeStream> IncomeStreams => Set<IncomeStream>();
    public DbSet<Goal> Goals => Set<Goal>();
    public DbSet<GoalAccount> GoalAccounts => Set<GoalAccount>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<ForecastSnapshot> ForecastSnapshots => Set<ForecastSnapshot>();
    public DbSet<Setting> Settings => Set<Setting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TimeZone).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Account configuration
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Domain);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Institution).HasMaxLength(255);
            entity.Property(e => e.AccountType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.CurrentBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SafeMinimumBalance).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ExternalAccountId).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Transaction configuration
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.AccountId, e.Date }).IsDescending(false, true);
            entity.HasIndex(e => e.BillId);
            entity.HasIndex(e => e.IncomeStreamId);
            entity.HasIndex(e => e.GoalId);

            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.NormalizedMerchant).HasMaxLength(255);
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.ExternalTransactionId).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Bill)
                .WithMany(b => b.Transactions)
                .HasForeignKey(e => e.BillId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.IncomeStream)
                .WithMany(i => i.Transactions)
                .HasForeignKey(e => e.IncomeStreamId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Goal)
                .WithMany(g => g.Transactions)
                .HasForeignKey(e => e.GoalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.TransferAccount)
                .WithMany()
                .HasForeignKey(e => e.TransferAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Bill configuration
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Domain);
            entity.HasIndex(e => e.NextDueDate);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Bills)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.DefaultAccount)
                .WithMany(a => a.Bills)
                .HasForeignKey(e => e.DefaultAccountId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // IncomeStream configuration
        modelBuilder.Entity<IncomeStream>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Domain);
            entity.HasIndex(e => e.NextExpectedDate);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TypicalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.LastReceivedAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.IncomeStreams)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.IncomeStreams)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Goal configuration
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Domain);
            entity.HasIndex(e => e.TargetDate);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.TargetAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FixedContributionAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PercentOfIncome).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Goals)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // GoalAccount configuration
        modelBuilder.Entity<GoalAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.GoalId, e.AccountId }).IsUnique();
            entity.HasIndex(e => e.GoalId);
            entity.HasIndex(e => e.AccountId);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.Goal)
                .WithMany(g => g.GoalAccounts)
                .HasForeignKey(e => e.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Account)
                .WithMany(a => a.GoalAccounts)
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Alert configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Severity);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => e.TriggeredAt).IsDescending();

            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Message).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Alerts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.RelatedAccount)
                .WithMany()
                .HasForeignKey(e => e.RelatedAccountId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RelatedBill)
                .WithMany()
                .HasForeignKey(e => e.RelatedBillId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RelatedGoal)
                .WithMany()
                .HasForeignKey(e => e.RelatedGoalId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.RelatedIncomeStream)
                .WithMany()
                .HasForeignKey(e => e.RelatedIncomeStreamId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ForecastSnapshot configuration
        modelBuilder.Entity<ForecastSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.Domain, e.HorizonDays, e.GeneratedAt })
                .IsDescending(false, false, false, true);

            entity.Property(e => e.ForecastData).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.ForecastSnapshots)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Setting configuration
        modelBuilder.Entity<Setting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.SettingKey }).IsUnique();

            entity.Property(e => e.SettingKey).IsRequired().HasMaxLength(255);
            entity.Property(e => e.SettingValue).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(u => u.Settings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
