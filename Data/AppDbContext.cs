using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Models;

namespace SavingChallengeService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<DailyRecord> DailyRecords => Set<DailyRecord>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Couple> Couples => Set<Couple>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User – UserKey must be unique
        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserKey)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.MonthlyIncome)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .Property(u => u.FixedExpenses)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .Property(u => u.SavingAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<User>()
            .Property(u => u.DailyBudget)
            .HasPrecision(18, 2);

        // DailyRecord – composite uniqueness (UserKey + Date)
        modelBuilder.Entity<DailyRecord>()
            .HasIndex(r => new { r.UserKey, r.Date })
            .IsUnique();

        modelBuilder.Entity<DailyRecord>()
            .Property(r => r.BaseBudget)
            .HasPrecision(18, 2);

        modelBuilder.Entity<DailyRecord>()
            .Property(r => r.CarryOver)
            .HasPrecision(18, 2);

        modelBuilder.Entity<DailyRecord>()
            .Property(r => r.AvailableBudget)
            .HasPrecision(18, 2);

        // Expense
        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        // Couple codes identify the relationship; multiple couple rows are supported.
        modelBuilder.Entity<Couple>()
            .HasIndex(c => c.CoupleCode);
    }
}
