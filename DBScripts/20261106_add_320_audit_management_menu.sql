SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ReferenceId int, @ParentId int, @Prioritize int, @Status bit;
    SELECT @ReferenceId = Id, @ParentId = IdParent,
           @Prioritize = ISNULL(Prioritize, 0) + 1, @Status = Status
    FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
    WHERE ControlName = 'uc316_ImpartialityAudit';

    IF @ReferenceId IS NULL
        THROW 51000, 'Cannot find the impartiality audit reference menu.', 1;

    DECLARE @FunctionId int;
    SELECT @FunctionId = Id
    FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK)
    WHERE ControlName IN ('uc320_AuditManagementMain', 'uc320_mainjihua');

    IF @FunctionId IS NULL
    BEGIN
        SELECT @FunctionId = ISNULL(MAX(Id), 0) + 1
        FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK);

        INSERT INTO dbo.dm_Function
            (Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
        VALUES
            (@FunctionId, @ParentId, N'稽核管理系統', 'uc320_mainjihua',
             @Prioritize, @Status, 'icons_circled_5.svg');
    END;

    -- Keep the existing menu ID and permissions when renaming the initial UC.
    UPDATE dbo.dm_Function
    SET ControlName = 'uc320_mainjihua'
    WHERE Id = @FunctionId AND ControlName = 'uc320_AuditManagementMain';

    INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
    SELECT DISTINCT @FunctionId, source.IdRole
    FROM dbo.dm_FunctionRole source
    WHERE source.IdFunction = @ReferenceId
      AND NOT EXISTS
      (
          SELECT 1 FROM dbo.dm_FunctionRole existing WITH (UPDLOCK, HOLDLOCK)
          WHERE existing.IdFunction = @FunctionId AND existing.IdRole = source.IdRole
      );

    COMMIT TRANSACTION;

    SELECT Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images
    FROM dbo.dm_Function WHERE Id = @FunctionId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
