namespace SavingChallengeService.Models;

public class User
{
    public int Id { get; set; }

    /// <summary>Unique key used to identify the user.</summary>
    public string UserKey { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    /// <summary>Monthly gross income in RM</summary>
    public decimal MonthlyIncome { get; set; }

    /// <summary>Fixed monthly expenses (rent, subscriptions, etc.) in RM</summary>
    public decimal FixedExpenses { get; set; }
    public decimal SavingAmount { get; set; }

    /// <summary>Emoji avatar character</summary>
    public string Avatar { get; set; } = "👤";

    /// <summary>Calculated: (MonthlyIncome * 0.55 - FixedExpenses) / 30</summary>
    public decimal DailyBudget { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ICollection<DailyRecord> DailyRecords { get; set; } = [];
}
