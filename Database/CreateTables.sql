-- SQL Server schema for SavingChallengeService.

IF OBJECT_ID(N'dbo.Expenses', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Expenses
    (
        Id            int IDENTITY(1, 1) NOT NULL,
        Amount        decimal(18, 2) NOT NULL,
        Category      nvarchar(50) NOT NULL CONSTRAINT DF_Expenses_Category DEFAULT N'other',
        Note          nvarchar(500) NULL,
        Time          datetime2(7) NOT NULL,
        DailyRecordId int NOT NULL,
        CONSTRAINT PK_Expenses PRIMARY KEY (Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.DailyRecords', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DailyRecords
    (
        Id              int IDENTITY(1, 1) NOT NULL,
        Date            nvarchar(10) NOT NULL,
        UserKey         nvarchar(50) NOT NULL,
        BaseBudget      decimal(18, 2) NOT NULL,
        CarryOver       decimal(18, 2) NOT NULL,
        AvailableBudget decimal(18, 2) NOT NULL,
        UserId          int NOT NULL,
        CONSTRAINT PK_DailyRecords PRIMARY KEY (Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Users
    (
        Id             int IDENTITY(1, 1) NOT NULL,
        UserKey        nvarchar(50) NOT NULL,
        Name           nvarchar(200) NOT NULL,
        MonthlyIncome  decimal(18, 2) NOT NULL,
        FixedExpenses  decimal(18, 2) NOT NULL,
        SavingAmount   decimal(18, 2) NOT NULL,
        Avatar         nvarchar(10) NOT NULL,
        DailyBudget    decimal(18, 2) NOT NULL,
        CreatedAt      datetime2(7) NOT NULL,
        UpdatedAt      datetime2(7) NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY (Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Couples', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Couples
    (
        Id         int IDENTITY(1, 1) NOT NULL,
        CoupleCode nvarchar(100) NOT NULL,
        UserKeyA   nvarchar(50) NOT NULL,
        UserKeyB   nvarchar(50) NULL,
        IsLinked   bit NOT NULL,
        LinkedAt   datetime2(7) NOT NULL,
        UpdatedAt  datetime2(7) NOT NULL,
        CONSTRAINT PK_Couples PRIMARY KEY (Id)
    );
END;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Users_UserKey' AND object_id = OBJECT_ID(N'dbo.Users')
)
    CREATE UNIQUE INDEX IX_Users_UserKey ON dbo.Users(UserKey);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_DailyRecords_UserKey_Date' AND object_id = OBJECT_ID(N'dbo.DailyRecords')
)
    CREATE UNIQUE INDEX IX_DailyRecords_UserKey_Date ON dbo.DailyRecords(UserKey, Date);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Couples_CoupleCode' AND object_id = OBJECT_ID(N'dbo.Couples')
)
    CREATE INDEX IX_Couples_CoupleCode ON dbo.Couples(CoupleCode);
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_DailyRecords_Users_UserId'
)
    ALTER TABLE dbo.DailyRecords
        ADD CONSTRAINT FK_DailyRecords_Users_UserId
        FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE;
GO

IF NOT EXISTS
(
    SELECT 1 FROM sys.foreign_keys
    WHERE name = N'FK_Expenses_DailyRecords_DailyRecordId'
)
    ALTER TABLE dbo.Expenses
        ADD CONSTRAINT FK_Expenses_DailyRecords_DailyRecordId
        FOREIGN KEY (DailyRecordId) REFERENCES dbo.DailyRecords(Id) ON DELETE CASCADE;
GO