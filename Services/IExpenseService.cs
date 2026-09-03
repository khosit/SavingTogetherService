using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public interface IExpenseService
{
    Task<ExpenseResponse?> AddExpenseAsync(string userKey, AddExpenseRequest request);
    Task<bool> DeleteExpenseAsync(string userKey, int expenseId);
}
