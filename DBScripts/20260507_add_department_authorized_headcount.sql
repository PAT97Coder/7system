IF COL_LENGTH('dbo.dm_Departments', 'AuthorizedHeadcount') IS NULL
BEGIN
    ALTER TABLE dbo.dm_Departments
    ADD AuthorizedHeadcount INT NULL;
END
GO
