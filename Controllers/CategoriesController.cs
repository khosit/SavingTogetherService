using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.Models;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Returns the static list of expense categories (mirrors frontend EXPENSE_CATEGORIES).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    /// <summary>Get all supported expense categories.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ExpenseCategory>), 200)]
    public IActionResult GetAll() => Ok(CategoryMeta.All);
}
