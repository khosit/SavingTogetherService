using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Data;
using SavingChallengeService.DTOs;

namespace SavingChallengeService.Services;

public class StatsService(AppDbContext db, DailyRecordService recordService) : IStatsService
{
    public async Task<StreakResponse?> GetStreakAsync(string userKey)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        if (user is null) return null;

        int streak = 0;
        var date   = DateTime.UtcNow.Date;
        while (true)
        {
            var dateStr = date.ToString("yyyy-MM-dd");
            var record  = await recordService.LoadRecordAsync(userKey, dateStr);
            if (record is null) break;
            var spent = record.Expenses.Sum(e => e.Amount);
            if (spent <= record.AvailableBudget) { streak++; date = date.AddDays(-1); }
            else break;
        }

        return new StreakResponse(userKey, streak, DateTime.UtcNow.Date.ToString("yyyy-MM-dd"));
    }

    public async Task<DashboardStatsResponse?> GetDashboardStatsAsync(string userKey)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        if (user is null) return null;

        var today     = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var record    = await recordService.GetOrCreateRecordAsync(userKey, today);
        var spent     = record?.TotalSpent ?? 0;
        var available = record?.AvailableBudget ?? user.DailyBudget;
        var remaining = available - spent;

        // Streak
        var streakResp = await GetStreakAsync(userKey);
        var streak     = streakResp?.CurrentStreak ?? 0;

        // Month-to-date
        var now          = DateTime.UtcNow;
        var monthRecords = await recordService.GetMonthRecordsAsync(userKey, now.Year, now.Month);
        var monthSpent   = monthRecords.Sum(r => r.TotalSpent);
        var target       = user.SavingAmount; // Monthly savings target is now based on the user's specified saving amount

        // Tomorrow's projected budget
        var tomorrowBudget = Math.Max(0, user.DailyBudget + remaining);

        return new DashboardStatsResponse(
            userKey, today,
            user.DailyBudget, available,
            spent, remaining, remaining < 0,
            record?.CarryOver ?? 0,
            tomorrowBudget, streak,
            monthSpent, target,
            monthRecords.Count()
        );
    }
}
