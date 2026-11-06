/*
MODULE 320 - 稽核管理系統
Script tạo mới; chạy toàn bộ trong SSMS trên server cần dùng.
Database: DBDocumentManagementSystem. SQL Server 2012 trở lên.

Chỉ lưu trữ: mỗi bảng có khóa chính Id tự tăng.
Không tạo khóa ngoại, CHECK, UNIQUE, DEFAULT, trigger, procedure hoặc view.
Các cột bắt buộc vẫn dùng NOT NULL. Thời gian/người tạo do code truyền vào.
Mọi liên kết, kiểm tra mã, chống trùng và xử lý nghiệp vụ thực hiện bằng code/LINQ.
Không sửa, xóa hoặc chuyển dữ liệu module 316. Không tạo dữ liệu nghiệp vụ mẫu.
dt320_PlanUser lưu người tham gia theo từng đơn vị/nhóm trong kế hoạch.
Tên và chức vụ được code/LINQ tra từ danh mục nhân sự dùng chung.

Quy ước để code sử dụng (SQL KHÔNG kiểm tra các giá trị này):
- CycleType: M = Tháng/每月, H = Nửa năm/每半年, Q = Quý/每季, Y = Năm/每年.
- PeriodYear: năm thực hiện.
- PeriodNo: tháng 1..12, nửa năm 1..2, quý 1..4, năm = 1.
- AuditAt: thời gian kiểm tra, độc lập với kỳ báo cáo.
- Plan.CycleType: lưu chu kỳ tại thời điểm lập kế hoạch, không tự cập nhật
  khi chu kỳ của hạng mục thay đổi.
- ParticipationType: 1 = 督導單位, 2 = 陪集合 (giữ tên theo yêu cầu).
  Mỗi kế hoạch có nhiều đơn vị trong mỗi nhóm. Code quyết định và kiểm tra
  việc một đơn vị có được nằm trong cả hai nhóm hay không.
- Code lọc dm_Departments.Id bắt đầu bằng 7 khi chọn đơn vị.
- IdAuditItem, IdPlan, IdPlanDepartment, IdReport, IdAdt, IdDept, CreateBy,
  RemoveBy là các giá trị tham chiếu thông thường, KHÔNG phải khóa ngoại.
- RemoveAt/RemoveBy: đánh dấu xóa; code xử lý việc ẩn, sửa và xóa dữ liệu liên quan.

Hiển thị chi tiết kế hoạch bằng LINQ:
  Tab 1 督導單位: PlanDepartment.ParticipationType = 1 -> PlanUser -> dm_User.
  Tab 2 陪集合:   PlanDepartment.ParticipationType = 2 -> PlanUser -> dm_User.
  Hai tab có cột Bộ phận | VNW | Tên | Chức vụ.
  Tab 3 檔案:     Report -> ReportFile -> dm_Attachment và dm_User (người tải).

Hiển thị danh sách file bằng LINQ:
  Tên file      = dm_Attachment.ActualName, nối qua ReportFile.IdAdt.
  Người tải     = ReportFile.CreateBy -> dm_User.Id -> tên hiển thị.
  Thời gian tải = ReportFile.CreateAt (dd/MM/yyyy HH:mm:ss).
Một báo cáo có nhiều dòng ReportFile; mỗi file có người tải/thời gian riêng.
Code lưu file vật lý và bản ghi dm_Attachment bằng cơ chế upload hiện có.

Chạy lại: bỏ qua bảng đã tồn tại, không sửa cấu trúc bảng cũ.
Do đó bản này dành cho trường hợp CHƯA chạy script tạo bảng 320 trước đó.
Phần cuối đăng ký menu 320 và sao chép quyền từ menu 316, không tạo trùng menu.
*/
USE [DBDocumentManagementSystem];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.dm_Function', N'U') IS NULL
       OR OBJECT_ID(N'dbo.dm_FunctionRole', N'U') IS NULL
        THROW 51000, 'Required menu tables are missing. Check the selected database.', 1;

    -- 1. Danh mục hạng mục kiểm tra và chu kỳ mặc định.
    IF OBJECT_ID(N'dbo.dt320_AuditItem', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_AuditItem
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_AuditItem PRIMARY KEY,
            DisplayName nvarchar(200) NOT NULL,
            CycleType char(1) NOT NULL,
            Description nvarchar(1000) NULL,
            IsActive bit NOT NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- 2. Kế hoạch: một hạng mục, thời gian kiểm tra và một kỳ thực hiện.
    IF OBJECT_ID(N'dbo.dt320_Plan', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_Plan
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_Plan PRIMARY KEY,
            IdAuditItem int NOT NULL,
            DisplayName nvarchar(200) NOT NULL,
            AuditAt datetime NOT NULL,
            CycleType char(1) NOT NULL,
            PeriodYear smallint NOT NULL,
            PeriodNo tinyint NOT NULL,
            Remark nvarchar(1000) NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- 3. Đơn vị tham gia: nhiều dòng cho mỗi kế hoạch và mỗi nhóm.
    IF OBJECT_ID(N'dbo.dt320_PlanDepartment', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_PlanDepartment
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_PlanDepartment PRIMARY KEY,
            IdPlan int NOT NULL,
            IdDept varchar(4) NOT NULL,
            ParticipationType tinyint NOT NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- 4. Nhân sự tham gia: IdPlanDepartment xác định kế hoạch, đơn vị và nhóm.
    -- IdUser tham chiếu dm_User.Id bằng code, không khai báo khóa ngoại.
    IF OBJECT_ID(N'dbo.dt320_PlanUser', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_PlanUser
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_PlanUser PRIMARY KEY,
            IdPlanDepartment int NOT NULL,
            IdUser varchar(10) NOT NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- 5. Báo cáo gắn với đơn vị tham gia trong kế hoạch.
    IF OBJECT_ID(N'dbo.dt320_Report', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_Report
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_Report PRIMARY KEY,
            IdPlanDepartment int NOT NULL,
            Remark nvarchar(2000) NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- 6. Một dòng cho mỗi file của báo cáo; IdAdt tham chiếu dm_Attachment.Id.
    IF OBJECT_ID(N'dbo.dt320_ReportFile', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt320_ReportFile
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt320_ReportFile PRIMARY KEY,
            IdReport int NOT NULL,
            IdAdt int NOT NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    -- Menu: sibling of 公正性查核; same role grants and visibility setting.
    DECLARE @ReferenceId int, @ParentId int, @Prioritize int, @Status bit;
    SELECT @ReferenceId = Id, @ParentId = IdParent,
           @Prioritize = ISNULL(Prioritize, 0) + 1, @Status = Status
    FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
    WHERE ControlName = 'uc316_ImpartialityAudit';

    IF @ReferenceId IS NULL
        THROW 51001, 'Cannot find the reference menu uc316_ImpartialityAudit.', 1;

    IF (SELECT COUNT(*) FROM dbo.dm_Function
        WHERE ControlName IN ('uc320_mainjihua','uc320_AuditManagementMain')) > 1
        THROW 51002, 'Multiple module 320 menu records found. Resolve duplicates first.', 1;

    DECLARE @FunctionId int;
    SELECT @FunctionId = Id FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
    WHERE ControlName IN ('uc320_mainjihua','uc320_AuditManagementMain');

    IF @FunctionId IS NULL
    BEGIN
        SELECT @FunctionId = ISNULL(MAX(Id),0) + 1
        FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);

        INSERT INTO dbo.dm_Function
            (Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES
            (@FunctionId, @ParentId, N'稽核管理系統', 'uc320_mainjihua',
             @Prioritize, @Status, 'icons_circled_5.svg');
    END;
    ELSE
    BEGIN
        UPDATE dbo.dm_Function
        SET IdParent = @ParentId, DisplayName = N'稽核管理系統',
            ControlName = 'uc320_mainjihua', Prioritize = @Prioritize,
            Images = 'icons_circled_5.svg'
        WHERE Id = @FunctionId;
    END;

    INSERT INTO dbo.dm_FunctionRole(IdFunction, IdRole)
    SELECT DISTINCT @FunctionId, source.IdRole
    FROM dbo.dm_FunctionRole source
    WHERE source.IdFunction = @ReferenceId
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.dm_FunctionRole existing WITH (UPDLOCK, HOLDLOCK)
          WHERE existing.IdFunction = @FunctionId AND existing.IdRole = source.IdRole
      );

    COMMIT TRANSACTION;
    PRINT N'Module 320 schema and menu are ready. No 316 data was changed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO

-- Kiểm tra cấu trúc sau khi chạy: 6 bảng, mỗi bảng chỉ có khóa chính.
SELECT t.name AS TableName, kc.name AS PrimaryKeyName
FROM sys.tables t
LEFT JOIN sys.key_constraints kc ON kc.parent_object_id = t.object_id AND kc.type = 'PK'
WHERE t.name LIKE 'dt320[_]%'
ORDER BY t.name;

SELECT Id, IdParent, DisplayName, ControlName, Prioritize, Images
FROM dbo.dm_Function WHERE ControlName = 'uc320_mainjihua';

/* Ví dụ quy ước (chỉ mô tả, KHÔNG chèn dữ liệu):
   Hạng mục A: CycleType = Q.
   Kế hoạch 1: IdAuditItem = A.Id, CycleType = Q, PeriodYear = 2026, PeriodNo = 4.
   Đơn vị: IdPlan = 1, IdDept = 7810, ParticipationType = 1 (督導單位).
   Đơn vị: IdPlan = 1, IdDept = 7820, ParticipationType = 1 (督導單位).
   Đơn vị: IdPlan = 1, IdDept = 7110, ParticipationType = 2 (陪集合).
   Đơn vị: IdPlan = 1, IdDept = 7310, ParticipationType = 2 (陪集合).
   Báo cáo: IdPlanDepartment = Id của dòng đơn vị tương ứng.
   Các file: cùng IdReport, mỗi dòng có IdAdt, CreateBy và CreateAt riêng.
*/
