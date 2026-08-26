SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @ReferenceModuleId int;
    DECLARE @ParentId int;

    SELECT TOP 1 @ReferenceModuleId = IdParent
    FROM dbo.dm_Function
    WHERE ControlName IN ('uc315_Interview', 'uc314_HskExamMgmt', 'uc307_ExamMgmt')
    ORDER BY CASE ControlName
        WHEN 'uc315_Interview' THEN 1
        WHEN 'uc314_HskExamMgmt' THEN 2
        ELSE 3
    END;

    SELECT @ParentId = IdParent
    FROM dbo.dm_Function
    WHERE Id = @ReferenceModuleId;

    IF @ParentId IS NULL
    BEGIN
        SELECT TOP 1 @ParentId = IdParent
        FROM dbo.dm_Function
        WHERE ControlName = 'uc307_QuizMain';
    END;

    IF @ParentId IS NULL
        THROW 51000, 'Cannot determine the parent menu for exam statistics.', 1;

    DECLARE @ModuleId int =
    (
        SELECT TOP 1 Id
        FROM dbo.dm_Function
        WHERE ControlName = 'mnu317_ExamStatistics'
    );

    IF @ModuleId IS NULL
    BEGIN
        SET @ModuleId = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;

        INSERT INTO dbo.dm_Function
        (
            Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images
        )
        VALUES
        (
            @ModuleId,
            @ParentId,
            N'年度綜合成績',
            'mnu317_ExamStatistics',
            ISNULL((SELECT MAX(ISNULL(Prioritize, 0)) FROM dbo.dm_Function WHERE IdParent = @ParentId), 0) + 1,
            1,
            'Statistics.svg'
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.dm_Function
        SET IdParent = @ParentId,
            DisplayName = N'年度綜合成績',
            ControlName = 'mnu317_ExamStatistics',
            Status = 1,
            Images = 'Statistics.svg'
        WHERE Id = @ModuleId;
    END;

    DECLARE @FunctionId int =
    (
        SELECT TOP 1 Id
        FROM dbo.dm_Function
        WHERE ControlName = 'uc317_ExamStatistics'
    );

    IF @FunctionId IS NULL
    BEGIN
        SET @FunctionId = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;

        INSERT INTO dbo.dm_Function
        (
            Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images
        )
        VALUES
        (
            @FunctionId, @ModuleId, N'統計', 'uc317_ExamStatistics', 1, 1, 'Statistics.svg'
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.dm_Function
        SET IdParent = @ModuleId,
            DisplayName = N'統計',
            Prioritize = 1,
            Status = 1,
            Images = 'Statistics.svg'
        WHERE Id = @FunctionId;
    END;

    DECLARE @FunctionRoles TABLE (IdRole int NOT NULL PRIMARY KEY);

    INSERT INTO @FunctionRoles (IdRole)
    SELECT DISTINCT roleSource.IdRole
    FROM
    (
        SELECT fr.IdRole
        FROM dbo.dm_FunctionRole fr
        INNER JOIN dbo.dm_Function f ON f.Id = fr.IdFunction
        WHERE f.ControlName IN ('uc307_ExamMgmt', 'uc314_HskExamMgmt', 'uc315_Interview')

        UNION

        SELECT fr.IdRole
        FROM dbo.dm_FunctionRole fr
        WHERE fr.IdFunction = @ReferenceModuleId
    ) roleSource;

    INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
    SELECT target.IdFunction, roles.IdRole
    FROM (VALUES (@ModuleId), (@FunctionId)) target(IdFunction)
    CROSS JOIN @FunctionRoles roles
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.dm_FunctionRole existing
        WHERE existing.IdFunction = target.IdFunction
          AND existing.IdRole = roles.IdRole
    );

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
