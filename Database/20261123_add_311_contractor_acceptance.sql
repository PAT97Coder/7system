SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.dt311_ContractorAcceptance', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt311_ContractorAcceptance
    (
        DocumentNumber nvarchar(100) COLLATE Latin1_General_100_CI_AS NOT NULL PRIMARY KEY,
        Period varchar(7) NOT NULL,
        ReportDate date NOT NULL,
        CostCenter nvarchar(100) NOT NULL,
        ContractNumber nvarchar(100) NOT NULL,
        Contractor nvarchar(255) NOT NULL,
        Adjustment decimal(28,8) NOT NULL,
        BeforeTax decimal(28,8) NOT NULL,
        Tax decimal(28,8) NOT NULL,
        Total decimal(28,8) NOT NULL,
        SourceFileName nvarchar(255) NOT NULL,
        SheetName nvarchar(31) NOT NULL,
        CreatedByUserId varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL REFERENCES dbo.dm_User(Id),
        UpdatedByUserId varchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL REFERENCES dbo.dm_User(Id),
        CreatedUtc datetime2(3) NOT NULL,
        UpdatedUtc datetime2(3) NOT NULL,
        Version rowversion NOT NULL,
        CONSTRAINT CK_311Acceptance_Totals CHECK (BeforeTax >= 0 AND Tax >= 0 AND Total = BeforeTax + Tax)
    );
END;

IF OBJECT_ID(N'dbo.dt311_ContractorAcceptanceItem', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt311_ContractorAcceptanceItem
    (
        DocumentNumber nvarchar(100) COLLATE Latin1_General_100_CI_AS NOT NULL
            REFERENCES dbo.dt311_ContractorAcceptance(DocumentNumber) ON DELETE CASCADE,
        ExcelRow int NOT NULL,
        Description nvarchar(500) NOT NULL,
        WorkCode nvarchar(100) NOT NULL,
        Location nvarchar(255) NOT NULL,
        WorkPeriod nvarchar(100) NOT NULL,
        Unit nvarchar(100) NOT NULL,
        Quantity decimal(28,8) NOT NULL,
        UnitPrice decimal(28,8) NOT NULL,
        Currency nvarchar(10) NOT NULL,
        CONSTRAINT PK_311AcceptanceItem PRIMARY KEY (DocumentNumber, ExcelRow),
        CONSTRAINT CK_311AcceptanceItem_Values CHECK (ExcelRow >= 9 AND Quantity > 0 AND UnitPrice >= 0)
    );
END;

IF OBJECT_ID(N'dbo.dt311_ContractorAcceptanceFile', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt311_ContractorAcceptanceFile
    (
        DocumentNumber nvarchar(100) COLLATE Latin1_General_100_CI_AS NOT NULL PRIMARY KEY
            REFERENCES dbo.dt311_ContractorAcceptance(DocumentNumber) ON DELETE CASCADE,
        Content varbinary(max) NOT NULL,
        CONSTRAINT CK_311AcceptanceFile_Size CHECK (DATALENGTH(Content) BETWEEN 1 AND 10485760)
    );
END;

COMMIT TRANSACTION;
