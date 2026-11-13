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

IF EXISTS (SELECT 1 FROM dbo.dm_Function f
    JOIN (VALUES (40101,'EmployeeOffboardingView'),(40102,'EmployeeOffboardingUpload'),
                 (40103,'EmployeeOffboardingDownload')) p(Id,ControlName) ON f.Id = p.Id
    WHERE ISNULL(f.ControlName,'') <> p.ControlName)
    THROW 50001, N'離職文件權限代碼已被使用，請先確認設定。', 1;

DECLARE @ParentId int = (SELECT TOP (1) Id FROM dbo.dm_Function WHERE ControlName = 'uc401_UserManage' ORDER BY Id);
IF @ParentId IS NULL
    SET @ParentId = ISNULL((SELECT TOP (1) TRY_CONVERT(int,ValueT) FROM dbo.sys_StaticValue WHERE KeyT = 'RoleSysAdmin'),0);

DECLARE @NewPermissions TABLE (Id int NOT NULL PRIMARY KEY);

INSERT dbo.dm_Function(Id, IdParent, DisplayName, ControlName, Prioritize, Status)
OUTPUT inserted.Id INTO @NewPermissions(Id)
SELECT p.Id, @ParentId, p.DisplayName, p.ControlName, p.Id, 1
FROM (VALUES
    (40101,N'檢視離職文件','EmployeeOffboardingView'),
    (40102,N'上傳離職文件','EmployeeOffboardingUpload'),
    (40103,N'下載離職文件','EmployeeOffboardingDownload')
) p(Id,DisplayName,ControlName)
WHERE NOT EXISTS (SELECT 1 FROM dbo.dm_Function f WHERE f.Id = p.Id);

-- Cấp quyền ban đầu cho vai trò đang có quyền quản lý nhân viên.
-- Chỉ seed quyền vừa tạo; chạy lại không cấp lại những quyền đã bị quản trị thu hồi.
INSERT dbo.dm_FunctionRole(IdFunction,IdRole)
SELECT DISTINCT p.Id,r.IdRole
FROM dbo.dm_FunctionRole r
CROSS JOIN @NewPermissions p
WHERE (r.IdFunction = 33 OR r.IdFunction = @ParentId
    OR r.IdFunction IN (SELECT TRY_CONVERT(int,ValueT) FROM dbo.sys_StaticValue
                        WHERE KeyT IN ('RoleSysAdmin','RoleMod','RoleSafetyCertMain','RoleWorkManagementMain')))
    AND NOT EXISTS (SELECT 1 FROM dbo.dm_FunctionRole e WHERE e.IdFunction = p.Id AND e.IdRole = r.IdRole);

COMMIT TRANSACTION;
