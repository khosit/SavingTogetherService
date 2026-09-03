namespace SavingChallengeService.Models;

public class Couple
{
    public int Id { get; set; }

    /// <summary>Shared couple code chosen by both partners</summary>
    public string CoupleCode { get; set; } = string.Empty;

    public string UserKeyA { get; set; } = string.Empty;
    public string? UserKeyB { get; set; }

    public bool IsLinked { get; set; }

    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
