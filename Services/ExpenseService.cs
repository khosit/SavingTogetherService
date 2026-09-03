using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Data;
using SavingChallengeService.DTOs;
using SavingChallengeService.Models;

namespace SavingChallengeService.Services;

public class ExpenseService(AppDbContext db, DailyRecordService recordService) : IExpenseService
{
    public async Task<ExpenseResponse?> AddExpenseAsync(string userKey, AddExpenseRequest request)
    {
        var today  = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var record = await recordService.GetOrCreateRecordAsync(userKey, today);
        if (record is null) return null;

        var expense = new Expense
        {
            Amount        = request.Amount,
            Category      = request.Category,
            Note          = request.Note,
            Time          = DateTime.UtcNow,
            DailyRecordId = record.Id,
        };
        db.Expenses.Add(expense);
        await db.SaveChangesAsync();

        return new ExpenseResponse(expense.Id, expense.Amount, expense.Category, expense.Note, expense.Time, expense.DailyRecordId);
    }

    public async Task<bool> DeleteExpenseAsync(string userKey, int expenseId)
    {
        var expense = await db.Expenses
            .Include(e => e.DailyRecord)
            .FirstOrDefaultAsync(e => e.Id == expenseId && e.DailyRecord.UserKey == userKey);

        if (expense is null) return false;

        db.Expenses.Remove(expense);
        await db.SaveChangesAsync();
        return true;
    }
}
