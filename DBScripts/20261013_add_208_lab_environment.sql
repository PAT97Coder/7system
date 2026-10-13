-- Module 208 trong khu vực ISO. Chạy bằng tài khoản triển khai trên DB đã chọn.
-- Thời gian DATETIME2 trong module là giờ Việt Nam (UTC+07:00).
SET XACT_ABORT ON;
GO
BEGIN TRAN;
IF OBJECT_ID('dbo.dt208_Devices') IS NULL
BEGIN
 CREATE TABLE dbo.dt208_Devices (
  Id INT IDENTITY PRIMARY KEY, SN NVARCHAR(100) NOT NULL UNIQUE,
  DisplayName NVARCHAR(200) NOT NULL, Model NVARCHAR(100) NULL,
  CreatedAt DATETIME2(3) NOT NULL DEFAULT DATEADD(HOUR,7,SYSUTCDATETIME()));
 CREATE TABLE dbo.dt208_DeviceConfigs (
  Id BIGINT IDENTITY PRIMARY KEY, IdDevice INT NOT NULL REFERENCES dbo.dt208_Devices(Id),
  DisplayName NVARCHAR(200) NOT NULL, Model NVARCHAR(100) NULL,
  IdDept NVARCHAR(50) NOT NULL, Location NVARCHAR(200) NOT NULL,
  IdManager NVARCHAR(50) NOT NULL, TempMin DECIMAL(8,2) NOT NULL, TempMax DECIMAL(8,2) NOT NULL,
  HumidityMin DECIMAL(5,2) NOT NULL, HumidityMax DECIMAL(5,2) NOT NULL,
  ExpectedMinutes INT NOT NULL, DeviceState INT NOT NULL,
  EffectiveFrom DATETIME2(3) NOT NULL, ChangedBy NVARCHAR(50) NOT NULL,
  Reason NVARCHAR(500) NOT NULL,
  CONSTRAINT CK_dt208_DeviceConfigs_Ranges CHECK (TempMin<TempMax AND HumidityMin>=0 AND HumidityMax<=100
   AND HumidityMin<HumidityMax AND ExpectedMinutes BETWEEN 1 AND 10080 AND DeviceState BETWEEN 0 AND 3),
  CONSTRAINT UQ_dt208_DeviceConfigs_Effective UNIQUE(IdDevice,EffectiveFrom));
 CREATE TABLE dbo.dt208_Readings (
  Id BIGINT IDENTITY PRIMARY KEY, SN NVARCHAR(100) NOT NULL,
  MeasuredAt DATETIME2(3) NOT NULL, Temperature DECIMAL(8,2) NOT NULL,
  Humidity DECIMAL(5,2) NOT NULL CHECK(Humidity BETWEEN 0 AND 100),
  ReceivedAt DATETIME2(3) NOT NULL DEFAULT DATEADD(HOUR,7,SYSUTCDATETIME()),
  CONSTRAINT UQ_dt208_Readings UNIQUE(SN,MeasuredAt));
 CREATE TABLE dbo.dt208_ReadingConflicts (
  Id BIGINT IDENTITY PRIMARY KEY, ReadingId BIGINT NOT NULL REFERENCES dbo.dt208_Readings(Id),
  Temperature DECIMAL(8,2) NOT NULL, Humidity DECIMAL(5,2) NOT NULL,
  ReceivedAt DATETIME2(3) NOT NULL DEFAULT DATEADD(HOUR,7,SYSUTCDATETIME()));
 CREATE TABLE dbo.dt208_ReadingActions (
  Id BIGINT IDENTITY PRIMARY KEY, ReadingId BIGINT NOT NULL REFERENCES dbo.dt208_Readings(Id),
  Cause NVARCHAR(1000) NOT NULL, Action NVARCHAR(1000) NOT NULL, Result NVARCHAR(1000) NOT NULL,
  IsClosed BIT NOT NULL, CreatedBy NVARCHAR(50) NOT NULL,
  CreatedAt DATETIME2(3) NOT NULL DEFAULT DATEADD(HOUR,7,SYSUTCDATETIME()));
 CREATE INDEX IX_dt208_DeviceConfigs_Dept ON dbo.dt208_DeviceConfigs(IdDept,IdDevice,EffectiveFrom DESC);
