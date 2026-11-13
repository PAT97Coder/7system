-- Bổ sung câu tự luận module 312. Chạy trước khi triển khai bản ứng dụng mới.
-- Có thể chạy lại; giữ nguyên loại và đáp án của các câu trắc nghiệm hiện có.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.dt312_Questions', N'U') IS NULL
    THROW 51000, N'找不到題庫資料表，請先安裝模組312。', 1;
IF COL_LENGTH(N'dbo.dt312_Questions', N'IsEssay') IS NULL
    ALTER TABLE dbo.dt312_Questions ADD IsEssay bit NOT NULL
        CONSTRAINT DF_dt312_Questions_IsEssay DEFAULT (0) WITH VALUES;
IF COL_LENGTH(N'dbo.dt312_Questions', N'ReferenceAnswer') IS NULL
    ALTER TABLE dbo.dt312_Questions ADD ReferenceAnswer nvarchar(max) NULL;
COMMIT TRANSACTION;
