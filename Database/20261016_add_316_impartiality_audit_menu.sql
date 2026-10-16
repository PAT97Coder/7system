-- Chạy sau 20261016_add_316_impartiality_audit.sql, trên cùng database ứng dụng.
-- @ParentId: NULL = giữ menu 316 hiện có hoặc tìm khu vực chứa module 315/314/307.
-- Nếu không suy ra được vị trí menu, điền Id nhóm cha thực tế rồi chạy lại.
-- @IdRole: điền dm_Role.Id cần cấp quyền; NULL = chỉ tạo menu, giữ quyền hiện có.
-- Chạy lại với từng role cần thiết. Không sao chép quyền từ module khác.
-- Script chỉ cấp quyền menu module 316 và màn hình; role cần có quyền nhóm cha
-- để điều hướng từ trang chính. Đăng xuất/đăng nhập lại sau khi cấp quyền.
DECLARE @ParentId int = NULL;
DECLARE @IdRole int = NULL;

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.dt316_Plan', N'U') IS NULL
        OR OBJECT_ID(N'dbo.dt316_PlanUser', N'U') IS NULL
        OR OBJECT_ID(N'dbo.dt316_Report', N'U') IS NULL
        THROW 51316, N'請先執行模組316資料表安裝指令碼。', 1;

    IF @IdRole IS NOT NULL AND NOT EXISTS(SELECT 1 FROM dbo.dm_Role WHERE Id = @IdRole)
        THROW 51316, N'指定的角色不存在。', 1;

    IF EXISTS
    (
        SELECT ControlName FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
        WHERE ControlName IN ('mnu316_ImpartialityAudit', 'uc316_ImpartialityAudit')
        GROUP BY ControlName HAVING COUNT(*) > 1
    )
        THROW 51316, N'模組316選單重複，請先檢查。', 1;

    DECLARE @ModuleId int =
        (SELECT Id FROM dbo.dm_Function WHERE ControlName = 'mnu316_ImpartialityAudit');
    DECLARE @ScreenId int =
        (SELECT Id FROM dbo.dm_Function WHERE ControlName = 'uc316_ImpartialityAudit');

    -- Tái sử dụng nhóm của màn hình 316 đã cài thủ công, tránh tạo menu trùng.
    IF @ModuleId IS NULL AND @ScreenId IS NOT NULL
        SELECT @ModuleId = p.Id FROM dbo.dm_Function s
        JOIN dbo.dm_Function p ON p.Id = s.IdParent
        WHERE s.Id = @ScreenId
            AND (p.ControlName IS NULL OR p.ControlName = '' OR p.ControlName = 'mnu316_ImpartialityAudit')
            AND NOT EXISTS(SELECT 1 FROM dbo.dm_Function x WHERE x.IdParent = p.Id AND x.Id <> @ScreenId);

    IF @ParentId IS NULL AND @ModuleId IS NOT NULL
        SELECT @ParentId = IdParent FROM dbo.dm_Function WHERE Id = @ModuleId;
    IF @ParentId IS NULL AND @ScreenId IS NOT NULL
        SELECT @ParentId = IdParent FROM dbo.dm_Function WHERE Id = @ScreenId;
    IF @ParentId IS NULL
        SELECT TOP (1) @ParentId = m.IdParent
        FROM dbo.dm_Function s
        JOIN dbo.dm_Function m ON m.Id = s.IdParent
        WHERE s.ControlName IN ('uc315_Interview', 'uc314_HskExamMgmt', 'uc307_ExamMgmt')
        ORDER BY CASE s.ControlName WHEN 'uc315_Interview' THEN 1 WHEN 'uc314_HskExamMgmt' THEN 2 ELSE 3 END, s.Id;
    IF @ParentId IS NULL
        SELECT TOP (1) @ParentId = IdParent FROM dbo.dm_Function
        WHERE ControlName = 'uc307_QuizMain' ORDER BY Id;

    IF @ParentId IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.dm_Function WHERE Id = @ParentId)
        THROW 51316, N'無法判斷模組316的上層選單，請設定ParentId。', 1;
    IF @ParentId = @ModuleId OR @ParentId = @ScreenId
        THROW 51316, N'上層選單不可為模組316本身。', 1;

    IF @ModuleId IS NULL
    BEGIN
        SELECT @ModuleId = ISNULL(MAX(Id), 0) + 1 FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);
        INSERT dbo.dm_Function(Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES(@ModuleId, @ParentId, N'公正性查核', 'mnu316_ImpartialityAudit',
            (SELECT ISNULL(MAX(Prioritize), 0) + 1 FROM dbo.dm_Function WHERE IdParent = @ParentId), 1, NULL);
    END
    ELSE
        UPDATE dbo.dm_Function SET IdParent = @ParentId, DisplayName = N'公正性查核',
            ControlName = 'mnu316_ImpartialityAudit', Status = 1 WHERE Id = @ModuleId;

    IF @ScreenId IS NULL
    BEGIN
        SELECT @ScreenId = ISNULL(MAX(Id), 0) + 1 FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);
        INSERT dbo.dm_Function(Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES(@ScreenId, @ModuleId, N'主頁', 'uc316_ImpartialityAudit', 1, 1, NULL);
    END
    ELSE
        UPDATE dbo.dm_Function SET IdParent = @ModuleId, DisplayName = N'主頁', Status = 1
        WHERE Id = @ScreenId;

    -- Giữ quyền mở màn hình 316 hiện có khi bổ sung nhóm menu phía trên.
    INSERT dbo.dm_FunctionRole(IdFunction, IdRole)
        SELECT DISTINCT @ModuleId, r.IdRole FROM dbo.dm_FunctionRole r
        WHERE r.IdFunction = @ScreenId
            AND NOT EXISTS(SELECT 1 FROM dbo.dm_FunctionRole x
                WHERE x.IdFunction = @ModuleId AND x.IdRole = r.IdRole);

    IF @IdRole IS NOT NULL
        INSERT dbo.dm_FunctionRole(IdFunction, IdRole)
            SELECT f.Id, @IdRole FROM dbo.dm_Function f
            WHERE f.Id IN (@ModuleId, @ScreenId)
                AND NOT EXISTS(SELECT 1 FROM dbo.dm_FunctionRole r
                    WHERE r.IdFunction = f.Id AND r.IdRole = @IdRole);

    COMMIT TRANSACTION;
    SELECT @ParentId AS ParentId, @ModuleId AS ModuleId, @ScreenId AS ScreenId, @IdRole AS GrantedRoleId;
    IF @IdRole IS NULL
        PRINT N'選單已就緒；未指定新角色，請確認現有角色權限。';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
