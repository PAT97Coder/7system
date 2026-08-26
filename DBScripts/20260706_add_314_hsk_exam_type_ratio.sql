IF COL_LENGTH('dbo.dt314_HskExamMgmt', 'ExamType') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskExamMgmt
    ADD ExamType NVARCHAR(50) NOT NULL
        CONSTRAINT DF_dt314_HskExamMgmt_ExamType DEFAULT (N'模擬考試') WITH VALUES;
END
GO

IF COL_LENGTH('dbo.dt314_HskExamMgmt', 'HskRatio') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskExamMgmt
    ADD HskRatio NVARCHAR(20) NOT NULL
        CONSTRAINT DF_dt314_HskExamMgmt_HskRatio DEFAULT (N'9:1') WITH VALUES;
END
GO

UPDATE dbo.dt314_HskExamMgmt
SET
    ExamType = CASE
        WHEN Remark LIKE 'ExamType=%;HskRatio=%'
            THEN SUBSTRING(Remark, 10, CHARINDEX(';HskRatio=', Remark) - 10)
        ELSE ExamType
    END,
    HskRatio = CASE
        WHEN Remark LIKE 'ExamType=%;HskRatio=%'
            THEN SUBSTRING(Remark, CHARINDEX(';HskRatio=', Remark) + 10, 20)
        ELSE HskRatio
    END
WHERE Remark LIKE 'ExamType=%;HskRatio=%';
GO
