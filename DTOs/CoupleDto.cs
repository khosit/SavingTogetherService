using System.ComponentModel.DataAnnotations;

namespace SavingChallengeService.DTOs;

// ── Response ──────────────────────────────────────────────
public record CoupleResponse(
    int Id,
    string CoupleCode,
    string UserKeyA,
    string? UserKeyB,
    bool IsLinked,
    DateTime LinkedAt,
    DateTime UpdatedAt
);

// ── Requests ──────────────────────────────────────────────
public record LinkCoupleRequest(
    [Required, StringLength(20)] string CoupleCode,
    [Required, StringLength(50)] string UserKey
);

// ── Couple dashboard ──────────────────────────────────────
public record CoupleDashboardResponse(
    CoupleResponse Couple,
    UserDailySummary? UserA,
    UserDailySummary? UserB,
    decimal CombinedSpent,
    decimal CombinedBudget,
    decimal CombinedRemaining,
    bool IsOver,
    string? WinnerUserKey
);

public record UserDailySummary(
    string UserKey,
    string Name,
    string Avatar,
    decimal Spent,
    decimal Budget,
    decimal Remaining,
    bool IsOver,
    int Streak
);
