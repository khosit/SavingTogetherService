using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Data;
using SavingChallengeService.DTOs;
using SavingChallengeService.Models;

namespace SavingChallengeService.Services;

public class CoupleService(AppDbContext db, DailyRecordService recordService) : ICoupleService
{
    public async Task<CoupleResponse?> GetCoupleAsync(string userKey)
    {
        var key = userKey.Trim().ToUpperInvariant();
        var couple = await db.Couples.FirstOrDefaultAsync(c => c.UserKeyA == key || c.UserKeyB == key);
        return couple is null ? null : Map(couple);
    }

    public async Task<CoupleResponse> LinkCoupleAsync(LinkCoupleRequest request)
    {
        var coupleCode = request.CoupleCode.Trim();
        var userKey = request.UserKey.Trim().ToUpperInvariant();

        var couple = await db.Couples.FirstOrDefaultAsync(c => c.CoupleCode == coupleCode);
        if (!await db.Users.AnyAsync(u => u.UserKey == userKey))
            throw new ArgumentException($"User '{userKey}' does not exist.");

        if (couple is null)
        {
            couple = new Couple
            {
                CoupleCode = coupleCode,
                UserKeyA   = userKey,
                IsLinked   = false,
                LinkedAt   = DateTime.UtcNow,
                UpdatedAt  = DateTime.UtcNow,
            };
            db.Couples.Add(couple);
        }
        else
        {
            if (couple.UserKeyA == userKey || couple.UserKeyB == userKey)
                throw new ArgumentException("This user is already linked to the couple.");
            if (couple.UserKeyB is not null)
                throw new ArgumentException("This couple code already has two users.");

            couple.UserKeyB  = userKey;
            couple.IsLinked  = true;
            couple.UpdatedAt  = DateTime.UtcNow;
        }
        await db.SaveChangesAsync();
        return Map(couple);
    }

    public async Task<bool> UnlinkCoupleAsync(string userKey)
    {
        var key = userKey.Trim().ToUpperInvariant();
        var couple = await db.Couples.FirstOrDefaultAsync(c => c.UserKeyA == key || c.UserKeyB == key);
        if (couple is null) return false;
        couple.UserKeyA = string.Empty;
        couple.UserKeyB = null;
        couple.IsLinked = false;
        couple.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<CoupleDashboardResponse?> GetCoupleDashboardAsync(string userKey)
    {
        var key = userKey.Trim().ToUpperInvariant();
        var couple = await db.Couples.FirstOrDefaultAsync(c => c.UserKeyA == key || c.UserKeyB == key);
        if (couple is null) return null;

        var userA = await db.Users.FirstOrDefaultAsync(u => u.UserKey == couple.UserKeyA);
        var userB = couple.UserKeyB is not null
            ? await db.Users.FirstOrDefaultAsync(u => u.UserKey == couple.UserKeyB)
            : null;

        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        var recA  = userA is not null ? await recordService.GetOrCreateRecordAsync(couple.UserKeyA, today) : null;
        var recB  = userB is not null ? await recordService.GetOrCreateRecordAsync(couple.UserKeyB!, today) : null;
        var streakA = await CalcStreakAsync(couple.UserKeyA);
        var streakB = userB is not null ? await CalcStreakAsync(couple.UserKeyB!) : 0;
        UserDailySummary? BuildSummary(Models.User? u, DailyRecordResponse? rec, int streak) =>
            u is null ? null : new UserDailySummary(
                u.UserKey, u.Name, u.Avatar,
                rec?.TotalSpent ?? 0,
                rec?.AvailableBudget ?? u.DailyBudget,
                rec?.Remaining ?? u.DailyBudget,
                rec?.IsOver ?? false,
                streak
            );

        var sumA = BuildSummary(userA, recA, streakA);
        var sumB = BuildSummary(userB, recB, streakB);

        var combinedSpent     = (sumA?.Spent ?? 0) + (sumB?.Spent ?? 0);
        var combinedBudget    = (sumA?.Budget ?? 0) + (sumB?.Budget ?? 0);
        var combinedRemaining = combinedBudget - combinedSpent;
        var isOver            = combinedRemaining < 0;

        string? winner = null;
        if (sumA is not null && sumB is not null)
            winner = sumA.Spent == sumB.Spent ? null : sumA.Spent < sumB.Spent ? sumA.UserKey : sumB.UserKey;

        return new CoupleDashboardResponse(
            Map(couple), sumA, sumB,
            combinedSpent, combinedBudget, combinedRemaining, isOver, winner
        );
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private async Task<int> CalcStreakAsync(string userKey)
    {
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
        return streak;
    }

    private static CoupleResponse Map(Couple c) =>
        new(c.Id, c.CoupleCode, c.UserKeyA, c.UserKeyB, c.IsLinked, c.LinkedAt, c.UpdatedAt);
}
