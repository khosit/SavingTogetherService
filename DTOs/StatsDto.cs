namespace SavingChallengeService.DTOs;

public record StreakResponse(
    string UserKey,
    int CurrentStreak,
    string Date
);

public record DashboardStatsResponse(
    string UserKey,
    string Date,
    decimal DailyBudget,
    decimal AvailableBudget,
    decimal Spent,
    decimal Remaining,
    bool IsOver,
    decimal CarryOver,
    decimal TomorrowBudget,
    int Streak,
    decimal MonthSpent,
    decimal MonthTargetSavings,
    int MonthDaysRecorded
);
