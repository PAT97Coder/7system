SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @OldFunctionId int =
    (
        SELECT TOP 1 Id FROM dbo.dm_Function WHERE ControlName = 'uc307_Interview'
    );
    DECLARE @FunctionId int =
    (
        SELECT TOP 1 Id FROM dbo.dm_Function WHERE ControlName = 'uc315_Interview'
    );

    IF @FunctionId IS NULL AND @OldFunctionId IS NOT NULL
    BEGIN
        UPDATE dbo.dm_Function
        SET ControlName = 'uc315_Interview', DisplayName = N'主頁', Status = 1
        WHERE Id = @OldFunctionId;
        SET @FunctionId = @OldFunctionId;
    END;

    IF @FunctionId IS NULL
    BEGIN
        DECLARE @ParentId int =
        (
            SELECT TOP 1 IdParent FROM dbo.dm_Function WHERE ControlName = 'uc307_QuizMain'
        );
        DECLARE @ModuleId int = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;

        INSERT INTO dbo.dm_Function(Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES
        (
            @ModuleId,
            @ParentId,
            N'口試評核',
            NULL,
            ISNULL((SELECT MAX(ISNULL(Prioritize, 0)) FROM dbo.dm_Function WHERE IdParent = @ParentId), 0) + 1,
            1,
            NULL
        );

        SET @FunctionId = @ModuleId + 1;
        INSERT INTO dbo.dm_Function(Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES(@FunctionId, @ModuleId, N'主頁', 'uc315_Interview', 1, 1, NULL);
    END;

    DECLARE @CurrentParentId int = (SELECT IdParent FROM dbo.dm_Function WHERE Id = @FunctionId);
    UPDATE dbo.dm_Function SET DisplayName = N'口試評核', Status = 1 WHERE Id = @CurrentParentId;
    UPDATE dbo.dm_Function SET DisplayName = N'主頁', ControlName = 'uc315_Interview', Status = 1 WHERE Id = @FunctionId;

    IF NOT EXISTS (SELECT 1 FROM dbo.sys_StaticValue WHERE KeyT = '315WebLink')
        INSERT INTO dbo.sys_StaticValue(KeyT, ValueT) VALUES('315WebLink', '');

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