END;
COMMIT;
GO
-- Nhóm: 溫濕度組, 溫濕度【編輯】, 溫濕度【處理】; mỗi nhóm gắn một IdDept.
-- EDIT/HANDLE bao gồm quyền xem. Không tự cấp quyền thay đổi từ nhóm ISO.
IF OBJECT_ID('dbo.fn208_EnvironmentPermission','FN') IS NULL
 EXEC(N'CREATE FUNCTION dbo.fn208_EnvironmentPermission(@UserId NVARCHAR(50),@IdDept NVARCHAR(50),@Permission VARCHAR(10)) RETURNS BIT AS BEGIN RETURN 0 END');
GO
ALTER FUNCTION dbo.fn208_EnvironmentPermission(@UserId NVARCHAR(50), @IdDept NVARCHAR(50), @Permission VARCHAR(10))
RETURNS BIT AS
BEGIN
 DECLARE @Allowed BIT=0;
 IF EXISTS(SELECT 1 FROM dbo.dm_Group g JOIN dbo.dm_GroupUser u ON u.IdGroup=g.Id
  WHERE u.IdUser=@UserId AND g.DisplayName=N'溫濕度【管理】' AND g.IdDept='*') RETURN 1;
 IF EXISTS(SELECT 1 FROM dbo.dm_Group g JOIN dbo.dm_GroupUser u ON u.IdGroup=g.Id
  WHERE u.IdUser=@UserId AND g.IdDept=@IdDept AND
   (g.DisplayName=CASE @Permission WHEN 'VIEW' THEN N'溫濕度組' WHEN 'EDIT' THEN N'溫濕度【編輯】' WHEN 'HANDLE' THEN N'溫濕度【處理】' END
    OR (@Permission='VIEW' AND g.DisplayName IN(N'溫濕度【編輯】',N'溫濕度【處理】'))))
  SET @Allowed=1;
 RETURN @Allowed;
END;
GO
IF OBJECT_ID('dbo.usp208_IngestReading','P') IS NULL EXEC(N'CREATE PROCEDURE dbo.usp208_IngestReading AS RETURN');
GO
ALTER PROCEDURE dbo.usp208_IngestReading
 @SN NVARCHAR(4000), @MeasuredAt DATETIME2(3), @Temperature DECIMAL(8,2), @Humidity DECIMAL(5,2)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @SN=UPPER(LTRIM(RTRIM(@SN)));
 IF NULLIF(@SN,'') IS NULL OR LEN(@SN)>100 OR @MeasuredAt IS NULL OR @Temperature IS NULL OR @Humidity IS NULL
  OR @Humidity NOT BETWEEN 0 AND 100 OR @MeasuredAt>DATEADD(MINUTE,5,DATEADD(HOUR,7,SYSUTCDATETIME()))
  THROW 51000,N'Số đo không hợp lệ: kiểm tra SN, thời gian và độ ẩm 0–100%.',1;
 BEGIN TRY
  BEGIN TRAN;
  DECLARE @Id BIGINT, @OldT DECIMAL(8,2), @OldH DECIMAL(5,2);
  SELECT @Id=Id,@OldT=Temperature,@OldH=Humidity FROM dbo.dt208_Readings WITH(UPDLOCK,HOLDLOCK)
   WHERE SN=@SN AND MeasuredAt=@MeasuredAt;
  IF @Id IS NULL
  BEGIN
   INSERT dbo.dt208_Readings(SN,MeasuredAt,Temperature,Humidity) VALUES(@SN,@MeasuredAt,@Temperature,@Humidity);
   SET @Id=SCOPE_IDENTITY();
   COMMIT; SELECT @Id AS ReadingId,'Accepted' AS Result; RETURN;
  END;
  IF @OldT=@Temperature AND @OldH=@Humidity
  BEGIN
   COMMIT; SELECT @Id AS ReadingId,'Duplicate' AS Result; RETURN;
  END;
  IF NOT EXISTS(SELECT 1 FROM dbo.dt208_ReadingConflicts WHERE ReadingId=@Id AND Temperature=@Temperature AND Humidity=@Humidity)
   INSERT dbo.dt208_ReadingConflicts(ReadingId,Temperature,Humidity) VALUES(@Id,@Temperature,@Humidity);
  COMMIT; SELECT @Id AS ReadingId,'Conflict' AS Result;
 END TRY
 BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK; THROW;
 END CATCH;
