-- Chạy trên database đích trước khi triển khai ứng dụng.
-- Script có thể chạy lại; không sửa hoặc xóa hồ sơ nhân viên hiện hữu.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.EmployeeOffboarding', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmployeeOffboarding
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmployeeOffboarding PRIMARY KEY,
        UserId varchar(10) NOT NULL,
        Reason nvarchar(2000) NOT NULL,
        ResignDate date NULL,
        DepartmentId varchar(4) NULL,
        JobCode varchar(32) NULL,
        DisplayNameTW nvarchar(256) NULL,
        DisplayNameVN nvarchar(256) NULL,
        CreatedByUserId varchar(10) NOT NULL,
        CreatedAt datetime2(7) NOT NULL,
        CONSTRAINT FK_EmployeeOffboarding_User FOREIGN KEY (UserId) REFERENCES dbo.dm_User(Id),
        CONSTRAINT FK_EmployeeOffboarding_Creator FOREIGN KEY (CreatedByUserId) REFERENCES dbo.dm_User(Id)
    );
    CREATE INDEX IX_EmployeeOffboarding_UserId_CreatedAt ON dbo.EmployeeOffboarding(UserId, CreatedAt DESC);
END;

IF OBJECT_ID(N'dbo.EmployeeOffboardingFile', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.EmployeeOffboardingFile
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_EmployeeOffboardingFile PRIMARY KEY,
        OffboardingId int NOT NULL,
        OriginalName nvarchar(256) NOT NULL,
        StoredPath nvarchar(512) NOT NULL,
        FileSize bigint NOT NULL,
        CreatedByUserId varchar(10) NOT NULL,
        CreatedAt datetime2(7) NOT NULL,
        CONSTRAINT FK_EmployeeOffboardingFile_Record FOREIGN KEY (OffboardingId) REFERENCES dbo.EmployeeOffboarding(Id),
        CONSTRAINT FK_EmployeeOffboardingFile_Creator FOREIGN KEY (CreatedByUserId) REFERENCES dbo.dm_User(Id),
        CONSTRAINT CK_EmployeeOffboardingFile_Size CHECK (FileSize > 0 AND FileSize <= 104857600)
    );
    CREATE INDEX IX_EmployeeOffboardingFile_OffboardingId ON dbo.EmployeeOffboardingFile(OffboardingId);
END;

COMMIT TRANSACTION;
