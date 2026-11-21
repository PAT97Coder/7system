-- Chọn database đích trong SSMS trước khi chạy.
-- Chỉ tạo hai bảng mới của module 312. Không sửa sáu bảng 312 hiện có.
-- Id int IDENTITY, thời gian datetime theo cách khai báo các bảng 312 hiện tại.
-- Không tạo khóa ngoại. Liên kết và kiểm tra người thi được xử lý tại BUS.
-- Có thể chạy lại; không xóa hoặc chuyển đổi dữ liệu/bảng đã tồn tại.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.dt312_ExternalParticipant', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt312_ExternalParticipant (
            Id int IDENTITY(1,1) NOT NULL,
            EmployeeCode nvarchar(64) NOT NULL,
            EmployeeName nvarchar(128) NOT NULL,
            Department nvarchar(128) NOT NULL,
            UpdatedAt datetime NOT NULL,
            CONSTRAINT PK_dt312_ExternalParticipant PRIMARY KEY (Id),
            CONSTRAINT UQ_dt312_ExternalParticipant_Code UNIQUE (EmployeeCode),
            CONSTRAINT CK_dt312_ExternalParticipant_Required CHECK (
                LEN(LTRIM(RTRIM(EmployeeCode))) > 0 AND LEN(LTRIM(RTRIM(EmployeeName))) > 0
                AND LEN(LTRIM(RTRIM(Department))) > 0)
        );
    END;

    IF OBJECT_ID(N'dbo.dt312_PracticeAttempt', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt312_PracticeAttempt (
            Id int IDENTITY(1,1) NOT NULL,
            PracticeKind varchar(8) NOT NULL,
            InternalUserId varchar(10) NULL,
            ExternalParticipantId int NULL,
            -- Per-attempt identity snapshot preserves history when names/departments change.
            EmployeeCode nvarchar(64) NOT NULL,
            EmployeeName nvarchar(128) NOT NULL,
            Department nvarchar(128) NOT NULL,
            StartedBy varchar(10) NOT NULL,
            StartedAt datetime NOT NULL,
            SubmitAt datetime NULL,
            Score int NULL,
            ExamData nvarchar(max) NULL,
            CONSTRAINT PK_dt312_PracticeAttempt PRIMARY KEY (Id),
            CONSTRAINT CK_dt312_PracticeAttempt_Kind CHECK (
                (PracticeKind = 'INTERNAL' AND InternalUserId IS NOT NULL AND ExternalParticipantId IS NULL)
                OR (PracticeKind = 'EXTERNAL' AND InternalUserId IS NULL AND ExternalParticipantId IS NOT NULL)),
            CONSTRAINT CK_dt312_PracticeAttempt_Score CHECK (Score IS NULL OR Score BETWEEN 0 AND 100),
            CONSTRAINT CK_dt312_PracticeAttempt_Submission CHECK (
                (SubmitAt IS NULL AND Score IS NULL AND ExamData IS NULL)
                OR (SubmitAt IS NOT NULL AND ExamData IS NOT NULL AND SubmitAt >= StartedAt))
        );
    END;

    -- Nếu đã chạy bản nháp dùng Guid trước đây, dừng để kiểm tra; không tự đổi khóa/xóa bảng.
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.dt312_PracticeAttempt')
        AND name = N'Id' AND system_type_id = TYPE_ID(N'int') AND is_identity = 1)
        THROW 51000, N'dt312_PracticeAttempt đã tồn tại với Id khác int IDENTITY. Không tự sửa bảng; cần kiểm tra cấu trúc hiện có.', 1;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'dbo.dt312_ExternalParticipant')
        AND name = N'Id' AND system_type_id = TYPE_ID(N'int') AND is_identity = 1)
        THROW 51000, N'dt312_ExternalParticipant đã tồn tại với Id khác int IDENTITY. Cần kiểm tra cấu trúc hiện có.', 1;
    IF EXISTS (SELECT 1 FROM sys.foreign_keys
        WHERE parent_object_id IN (OBJECT_ID(N'dbo.dt312_ExternalParticipant'), OBJECT_ID(N'dbo.dt312_PracticeAttempt')))
        THROW 51000, N'Hai bảng thi thử đã tồn tại khóa ngoại từ bản khác. Script không tự xóa ràng buộc hiện có; cần kiểm tra trước.', 1;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.dt312_PracticeAttempt') AND name = N'IX_dt312_PracticeAttempt_History')
        CREATE INDEX IX_dt312_PracticeAttempt_History ON dbo.dt312_PracticeAttempt(PracticeKind, SubmitAt DESC)
            INCLUDE (EmployeeCode, EmployeeName, Department, Score, StartedBy);
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.dt312_PracticeAttempt') AND name = N'IX_dt312_PracticeAttempt_External')
        CREATE INDEX IX_dt312_PracticeAttempt_External ON dbo.dt312_PracticeAttempt(ExternalParticipantId, StartedAt);
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