END;
GO
IF OBJECT_ID('dbo.usp208_SaveDevice','P') IS NULL EXEC(N'CREATE PROCEDURE dbo.usp208_SaveDevice AS RETURN');
GO
ALTER PROCEDURE dbo.usp208_SaveDevice
 @UserId NVARCHAR(50), @Id INT, @ExpectedConfigId BIGINT,
 @SN NVARCHAR(100), @DisplayName NVARCHAR(200), @Model NVARCHAR(100),
 @IdDept NVARCHAR(50), @Location NVARCHAR(200), @IdManager NVARCHAR(50),
 @TempMin DECIMAL(8,2), @TempMax DECIMAL(8,2), @HumidityMin DECIMAL(5,2), @HumidityMax DECIMAL(5,2),
 @ExpectedMinutes INT, @DeviceState INT, @Reason NVARCHAR(500)
AS
BEGIN
 SET NOCOUNT ON; SET XACT_ABORT ON;
 SET @SN=UPPER(LTRIM(RTRIM(@SN))); SET @DisplayName=LTRIM(RTRIM(@DisplayName));
 IF dbo.fn208_EnvironmentPermission(@UserId,@IdDept,'EDIT')=0 THROW 51001,N'Không có quyền quản lý bộ phận này.',1;
 IF NULLIF(@SN,'') IS NULL OR NULLIF(@DisplayName,'') IS NULL OR NULLIF(LTRIM(RTRIM(@Location)),'') IS NULL
  OR NULLIF(LTRIM(RTRIM(@Reason)),'') IS NULL
  OR NOT EXISTS(SELECT 1 FROM dbo.dm_Departments WHERE Id=@IdDept AND IsActive=1)
  OR NOT EXISTS(SELECT 1 FROM dbo.dm_User WHERE Id=@IdManager)
  OR @TempMin>=@TempMax OR @HumidityMin<0 OR @HumidityMax>100 OR @HumidityMin>=@HumidityMax
  OR @ExpectedMinutes NOT BETWEEN 1 AND 10080 OR @DeviceState NOT BETWEEN 0 AND 3
  THROW 51002,N'Thông tin thiết bị hoặc giới hạn không hợp lệ.',1;
 BEGIN TRY
  BEGIN TRAN;
  DECLARE @Now DATETIME2(3)=DATEADD(HOUR,7,SYSUTCDATETIME()),@OldConfig BIGINT,@OldDept NVARCHAR(50),@Last DATETIME2(3);
  IF @Id>0
  BEGIN
   IF NOT EXISTS(SELECT 1 FROM dbo.dt208_Devices WITH(UPDLOCK,HOLDLOCK) WHERE Id=@Id AND SN=@SN)
    THROW 51003,N'Thiết bị không tồn tại hoặc SN đã bị thay đổi.',1;
   SELECT TOP 1 @OldConfig=Id,@OldDept=IdDept,@Last=EffectiveFrom FROM dbo.dt208_DeviceConfigs
    WHERE IdDevice=@Id ORDER BY EffectiveFrom DESC;
   IF @OldConfig<>@ExpectedConfigId OR @OldConfig IS NULL
    THROW 51004,N'Thiết bị đã được sửa bởi người khác. Hãy tải lại.',1;
   IF dbo.fn208_EnvironmentPermission(@UserId,@OldDept,'EDIT')=0 THROW 51001,N'Không có quyền quản lý bộ phận cũ.',1;
   IF @Now<=@Last SET @Now=DATEADD(MILLISECOND,1,@Last);
   UPDATE dbo.dt208_Devices SET DisplayName=@DisplayName,Model=@Model WHERE Id=@Id;
  END
  ELSE
  BEGIN
   INSERT dbo.dt208_Devices(SN,DisplayName,Model) VALUES(@SN,@DisplayName,@Model);
   SET @Id=SCOPE_IDENTITY();
  END;
  INSERT dbo.dt208_DeviceConfigs(IdDevice,DisplayName,Model,IdDept,Location,IdManager,TempMin,TempMax,HumidityMin,HumidityMax,
   ExpectedMinutes,DeviceState,EffectiveFrom,ChangedBy,Reason)
   VALUES(@Id,@DisplayName,@Model,@IdDept,@Location,@IdManager,@TempMin,@TempMax,@HumidityMin,@HumidityMax,
    @ExpectedMinutes,@DeviceState,@Now,@UserId,@Reason);
  COMMIT; SELECT @Id AS Id;
 END TRY
 BEGIN CATCH
  IF @@TRANCOUNT>0 ROLLBACK; THROW;
 END CATCH;
