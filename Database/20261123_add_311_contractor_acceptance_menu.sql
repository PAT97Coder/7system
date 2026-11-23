-- Chạy trên database ứng dụng. Điền Id của role cần cấp quyền rồi chạy lại cho từng role.
-- Role cần có quyền nhóm cha hiện tại của module 311.
DECLARE @RoleId int = NULL;
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;
    IF (SELECT COUNT(*) FROM dbo.dm_Function WHERE ControlName = 'uc311_ExpenseMain') <> 1
        THROW 51311, N'找不到唯一的模組 311 主畫面。', 1;
    IF @RoleId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.dm_Role WHERE Id = @RoleId)
        THROW 51311, N'指定的角色不存在。', 1;

    DECLARE @ParentId int = (SELECT IdParent FROM dbo.dm_Function WHERE ControlName = 'uc311_ExpenseMain');
    IF (SELECT COUNT(*) FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
        WHERE ControlName = 'uc311_ContractorAcceptance') > 1
        THROW 51311, N'外包工作驗收選單重複。', 1;

    DECLARE @FunctionId int = (SELECT Id FROM dbo.dm_Function WHERE ControlName = 'uc311_ContractorAcceptance');
    IF @FunctionId IS NULL
    BEGIN
        SELECT @FunctionId = ISNULL(MAX(Id), 0) + 1 FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);
        INSERT dbo.dm_Function (Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES (@FunctionId, @ParentId, N'外包工作驗收', 'uc311_ContractorAcceptance',
            (SELECT ISNULL(MAX(Prioritize), 0) + 1 FROM dbo.dm_Function WHERE IdParent = @ParentId), 1, 'icons_excel.svg');
    END;

    IF @RoleId IS NOT NULL AND NOT EXISTS
        (SELECT 1 FROM dbo.dm_FunctionRole WITH (UPDLOCK, HOLDLOCK) WHERE IdFunction = @FunctionId AND IdRole = @RoleId)
        INSERT dbo.dm_FunctionRole (IdFunction, IdRole) VALUES (@FunctionId, @RoleId);

    COMMIT TRANSACTION;
    SELECT @FunctionId AS FunctionId, @RoleId AS GrantedRoleId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
