namespace SavingChallengeService.Models;

public class Expense
{
    public int Id { get; set; }

    /// <summary>Amount spent in RM</summary>
    public decimal Amount { get; set; }

    /// <summary>Category id: food, transport, shopping, entertainment, health, bills, other</summary>
    public string Category { get; set; } = "other";

    /// <summary>Optional free-text note</summary>
    public string? Note { get; set; }

    public DateTime Time { get; set; } = DateTime.UtcNow;

    // FK
    public int DailyRecordId { get; set; }
    public DailyRecord DailyRecord { get; set; } = null!;
}
