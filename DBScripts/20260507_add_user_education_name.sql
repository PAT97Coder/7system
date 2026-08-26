IF COL_LENGTH('dbo.dm_User', 'EducationName') IS NULL
BEGIN
    ALTER TABLE dbo.dm_User
    ADD EducationName NVARCHAR(64) NULL;
END
GO
