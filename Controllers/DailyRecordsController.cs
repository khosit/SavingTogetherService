using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.DTOs;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Read daily budget records and monthly summaries.</summary>
[ApiController]
[Route("api/users/{userKey}/records")]
[Produces("application/json")]
public class DailyRecordsController(IDailyRecordService recordService) : ControllerBase
{
    /// <summary>Get (or auto-create) today's record for a user.</summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(DailyRecordResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetToday(string userKey)
    {
        var record = await recordService.GetOrCreateTodayRecordAsync(userKey.ToUpperInvariant());
        return record is null ? NotFound($"User '{userKey}' not found.") : Ok(record);
    }

    /// <summary>Get a record by specific date (YYYY-MM-DD).</summary>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(DailyRecordResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByDate(string userKey, string date)
    {
        var record = await recordService.GetRecordByDateAsync(userKey.ToUpperInvariant(), date);
        return record is null ? NotFound() : Ok(record);
    }

    /// <summary>Get all records for a user.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DailyRecordResponse>), 200)]
    public async Task<IActionResult> GetAll(string userKey) =>
        Ok(await recordService.GetAllRecordsAsync(userKey.ToUpperInvariant()));

    /// <summary>Get records for a specific month.</summary>
    [HttpGet("month/{year:int}/{month:int}")]
    [ProducesResponseType(typeof(IEnumerable<DailyRecordResponse>), 200)]
    public async Task<IActionResult> GetMonth(string userKey, int year, int month) =>
        Ok(await recordService.GetMonthRecordsAsync(userKey.ToUpperInvariant(), year, month));

    /// <summary>Get monthly summary including category breakdown.</summary>
    [HttpGet("month/{year:int}/{month:int}/summary")]
    [ProducesResponseType(typeof(MonthSummaryResponse), 200)]
    public async Task<IActionResult> GetMonthSummary(string userKey, int year, int month) =>
        Ok(await recordService.GetMonthSummaryAsync(userKey.ToUpperInvariant(), year, month));
}
