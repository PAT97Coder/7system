-- Cấp quyền module 208 cho một người dùng / bộ phận. Sửa 3 giá trị dưới trước khi chạy.
-- VIEW: xem; EDIT: quản lý thiết bị/ngưỡng + xem; HANDLE: xử lý vượt giới hạn + xem.
-- ADMIN: quản trị tất cả bộ phận và xem SN chưa đăng ký; dùng IdDept='*'.
DECLARE @IdUser NVARCHAR(50)=NULL;
DECLARE @IdDept NVARCHAR(50)=NULL;
DECLARE @Permission VARCHAR(10)='VIEW';
SET XACT_ABORT ON;
IF @IdUser IS NULL OR @IdDept IS NULL THROW 51010,N'Điền IdUser, IdDept và Permission trước khi cấp quyền.',1;
IF @Permission NOT IN('VIEW','EDIT','HANDLE','ADMIN') THROW 51010,N'Quyền không hợp lệ.',1;
IF NOT EXISTS(SELECT 1 FROM dbo.dm_User WHERE Id=@IdUser) THROW 51010,N'Người dùng không tồn tại.',1;
IF (@Permission='ADMIN' AND @IdDept<>'*') OR (@Permission<>'ADMIN' AND NOT EXISTS(SELECT 1 FROM dbo.dm_Departments WHERE Id=@IdDept AND IsActive=1))
 THROW 51010,N'Bộ phận không hợp lệ.',1;
BEGIN TRAN;
DECLARE @Group INT;
DECLARE @GroupName NVARCHAR(100)=CASE @Permission WHEN 'VIEW' THEN N'溫濕度組' WHEN 'EDIT' THEN N'溫濕度【編輯】' WHEN 'HANDLE' THEN N'溫濕度【處理】' WHEN 'ADMIN' THEN N'溫濕度【管理】' END;
SELECT @Group=Id FROM dbo.dm_Group WITH(UPDLOCK,HOLDLOCK) WHERE DisplayName=@GroupName AND IdDept=@IdDept;
IF @Group IS NULL
BEGIN
 INSERT dbo.dm_Group(DisplayName,Describe,Prioritize,IdDept)
 VALUES(@GroupName,N'Quyền quản lý nhiệt độ và độ ẩm - module 208',0,@IdDept);
 SET @Group=SCOPE_IDENTITY();
END;
IF NOT EXISTS(SELECT 1 FROM dbo.dm_GroupUser WITH(UPDLOCK,HOLDLOCK) WHERE IdGroup=@Group AND IdUser=@IdUser)
 INSERT dbo.dm_GroupUser(IdGroup,IdUser) VALUES(@Group,@IdUser);
DECLARE @Function INT=(SELECT TOP 1 Id FROM dbo.dm_Function WHERE ControlName='uc208_LabEnvironment');
IF @Function IS NULL THROW 51010,N'Cần cài module 208 trước.',1;
-- Quyền mở menu dùng các role hiện có của người dùng, giống hệ thống hiện tại.
INSERT dbo.dm_FunctionRole(IdFunction,IdRole)
 SELECT DISTINCT f.Id,r.IdRole FROM dbo.dm_UserRole r
 JOIN dbo.dm_Function f ON f.Id=@Function OR f.ControlName='mnu208_LabEnvironment'
 WHERE r.IdUser=@IdUser
 AND NOT EXISTS(SELECT 1 FROM dbo.dm_FunctionRole x WHERE x.IdFunction=f.Id AND x.IdRole=r.IdRole);
COMMIT;
-- Đăng xuất/đăng nhập lại để tải quyền menu.
