-- Chạy trước khi triển khai phiên bản ứng dụng có phân loại tài liệu.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF COL_LENGTH('dbo.dt209_Base', 'Articles') IS NULL
    ALTER TABLE dbo.dt209_Base ADD Articles nvarchar(512) NULL;
IF COL_LENGTH('dbo.dt209_Base', 'DocumentCategory') IS NULL
    ALTER TABLE dbo.dt209_Base ADD DocumentCategory nvarchar(16) NULL;
GO
-- Giữ nội dung điều khoản hiện có; không ghi đè nội dung nhập tay.
UPDATE b SET Articles = r.Articles
FROM dbo.dt209_Base b
JOIN dbo.dt209_RecordCode r ON b.IdRecordCode = r.Id
WHERE b.Articles IS NULL;
IF NOT EXISTS (SELECT 1 FROM dbo.sys_StaticValue WHERE KeyT = '209DocTypes')
    INSERT dbo.sys_StaticValue(KeyT, ValueT) VALUES ('209DocTypes', N'一階;二階;三階;四階');
ELSE
    UPDATE dbo.sys_StaticValue
    SET ValueT = CASE WHEN NULLIF(ValueT, N'') IS NULL THEN N'一階;二階;三階;四階' ELSE ValueT + N';四階' END
    WHERE KeyT = '209DocTypes' AND N';' + ISNULL(ValueT, N'') + N';' NOT LIKE N'%;四階;%';
COMMIT TRANSACTION;
