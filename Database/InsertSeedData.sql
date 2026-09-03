-- SQL Server sample data equivalent to Data/SeedData.cs.
-- Run Database/CreateTables.sql before running this script.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Now datetime2(7) = SYSUTCDATETIME();
DECLARE @Today date = CONVERT(date, @Now);

IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE UserKey = N'A')
BEGIN
    INSERT INTO dbo.Users
        (UserKey, Name, MonthlyIncome, FixedExpenses, SavingAmount, Avatar,
         DailyBudget, CreatedAt, UpdatedAt)
    VALUES
        (N'A', N'Alex', 6000.00, 1800.00, 1000.00, N'👨', 50.00,
         DATEADD(day, -30, @Now), @Now),
        (N'B', N'Sara', 5000.00, 1200.00, 800.00, N'👩', 51.67,
         DATEADD(day, -30, @Now), @Now);
END;

IF NOT EXISTS (SELECT 1 FROM dbo.Couples WHERE CoupleCode = N'ALEX&SARA')
BEGIN
    INSERT INTO dbo.Couples (CoupleCode, UserKeyA, UserKeyB, IsLinked, LinkedAt, UpdatedAt)
    VALUES (N'ALEX&SARA', N'A', N'B', 1, DATEADD(day, -25, @Now), @Now);
END;

;WITH Days AS
(
    SELECT -6 AS DayOffset
    UNION ALL SELECT -5
    UNION ALL SELECT -4
    UNION ALL SELECT -3
    UNION ALL SELECT -2
    UNION ALL SELECT -1
    UNION ALL SELECT 0
)
INSERT INTO dbo.DailyRecords
    (Date, UserKey, BaseBudget, CarryOver, AvailableBudget, UserId)
SELECT
    CONVERT(nvarchar(10), DATEADD(day, Days.DayOffset, @Today), 23),
    Users.UserKey,
    Users.DailyBudget,
    0.00,
    Users.DailyBudget,
    Users.Id
FROM Days
CROSS JOIN dbo.Users AS Users
WHERE Users.UserKey IN (N'A', N'B')
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.DailyRecords AS Existing
      WHERE Existing.UserKey = Users.UserKey
        AND Existing.Date = CONVERT(nvarchar(10), DATEADD(day, Days.DayOffset, @Today), 23)
  );

INSERT INTO dbo.Expenses (Amount, Category, Note, Time, DailyRecordId)
SELECT 5.50, N'food', N'Morning coffee', DATEADD(hour, 8, DATEADD(day, -6, CONVERT(datetime2, @Today))), Id
FROM dbo.DailyRecords WHERE UserKey = N'A' AND Date = CONVERT(nvarchar(10), DATEADD(day, -6, @Today), 23)
  AND NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE DailyRecordId = dbo.DailyRecords.Id);

INSERT INTO dbo.Expenses (Amount, Category, Note, Time, DailyRecordId)
SELECT 15.00, N'food', N'Lunch at mamak', DATEADD(hour, 12, DATEADD(day, -5, CONVERT(datetime2, @Today))), Id
FROM dbo.DailyRecords WHERE UserKey = N'A' AND Date = CONVERT(nvarchar(10), DATEADD(day, -5, @Today), 23)
  AND NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE DailyRecordId = dbo.DailyRecords.Id);

INSERT INTO dbo.Expenses (Amount, Category, Note, Time, DailyRecordId)
SELECT 6.00, N'food', N'Bubble tea', DATEADD(hour, 10, DATEADD(day, -6, CONVERT(datetime2, @Today))), Id
FROM dbo.DailyRecords WHERE UserKey = N'B' AND Date = CONVERT(nvarchar(10), DATEADD(day, -6, @Today), 23)
  AND NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE DailyRecordId = dbo.DailyRecords.Id);

INSERT INTO dbo.Expenses (Amount, Category, Note, Time, DailyRecordId)
SELECT 12.00, N'food', N'Lunch set', DATEADD(hour, 12, DATEADD(day, -5, CONVERT(datetime2, @Today))), Id
FROM dbo.DailyRecords WHERE UserKey = N'B' AND Date = CONVERT(nvarchar(10), DATEADD(day, -5, @Today), 23)
  AND NOT EXISTS (SELECT 1 FROM dbo.Expenses WHERE DailyRecordId = dbo.DailyRecords.Id);

COMMIT TRANSACTION;