namespace SavingChallengeService.DTOs;

// ── Response ──────────────────────────────────────────────
public record DailyRecordResponse(
    int Id,
    string Date,
    string UserKey,
    decimal BaseBudget,
    decimal CarryOver,
    decimal AvailableBudget,
    decimal TotalSpent,
    decimal Remaining,
    bool IsOver,
    IEnumerable<ExpenseResponse> Expenses
);

// ── Monthly summary ───────────────────────────────────────
public record MonthSummaryResponse(
    int Year,
    int Month,
    string UserKey,
    decimal TotalSpent,
    decimal TotalBudget,
    decimal TotalSaved,
    int DaysRecorded,
    int DaysUnderBudget,
    decimal AvgDailySpend,
    decimal TargetMonthlySavings,
    IEnumerable<CategoryBreakdown> CategoryBreakdown,
    IEnumerable<DailyRecordResponse> DailyRecords
);

public record CategoryBreakdown(
    string CategoryId,
    string Label,
    string Icon,
    string Color,
    decimal Amount,
    double Percentage
);
