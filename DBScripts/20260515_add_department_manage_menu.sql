/* ---------------------------------------------------------------------------
   [401] Add department management user control
   - Register uc401_DepartmentManage under system/moderator management menus
   - Clone roles from sibling user/group/job management functions
   --------------------------------------------------------------------------- */

SET NOCOUNT ON;

DECLARE @Targets TABLE
(
    ParentId INT NOT NULL,
    DisplayName NVARCHAR(32) NOT NULL,
    Prioritize INT NOT NULL
);

INSERT INTO @Targets (ParentId, DisplayName, Prioritize)
SELECT DISTINCT
    f.IdParent,
    NCHAR(37096) + NCHAR(38272) + NCHAR(31649) + NCHAR(29702),
    ISNULL((SELECT MAX(ISNULL(f2.Prioritize, 0)) FROM dbo.dm_Function f2 WHERE f2.IdParent = f.IdParent), 0) + 1
FROM dbo.dm_Function f
WHERE f.ControlName IN ('uc401_UserManage', 'uc401_GroupManage', 'uc401_JobTitle')
  AND f.IdParent IN
  (
      SELECT IdParent
      FROM dbo.dm_Function
      WHERE ControlName = 'uc401_GroupManage'
  )
  AND NOT EXISTS
  (
      SELECT 1
      FROM dbo.dm_Function existed
      WHERE existed.IdParent = f.IdParent
        AND existed.ControlName = 'uc401_DepartmentManage'
  );

DECLARE @ParentId INT;
DECLARE @DisplayName NVARCHAR(32);
DECLARE @Prioritize INT;

DECLARE target_cursor CURSOR LOCAL FAST_FORWARD FOR
SELECT ParentId, DisplayName, Prioritize
FROM @Targets;

OPEN target_cursor;
FETCH NEXT FROM target_cursor INTO @ParentId, @DisplayName, @Prioritize;

WHILE @@FETCH_STATUS = 0
BEGIN
    DECLARE @FunctionId INT = ISNULL((SELECT MAX(Id) FROM dbo.dm_Function), 0) + 1;

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
        @FunctionId,
        @ParentId,
        @DisplayName,
        'uc401_DepartmentManage',
        @Prioritize,
        1,
        'BO_Department.svg'
    );

    INSERT INTO dbo.dm_FunctionRole (IdFunction, IdRole)
    SELECT @FunctionId, SourceRole.IdRole
    FROM
    (
        SELECT DISTINCT fr.IdRole
        FROM dbo.dm_FunctionRole fr
        INNER JOIN dbo.dm_Function sibling ON sibling.Id = fr.IdFunction
        WHERE sibling.IdParent = @ParentId
          AND sibling.ControlName IN ('uc401_UserManage', 'uc401_GroupManage', 'uc401_JobTitle')
    ) AS SourceRole
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.dm_FunctionRole existed
        WHERE existed.IdFunction = @FunctionId
          AND existed.IdRole = SourceRole.IdRole
    );

    FETCH NEXT FROM target_cursor INTO @ParentId, @DisplayName, @Prioritize;
END;

CLOSE target_cursor;
DEALLOCATE target_cursor;
GO