END;
GO
IF OBJECT_ID('dbo.usp208_AddReadingAction','P') IS NULL EXEC(N'CREATE PROCEDURE dbo.usp208_AddReadingAction AS RETURN');
GO
ALTER PROCEDURE dbo.usp208_AddReadingAction
 @UserId NVARCHAR(50),@ReadingId BIGINT,@Cause NVARCHAR(1000),@Action NVARCHAR(1000),@Result NVARCHAR(1000),@IsClosed BIT
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @Dept NVARCHAR(50),@Abnormal BIT;
 SELECT @Dept=c.IdDept,@Abnormal=CASE WHEN r.Temperature<c.TempMin OR r.Temperature>c.TempMax
  OR r.Humidity<c.HumidityMin OR r.Humidity>c.HumidityMax THEN 1 ELSE 0 END
 FROM dbo.dt208_Readings r JOIN dbo.dt208_Devices d ON d.SN=r.SN
 CROSS APPLY(SELECT TOP 1 * FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id AND EffectiveFrom<=r.MeasuredAt ORDER BY EffectiveFrom DESC)c
 WHERE r.Id=@ReadingId;
 IF @Dept IS NULL OR dbo.fn208_EnvironmentPermission(@UserId,@Dept,'HANDLE')=0 THROW 51001,N'Không có quyền xử lý số đo này.',1;
 IF @Abnormal=0 OR NULLIF(LTRIM(RTRIM(@Cause)),'') IS NULL OR NULLIF(LTRIM(RTRIM(@Action)),'') IS NULL
  OR (@IsClosed=1 AND NULLIF(LTRIM(RTRIM(@Result)),'') IS NULL)
  THROW 51005,N'Cần số đo vượt giới hạn, nguyên nhân, biện pháp và kết quả khi đóng xử lý.',1;
 INSERT dbo.dt208_ReadingActions(ReadingId,Cause,Action,Result,IsClosed,CreatedBy)
  VALUES(@ReadingId,@Cause,@Action,@Result,@IsClosed,@UserId);
