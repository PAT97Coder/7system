-- Apply before deploying the application with question-type ratios.
-- Store the choice percentage only; essay percentage is always 100 minus it.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.dt312_Setting', N'U') IS NULL
    THROW 51000, 'Module 312 settings table does not exist.', 1;
IF COL_LENGTH(N'dbo.dt312_Setting', N'ChoicePercent') IS NULL
    ALTER TABLE dbo.dt312_Setting ADD ChoicePercent int NOT NULL
        CONSTRAINT DF_dt312_Setting_ChoicePercent DEFAULT (70) WITH VALUES;
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE parent_object_id = OBJECT_ID(N'dbo.dt312_Setting') AND name = N'CK_dt312_Setting_ChoicePercent')
    EXEC(N'ALTER TABLE dbo.dt312_Setting WITH CHECK ADD CONSTRAINT CK_dt312_Setting_ChoicePercent CHECK (ChoicePercent BETWEEN 0 AND 100);');
COMMIT TRANSACTION;
