/* [310] Quản lý thiết bị, in tem và lịch sử bàn giao.
   Chạy trên DB ứng dụng trước khi triển khai bản mới. Có thể chạy lại.
   Không thay đổi dữ liệu thiết bị hiện có hoặc cấp thêm quyền ngoài mô-đun 310. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.dt310_EquipmentInfo', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt310_EquipmentInfo
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        Code NVARCHAR(50) NOT NULL,
        DisplayNameVN NVARCHAR(255) NULL,
        DisplayNameTW NVARCHAR(255) NULL,
        DeptId NVARCHAR(20) NOT NULL,
        ManagerId NVARCHAR(20) NOT NULL,
        Note NVARCHAR(500) NULL,
        CreatedAt DATETIME NULL,
        CreatedBy NVARCHAR(20) NULL,
        DeletedAt DATETIME NULL,
        DeletedBy NVARCHAR(20) NULL
    );
END;

IF EXISTS (SELECT Code FROM dbo.dt310_EquipmentInfo WHERE DeletedAt IS NULL GROUP BY Code HAVING COUNT(*) > 1)
    THROW 51000, N'Có mã thiết bị đang sử dụng bị trùng. Cần xử lý dữ liệu trước khi triển khai.', 1;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.dt310_EquipmentInfo') AND name = N'UX_dt310_EquipmentInfo_ActiveCode')
    CREATE UNIQUE INDEX UX_dt310_EquipmentInfo_ActiveCode ON dbo.dt310_EquipmentInfo(Code) WHERE DeletedAt IS NULL;

IF OBJECT_ID(N'dbo.dt310_EquipmentHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt310_EquipmentHistory
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        EquipmentId INT NOT NULL,
        PreviousManagerId NVARCHAR(20) NOT NULL,
        NewManagerId NVARCHAR(20) NOT NULL,
        PreviousDeptId NVARCHAR(20) NOT NULL,
        NewDeptId NVARCHAR(20) NOT NULL,
        ChangedBy NVARCHAR(20) NOT NULL,
        ChangedAt DATETIME NOT NULL,
        HandoverId INT NULL,
        CONSTRAINT FK_dt310_EquipmentHistory_Equipment FOREIGN KEY (EquipmentId) REFERENCES dbo.dt310_EquipmentInfo(Id)
    );
    CREATE INDEX IX_dt310_EquipmentHistory_Equipment ON dbo.dt310_EquipmentHistory(EquipmentId, Id DESC);
END;

IF OBJECT_ID(N'dbo.vw310_EquipmentInfo', N'V') IS NULL
    EXEC(N'CREATE VIEW dbo.vw310_EquipmentInfo AS SELECT 1 AS Placeholder');
EXEC(N'ALTER VIEW dbo.vw310_EquipmentInfo AS
    SELECT e.Id, e.Code, e.DisplayNameVN, e.DisplayNameTW, e.DeptId,
           d.DisplayName AS DeptNameTW, d.DisplayNameVN AS DeptNameVN,
           e.ManagerId, u.DisplayName AS ManagerNameTW, u.DisplayNameVN AS ManagerNameVN,
           e.Note, e.CreatedAt, e.CreatedBy
    FROM dbo.dt310_EquipmentInfo e
    LEFT JOIN dbo.dm_Departments d ON d.Id = e.DeptId
    LEFT JOIN dbo.dm_User u ON u.Id = e.ManagerId
    WHERE e.DeletedAt IS NULL;');

DECLARE @ParentId INT;
SELECT TOP (1) @ParentId = IdParent FROM dbo.dm_Function
WHERE ControlName IN (N'uc310_Area5SResponsible', N'uc310_UnitEHSOrg', N'uc310_UpdateLeaveUser')
ORDER BY CASE ControlName WHEN N'uc310_Area5SResponsible' THEN 0 ELSE 1 END, Id;
IF @ParentId IS NULL
    THROW 51001, N'Không tìm thấy menu 310 để đăng ký chức năng quản lý thiết bị.', 1;

DECLARE @FunctionId INT;
SELECT TOP (1) @FunctionId = Id FROM dbo.dm_Function
WHERE ControlName = N'uc310_EquipmentInfo' AND IdParent = @ParentId ORDER BY Id;
IF @FunctionId IS NULL
BEGIN
    SELECT @FunctionId = ISNULL(MAX(Id), 0) + 1 FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);
    INSERT INTO dbo.dm_Function (Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
    SELECT @FunctionId, @ParentId, N'設備管理', N'uc310_EquipmentInfo', ISNULL(MAX(Prioritize), 0) + 1, 1, N'BO_Product.svg'
    FROM dbo.dm_Function WHERE IdParent = @ParentId;
END;
UPDATE dbo.dm_Function SET Status = 1 WHERE Id = @FunctionId;

INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
SELECT DISTINCT @FunctionId, fr.IdRole
FROM dbo.dm_FunctionRole fr
INNER JOIN dbo.dm_Function f ON f.Id = fr.IdFunction
WHERE f.IdParent = @ParentId AND f.ControlName IN (N'uc310_Area5SResponsible', N'uc310_UnitEHSOrg')
AND NOT EXISTS (SELECT 1 FROM dbo.dm_FunctionRole existing WHERE existing.IdFunction = @FunctionId AND existing.IdRole = fr.IdRole);

COMMIT TRANSACTION;
