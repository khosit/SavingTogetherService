using System.ComponentModel.DataAnnotations;

namespace SavingChallengeService.DTOs;

// ── Response ──────────────────────────────────────────────
public record UserResponse(
    int Id,
    string UserKey,
    string Name,
    decimal MonthlyIncome,
    decimal FixedExpenses,
    decimal SavingAmount,
    string Avatar,
    decimal DailyBudget,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

// ── Requests ──────────────────────────────────────────────
public record CreateUserRequest(
    [Required, StringLength(50)] string UserKey,
    [Required, StringLength(50)] string Name,
    [Range(0, double.MaxValue)] decimal MonthlyIncome,
    [Range(0, double.MaxValue)] decimal FixedExpenses,
    [Range(0, double.MaxValue)] decimal SavingAmount,
    [StringLength(10)] string Avatar = "👤"
);

public record CreateOrUpdateUserRequest(
    [Required, StringLength(50)] string Name,
    [Range(0, double.MaxValue)] decimal MonthlyIncome,
    [Range(0, double.MaxValue)] decimal FixedExpenses,
    [Range(0, double.MaxValue)] decimal SavingAmount,
    [StringLength(10)] string Avatar = "👤"
);
