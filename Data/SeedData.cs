using SavingChallengeService.Models;

namespace SavingChallengeService.Data;

/// <summary>
/// Seeds the database with realistic dummy data so the API
/// returns meaningful responses right from the first run.
/// </summary>
public static class SeedData
{
    public static void Initialize(AppDbContext db)
    {
        if (db.Users.Any()) return; // already seeded

        // ── Users ────────────────────────────────────────────────────────
        static decimal CalcDailyBudget(decimal income, decimal fixed_) =>
            Math.Max(0, (income * 0.55m - fixed_) / 30m);

        var userA = new User
        {
            UserKey       = "A",
            Name          = "Alex",
            Avatar        = "👨",
            MonthlyIncome = 6000m,
            FixedExpenses = 1800m,
            SavingAmount  = 1000m,
            DailyBudget   = CalcDailyBudget(6000m, 1800m),
            CreatedAt     = DateTime.UtcNow.AddDays(-30),
            UpdatedAt     = DateTime.UtcNow,
        };

        var userB = new User
        {
            UserKey       = "B",
            Name          = "Sara",
            Avatar        = "👩",
            MonthlyIncome = 5000m,
            FixedExpenses = 1200m,
            SavingAmount  = 800m,
            DailyBudget   = CalcDailyBudget(5000m, 1200m),
            CreatedAt     = DateTime.UtcNow.AddDays(-30),
            UpdatedAt     = DateTime.UtcNow,
        };

        db.Users.AddRange(userA, userB);
        db.SaveChanges();

        // ── Couple ───────────────────────────────────────────────────────
        var couple = new Couple
        {
            CoupleCode = "ALEX&SARA",
            UserKeyA   = userA.UserKey,
            UserKeyB   = userB.UserKey,
            IsLinked   = true,
            LinkedAt   = DateTime.UtcNow.AddDays(-25),
            UpdatedAt  = DateTime.UtcNow,
        };
        db.Couples.Add(couple);
        db.SaveChanges();

        // ── Daily Records (last 7 days for each user) ─────────────────────
        var rand = new Random(42);
        var today = DateTime.UtcNow.Date;

        foreach (var (user, seedExpenses) in new[]
        {
            (userA, SeedExpenseSets.Alex),
            (userB, SeedExpenseSets.Sara),
        })
        {
            decimal carry = 0m;

            for (int dayOffset = -6; dayOffset <= 0; dayOffset++)
            {
                var date       = today.AddDays(dayOffset);
                var dateStr    = date.ToString("yyyy-MM-dd");
                var available  = user.DailyBudget + carry;

                var record = new DailyRecord
                {
                    Date            = dateStr,
                    UserKey         = user.UserKey,
                    BaseBudget      = user.DailyBudget,
                    CarryOver       = carry,
                    AvailableBudget = available,
                    UserId          = user.Id,
                };
                db.DailyRecords.Add(record);
                db.SaveChanges();

                // Pick 1-4 random expenses for this day
                var dayExpenses = seedExpenses
                    .OrderBy(_ => rand.Next())
                    .Take(rand.Next(1, 5))
                    .Select(e => new Expense
                    {
                        Amount        = e.Amount,
                        Category      = e.Category,
                        Note          = e.Note,
                        Time          = date.AddHours(rand.Next(8, 21)).ToUniversalTime(),
                        DailyRecordId = record.Id,
                    })
                    .ToList();

                db.Expenses.AddRange(dayExpenses);
                db.SaveChanges();

                var spent = dayExpenses.Sum(e => e.Amount);
                carry = available - spent; // positive = surplus, negative = deficit
            }
        }
    }
}

// ── Seed expense templates ───────────────────────────────────────────────────
file static class SeedExpenseSets
{
    public static readonly (decimal Amount, string Category, string? Note)[] Alex =
    [
        (5.50m,  "food",          "Morning coffee"),
        (15.00m, "food",          "Lunch at mamak"),
        (35.00m, "food",          "Groceries"),
        (20.00m, "transport",     "Grab to office"),
        (8.00m,  "transport",     "Parking"),
        (12.00m, "transport",     "Touch-n-go reload"),
        (45.00m, "shopping",      "New t-shirt"),
        (18.00m, "entertainment", "Netflix month"),
        (9.90m,  "entertainment", "Movie ticket"),
        (30.00m, "health",        "Pharmacy"),
        (60.00m, "bills",         "Electricity bill"),
        (10.00m, "other",         "Birthday card"),
    ];

    public static readonly (decimal Amount, string Category, string? Note)[] Sara =
    [
        (6.00m,  "food",          "Bubble tea"),
        (12.00m, "food",          "Lunch set"),
        (28.00m, "food",          "Supermarket"),
        (15.00m, "transport",     "Grab ride"),
        (5.00m,  "transport",     "Parking"),
        (55.00m, "shopping",      "Online shopping"),
        (22.00m, "shopping",      "Skincare"),
        (14.90m, "entertainment", "Spotify + streaming"),
        (25.00m, "health",        "Vitamins"),
        (80.00m, "bills",         "Water + internet"),
        (8.00m,  "other",         "Misc"),
    ];
}
