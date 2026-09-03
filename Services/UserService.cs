using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Data;
using SavingChallengeService.DTOs;
using SavingChallengeService.Models;

namespace SavingChallengeService.Services;

public class UserService(AppDbContext db) : IUserService
{
    public async Task<UserResponse?> GetUserAsync(string userKey)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await db.Users.ToListAsync();
        return users.Select(MapToResponse);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var userKey = request.UserKey.Trim().ToUpperInvariant();
        if (await db.Users.AnyAsync(u => u.UserKey == userKey))
            throw new InvalidOperationException($"User '{userKey}' already exists.");

        var daily = BudgetCalculator.CalcDailyBudget(request.MonthlyIncome, request.FixedExpenses, request.SavingAmount);
        var user = new User
        {
            UserKey       = userKey,
            Name          = request.Name.Trim(),
            MonthlyIncome = request.MonthlyIncome,
            FixedExpenses = request.FixedExpenses,
            SavingAmount  = request.SavingAmount,
            Avatar        = request.Avatar,
            DailyBudget   = daily,
            CreatedAt     = DateTime.UtcNow,
            UpdatedAt     = DateTime.UtcNow,
        };

        db.Users.Add(user);
        await db.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateOrUpdateUserAsync(string userKey, CreateOrUpdateUserRequest request)
    {
        var daily = BudgetCalculator.CalcDailyBudget(request.MonthlyIncome, request.FixedExpenses, request.SavingAmount);

        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        if (user is null)
        {
            user = new User
            {
                UserKey       = userKey,
                Name          = request.Name,
                MonthlyIncome = request.MonthlyIncome,
                FixedExpenses = request.FixedExpenses,
                SavingAmount  = request.SavingAmount,
                Avatar        = request.Avatar,
                DailyBudget   = daily,
                CreatedAt     = DateTime.UtcNow,
                UpdatedAt     = DateTime.UtcNow,
            };
            db.Users.Add(user);
        }
        else
        {
            user.Name          = request.Name;
            user.MonthlyIncome = request.MonthlyIncome;
            user.FixedExpenses = request.FixedExpenses;
            user.SavingAmount  = request.SavingAmount;
            user.Avatar        = request.Avatar;
            user.DailyBudget   = daily;
            user.UpdatedAt     = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User u) => new(
        u.Id, u.UserKey, u.Name, u.MonthlyIncome, u.FixedExpenses, u.SavingAmount, u.Avatar, u.DailyBudget, u.CreatedAt, u.UpdatedAt
    );
}