END;
GO
IF DATABASE_PRINCIPAL_ID('lab_environment_ingest') IS NULL CREATE ROLE lab_environment_ingest;
GRANT EXECUTE ON dbo.usp208_IngestReading TO lab_environment_ingest;
GO
-- Thêm nhóm module 208 trong khu vực ISO, không tạo tile trên trang chính.
SET XACT_ABORT ON;
BEGIN TRAN;
DECLARE @IsoId INT=TRY_CONVERT(INT,(SELECT TOP 1 ValueT FROM dbo.sys_StaticValue WHERE KeyT='RoleISOAuditDocsMain'));
IF @IsoId IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.dm_Function WHERE Id=@IsoId)
 THROW 51006,N'Không tìm thấy chức năng ISO từ RoleISOAuditDocsMain.',1;
DECLARE @Parent INT=@IsoId,@Module INT;
SELECT @Module=Id FROM dbo.dm_Function WHERE ControlName='mnu208_LabEnvironment';
IF @Module IS NULL
BEGIN
 SELECT @Module=ISNULL(MAX(Id),0)+1 FROM dbo.dm_Function WITH(UPDLOCK,HOLDLOCK);
 INSERT dbo.dm_Function(Id,IdParent,DisplayName,ControlName,Prioritize,Status,Images)
  VALUES(@Module,@Parent,N'溫濕度管理','mnu208_LabEnvironment',
   (SELECT ISNULL(MAX(Prioritize),0)+1 FROM dbo.dm_Function WHERE IdParent=@Parent),1,NULL);
END;
DECLARE @Screen INT=(SELECT TOP 1 Id FROM dbo.dm_Function WHERE ControlName='uc208_LabEnvironment');
IF @Screen IS NULL
BEGIN
 SELECT @Screen=ISNULL(MAX(Id),0)+1 FROM dbo.dm_Function WITH(UPDLOCK,HOLDLOCK);
 INSERT dbo.dm_Function(Id,IdParent,DisplayName,ControlName,Prioritize,Status,Images)
 VALUES(@Screen,@Module,N'溫濕度監控','uc208_LabEnvironment',1,1,NULL);
END;
INSERT dbo.dm_FunctionRole(IdFunction,IdRole)
 SELECT DISTINCT f.Id,r.IdRole FROM dbo.dm_FunctionRole r CROSS JOIN(SELECT @Module Id UNION ALL SELECT @Screen)f WHERE r.IdFunction=@IsoId
 AND NOT EXISTS(SELECT 1 FROM dbo.dm_FunctionRole x WHERE x.IdFunction=f.Id AND x.IdRole=r.IdRole);
COMMIT;
GO
-- Nhóm theo bộ phận tương tự ISO. Không tự sao chép thành viên hoặc cấp quyền cá nhân.
SET XACT_ABORT ON;
BEGIN TRAN;
INSERT dbo.dm_Group(DisplayName,Describe,Prioritize,IdDept)
 SELECT p.DisplayName,N'Module 208 - quản lý nhiệt độ và độ ẩm',0,depts.IdDept
 FROM(SELECT DISTINCT g.IdDept FROM dbo.dm_Group g JOIN dbo.dm_Departments d ON d.Id=g.IdDept
  WHERE g.DisplayName=N'ISO組' AND d.IsActive=1)depts
 CROSS JOIN(SELECT N'溫濕度組' DisplayName UNION ALL SELECT N'溫濕度【編輯】' UNION ALL SELECT N'溫濕度【處理】')p
 WHERE NOT EXISTS(SELECT 1 FROM dbo.dm_Group g WITH(UPDLOCK,HOLDLOCK) WHERE g.DisplayName=p.DisplayName AND g.IdDept=depts.IdDept);
IF NOT EXISTS(SELECT 1 FROM dbo.dm_Group WITH(UPDLOCK,HOLDLOCK) WHERE DisplayName=N'溫濕度【管理】' AND IdDept='*')
 INSERT dbo.dm_Group(DisplayName,Describe,Prioritize,IdDept) VALUES(N'溫濕度【管理】',N'Module 208 - quản trị tất cả bộ phận',0,'*');
COMMIT;
GO
