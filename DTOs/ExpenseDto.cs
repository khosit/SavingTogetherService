using System.ComponentModel.DataAnnotations;

namespace SavingChallengeService.DTOs;

// ── Response ──────────────────────────────────────────────
public record ExpenseResponse(
    int Id,
    decimal Amount,
    string Category,
    string? Note,
    DateTime Time,
    int DailyRecordId
);

// ── Requests ──────────────────────────────────────────────
public record AddExpenseRequest(
    [Range(0.01, double.MaxValue)] decimal Amount,
    [Required] string Category,
    [StringLength(60)] string? Note
);
