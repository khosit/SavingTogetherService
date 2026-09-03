using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.DTOs;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Add and delete expenses for a user's current day.</summary>
[ApiController]
[Route("api/users/{userKey}/expenses")]
[Produces("application/json")]
public class ExpensesController(IExpenseService expenseService) : ControllerBase
{
    /// <summary>Add an expense to today's record.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AddExpense(string userKey, [FromBody] AddExpenseRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var expense = await expenseService.AddExpenseAsync(userKey.ToUpperInvariant(), request);
        return expense is null
            ? NotFound($"User '{userKey}' not found. Set up the profile first.")
            : CreatedAtAction(nameof(AddExpense), new { userKey }, expense);
    }

    /// <summary>Delete an expense by id (must belong to the given user).</summary>
    [HttpDelete("{expenseId:int}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteExpense(string userKey, int expenseId)
    {
        var deleted = await expenseService.DeleteExpenseAsync(userKey.ToUpperInvariant(), expenseId);
        return deleted ? NoContent() : NotFound($"Expense {expenseId} not found for user '{userKey}'.");
    }
}
