namespace SavingChallengeService.Models;

public class DailyRecord
{
    public int Id { get; set; }

    /// <summary>Date string: YYYY-MM-DD</summary>
    public string Date { get; set; } = string.Empty;

    /// <summary>Partner slot: "A" or "B"</summary>
    public string UserKey { get; set; } = string.Empty;

    /// <summary>Base daily budget without carry-over</summary>
    public decimal BaseBudget { get; set; }

    /// <summary>Positive = rolled-over surplus; Negative = deficit from previous day</summary>
    public decimal CarryOver { get; set; }

    /// <summary>BaseBudget + CarryOver</summary>
    public decimal AvailableBudget { get; set; }

    // FK
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    // Navigation
    public ICollection<Expense> Expenses { get; set; } = [];
}
