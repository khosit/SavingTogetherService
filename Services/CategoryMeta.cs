using SavingChallengeService.Models;

namespace SavingChallengeService.Services;

/// <summary>Mirrors the EXPENSE_CATEGORIES constant from the frontend.</summary>
public static class CategoryMeta
{
    public static readonly IReadOnlyList<ExpenseCategory> All =
    [
        new() { Id = "food",          Label = "Food & Drinks",  Icon = "🍜", Color = "#f97316" },
        new() { Id = "transport",     Label = "Transport",      Icon = "🚗", Color = "#3b82f6" },
        new() { Id = "shopping",      Label = "Shopping",       Icon = "🛍️", Color = "#ec4899" },
        new() { Id = "entertainment", Label = "Entertainment",  Icon = "🎮", Color = "#8b5cf6" },
        new() { Id = "health",        Label = "Health",         Icon = "💊", Color = "#10b981" },
        new() { Id = "bills",         Label = "Bills",          Icon = "📋", Color = "#6b7280" },
        new() { Id = "other",         Label = "Other",          Icon = "💸", Color = "#f59e0b" },
    ];
}
