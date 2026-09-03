using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.DTOs;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Aggregate stats — streak, dashboard summary.</summary>
[ApiController]
[Route("api/users/{userKey}/stats")]
[Produces("application/json")]
public class StatsController(IStatsService statsService) : ControllerBase
{
    /// <summary>
    /// Get the current consecutive under-budget streak for a user.
    /// A streak increments for each past day where spent &lt;= available budget.
    /// </summary>
    [HttpGet("streak")]
    [ProducesResponseType(typeof(StreakResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetStreak(string userKey)
    {
        var result = await statsService.GetStreakAsync(userKey.ToUpperInvariant());
        return result is null ? NotFound($"User '{userKey}' not found.") : Ok(result);
    }

    /// <summary>
    /// Full dashboard stats snapshot for a user — budget, spending, carry-over,
    /// tomorrow's projected budget, streak, and month-to-date figures.
    /// </summary>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(DashboardStatsResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDashboard(string userKey)
    {
        var result = await statsService.GetDashboardStatsAsync(userKey.ToUpperInvariant());
        return result is null ? NotFound($"User '{userKey}' not found.") : Ok(result);
    }
}
