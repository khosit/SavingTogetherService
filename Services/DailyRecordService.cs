using Microsoft.EntityFrameworkCore;
using SavingChallengeService.Data;
using SavingChallengeService.DTOs;
using SavingChallengeService.Models;

namespace SavingChallengeService.Services;

public class DailyRecordService(AppDbContext db) : IDailyRecordService
{
    // ── Public API ────────────────────────────────────────────────────────

    public async Task<DailyRecordResponse?> GetOrCreateTodayRecordAsync(string userKey)
    {
        var today = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
        return await GetOrCreateRecordAsync(userKey, today);
    }

    public async Task<DailyRecordResponse?> GetRecordByDateAsync(string userKey, string date)
    {
        var record = await LoadRecordAsync(userKey, date);
        return record is null ? null : MapToResponse(record);
    }

    public async Task<IEnumerable<DailyRecordResponse>> GetMonthRecordsAsync(string userKey, int year, int month)
    {
        var prefix = $"{year}-{month:D2}";
        var records = await db.DailyRecords
            .Include(r => r.Expenses)
            .Where(r => r.UserKey == userKey && r.Date.StartsWith(prefix))
            .OrderBy(r => r.Date)
            .ToListAsync();

        return records.Select(MapToResponse);
    }

    public async Task<IEnumerable<DailyRecordResponse>> GetAllRecordsAsync(string userKey)
    {
        var records = await db.DailyRecords
            .Include(r => r.Expenses)
            .Where(r => r.UserKey == userKey)
            .OrderBy(r => r.Date)
            .ToListAsync();

        return records.Select(MapToResponse);
    }

    public async Task<MonthSummaryResponse> GetMonthSummaryAsync(string userKey, int year, int month)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        var records = (await GetMonthRecordsAsync(userKey, year, month)).ToList();

        var totalSpent  = records.Sum(r => r.TotalSpent);
        var totalBudget = records.Sum(r => r.AvailableBudget);
        var totalSaved  = totalBudget - totalSpent;
        var daysUnder   = records.Count(r => !r.IsOver);
        var avgDaily    = records.Count > 0 ? totalSpent / records.Count : 0;
        var target      = user is not null ? user.SavingAmount : 0;

        // Category breakdown
        var allExpenses = records.SelectMany(r => r.Expenses).ToList();
        var catGroups   = allExpenses
            .GroupBy(e => e.Category)
            .Select(g => new
            {
                CategoryId = g.Key,
                Amount     = g.Sum(e => e.Amount),
            })
            .OrderByDescending(x => x.Amount)
            .ToList();

        var cats = CategoryMeta.All;
        var breakdown = catGroups.Select(g =>
        {
            var meta = cats.FirstOrDefault(c => c.Id == g.CategoryId) ?? cats.Last();
            var pct  = totalSpent > 0 ? (double)(g.Amount / totalSpent * 100) : 0;
            return new CategoryBreakdown(meta.Id, meta.Label, meta.Icon, meta.Color, g.Amount, Math.Round(pct, 1));
        });

        return new MonthSummaryResponse(
            year, month, userKey,
            totalSpent, totalBudget, totalSaved,
            records.Count, daysUnder,
            Math.Round(avgDaily, 2), target,
            breakdown, records
        );
    }

    // ── Internal helpers ──────────────────────────────────────────────────

    internal async Task<DailyRecordResponse?> GetOrCreateRecordAsync(string userKey, string date)
    {
        var existing = await LoadRecordAsync(userKey, date);
        if (existing is not null) return MapToResponse(existing);

        var user = await db.Users.FirstOrDefaultAsync(u => u.UserKey == userKey);
        if (user is null) return null;

        // Compute carry-over from yesterday
        var yesterday = DateTime.Parse(date).AddDays(-1).ToString("yyyy-MM-dd");
        var ydRecord  = await LoadRecordAsync(userKey, yesterday);
        decimal carry = 0;

        if(DateTime.Parse(date).Day == 1)
        {
            // Reset carry-over at the start of a new month
            carry = 0;
        }
        else if (ydRecord is not null)
        {
            var ydSpent = ydRecord.Expenses.Sum(e => e.Amount);
            carry = ydRecord.AvailableBudget - ydSpent;
        }

        var record = new DailyRecord
        {
            Date            = date,
            UserKey         = userKey,
            BaseBudget      = user.DailyBudget,
            CarryOver       = carry,
            AvailableBudget = user.DailyBudget + carry,
            UserId          = user.Id,
        };
        db.DailyRecords.Add(record);
        await db.SaveChangesAsync();

        return MapToResponse(record);
    }

    internal async Task<DailyRecord?> LoadRecordAsync(string userKey, string date) =>
        await db.DailyRecords
            .Include(r => r.Expenses)
            .FirstOrDefaultAsync(r => r.UserKey == userKey && r.Date == date);

    internal static DailyRecordResponse MapToResponse(DailyRecord r)
    {
        var spent = r.Expenses.Sum(e => e.Amount);
        return new DailyRecordResponse(
            r.Id, r.Date, r.UserKey,
            r.BaseBudget, r.CarryOver, r.AvailableBudget,
            spent, r.AvailableBudget - spent, spent > r.AvailableBudget,
            r.Expenses.Select(e => new ExpenseResponse(e.Id, e.Amount, e.Category, e.Note, e.Time, e.DailyRecordId))
        );
    }
}
