using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.DTOs;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Manage the couple link and view shared dashboard.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CoupleController(ICoupleService coupleService) : ControllerBase
{
    /// <summary>Get the current couple status.</summary>
    [HttpGet("{userKey}")]
    [ProducesResponseType(typeof(CoupleResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCouple(string userKey)
    {
        var couple = await coupleService.GetCoupleAsync(userKey);
        return couple is null ? NotFound("No active couple link.") : Ok(couple);
    }

    /// <summary>Create or join a couple using a shared couple code.</summary>
    [HttpPost("link")]
    [ProducesResponseType(typeof(CoupleResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Link([FromBody] LinkCoupleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            return Ok(await coupleService.LinkCoupleAsync(request));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    /// <summary>Unlink the couple.</summary>
    [HttpDelete("unlink/{userKey}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Unlink(string userKey)
    {
        var success = await coupleService.UnlinkCoupleAsync(userKey);
        return success ? NoContent() : NotFound("No active couple link to unlink.");
    }

    /// <summary>Get the couple dashboard — both users' today snapshot side-by-side.</summary>
    [HttpGet("dashboard/{userKey}")]
    [ProducesResponseType(typeof(CoupleDashboardResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetDashboard(string userKey)
    {
        var dashboard = await coupleService.GetCoupleDashboardAsync(userKey);
        return dashboard is null ? NotFound("Couple is not linked.") : Ok(dashboard);
    }
}
