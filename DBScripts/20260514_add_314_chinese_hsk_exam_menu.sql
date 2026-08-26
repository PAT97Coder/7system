DECLARE @ParentId INT;

SELECT TOP 1
    @ParentId = IdParent
FROM dbo.dm_Function
WHERE ControlName = 'uc307_QuizMain';

IF @ParentId IS NOT NULL
BEGIN
    DECLARE @ModuleId INT =
    (
        SELECT TOP 1 Id
        FROM dbo.dm_Function
        WHERE ControlName = 'mnu314_ChineseExam'
           OR ControlName = 'uc314_ChineseExamMain'
    );

    IF @ModuleId IS NULL
    BEGIN
        DECLARE @NewId INT = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;
        DECLARE @Prioritize INT =
            ISNULL((SELECT MAX(ISNULL(Prioritize, 0)) FROM dbo.dm_Function WHERE IdParent = @ParentId), 0) + 1;

        INSERT INTO dbo.dm_Function
        (
            Id,
            IdParent,
            DisplayName,
            ControlName,
            Prioritize,
            Status,
            Images
        )
        VALUES
        (
            @NewId,
            @ParentId,
            N'漢語水平考試',
            'mnu314_ChineseExam',
            @Prioritize,
            1,
            NULL
        );

        SET @ModuleId = @NewId;
    END
    ELSE
    BEGIN
        UPDATE dbo.dm_Function
        SET DisplayName = N'漢語水平考試',
            ControlName = 'mnu314_ChineseExam',
            IdParent = @ParentId,
            Status = 1
        WHERE Id = @ModuleId;
    END;

    DECLARE @HskFunctions TABLE
    (
        DisplayName NVARCHAR(100) NOT NULL,
        ControlName VARCHAR(100) NOT NULL,
        Prioritize INT NOT NULL
    );

    INSERT INTO @HskFunctions (DisplayName, ControlName, Prioritize)
    VALUES
        (N'題庫', 'uc314_HskQuestionBank', 1),
        (N'考試管理', 'uc314_HskExamMgmt', 2);

    UPDATE dbo.dm_Function
    SET Status = 0
    WHERE ControlName = 'uc314_HskMyExam';

    DECLARE @DisplayName NVARCHAR(100);
    DECLARE @ControlName VARCHAR(100);
    DECLARE @ChildPrioritize INT;

    DECLARE HskFunctionCursor CURSOR LOCAL FAST_FORWARD FOR
        SELECT DisplayName, ControlName, Prioritize
        FROM @HskFunctions
        ORDER BY Prioritize;

    OPEN HskFunctionCursor;
    FETCH NEXT FROM HskFunctionCursor INTO @DisplayName, @ControlName, @ChildPrioritize;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM dbo.dm_Function WHERE ControlName = @ControlName)
        BEGIN
            INSERT INTO dbo.dm_Function
            (
                Id,
                IdParent,
                DisplayName,
                ControlName,
                Prioritize,
                Status,
                Images
            )
            VALUES
            (
                ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1,
                @ModuleId,
                @DisplayName,
                @ControlName,
                @ChildPrioritize,
                1,
                NULL
            );
        END
        ELSE
        BEGIN
            UPDATE dbo.dm_Function
            SET IdParent = @ModuleId,
                DisplayName = @DisplayName,
                Prioritize = @ChildPrioritize,
                Status = 1
            WHERE ControlName = @ControlName;
        END;

        FETCH NEXT FROM HskFunctionCursor INTO @DisplayName, @ControlName, @ChildPrioritize;
    END;

    CLOSE HskFunctionCursor;
    DEALLOCATE HskFunctionCursor;

    DECLARE @FunctionRoles TABLE (IdRole INT NOT NULL PRIMARY KEY);

    INSERT INTO @FunctionRoles (IdRole)
    SELECT DISTINCT SourceRole.IdRole
    FROM
    (
        SELECT DISTINCT fr.IdRole
        FROM dbo.dm_FunctionRole fr
        INNER JOIN dbo.dm_Function f ON f.Id = fr.IdFunction
        WHERE f.ControlName = 'uc307_QuizMain'
           OR f.IdParent = @ParentId
    ) AS SourceRole
    WHERE NOT EXISTS (SELECT 1 FROM @FunctionRoles existed WHERE existed.IdRole = SourceRole.IdRole);

    INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
    SELECT funcs.Id, roles.IdRole
    FROM dbo.dm_Function funcs
    CROSS JOIN @FunctionRoles roles
    WHERE (funcs.Id = @ModuleId OR funcs.IdParent = @ModuleId)
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.dm_FunctionRole existed
          WHERE existed.IdFunction = funcs.Id
            AND existed.IdRole = roles.IdRole
      );
END;
