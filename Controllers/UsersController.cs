using Microsoft.AspNetCore.Mvc;
using SavingChallengeService.DTOs;
using SavingChallengeService.Services;

namespace SavingChallengeService.Controllers;

/// <summary>Manage user profiles (Partner A and Partner B).</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>Get all users.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), 200)]
    public async Task<IActionResult> GetAll() =>
        Ok(await userService.GetAllUsersAsync());

    /// <summary>Create a new user profile.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var user = await userService.CreateUserAsync(request);
            return CreatedAtAction(nameof(GetByKey), new { userKey = user.UserKey }, user);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    /// <summary>Get a single user by key ("A" or "B").</summary>
    [HttpGet("{userKey}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByKey(string userKey)
    {
        var user = await userService.GetUserAsync(userKey.ToUpperInvariant());
        return user is null ? NotFound($"User '{userKey}' not found.") : Ok(user);
    }

    /// <summary>Create or update a user profile.</summary>
    [HttpPut("{userKey}")]
    [ProducesResponseType(typeof(UserResponse), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> CreateOrUpdate(string userKey, [FromBody] CreateOrUpdateUserRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var key = userKey.Trim().ToUpperInvariant();

        var user = await userService.CreateOrUpdateUserAsync(key, request);
        return Ok(user);
    }
}
