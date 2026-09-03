namespace SavingChallengeService.Services;

/// <summary>Pure budget math — mirrors the frontend calcDailyBudget formula.</summary>
public static class BudgetCalculator
{
    public static decimal CalcDailyBudget(decimal monthlyIncome, decimal fixedExpenses, decimal savingAmount) =>
        Math.Max(0, (monthlyIncome - fixedExpenses - savingAmount) / 30m);

    public static decimal MonthlyTargetSavings(decimal monthlyIncome) =>
        Math.Round(monthlyIncome * 0.45m, 2);
}
