IF COL_LENGTH('dbo.dm_Departments', 'IsActive') IS NULL
BEGIN
    ALTER TABLE dbo.dm_Departments
    ADD IsActive BIT NOT NULL
        CONSTRAINT DF_dm_Departments_IsActive DEFAULT (1);
END
GO

UPDATE dbo.dm_Departments
SET IsActive = 0
WHERE Id IN (
    '7020',
    '7120',
    '7140',
    '72',
    '7200',
    '7210',
    '7220',
    '7230',
    '7840'
);
GO
