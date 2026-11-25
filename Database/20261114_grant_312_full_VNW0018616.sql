-- Target: the database configured by the current Debug application.
-- Run 20261114_add_312_grading_permission.sql first.
-- Dedicated role avoids expanding permissions of other users.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
DECLARE @User varchar(10) = 'VNW0018616';
IF NOT EXISTS (SELECT 1 FROM dbo.dm_User WHERE Id = @User)
    THROW 51000, 'User VNW0018616 does not exist.', 1;
DECLARE @Module int = (SELECT TOP 1 IdParent FROM dbo.dm_Function WHERE ControlName = 'uc312_ExamMgmt');
IF @Module IS NULL OR NOT EXISTS (SELECT 1 FROM dbo.dm_Function WHERE ControlName = 'f312_ExamReview_Grading')
    THROW 51000, 'Module 312 or grading permission is missing.', 1;
DECLARE @Role int = (SELECT Id FROM dbo.dm_Role WITH (UPDLOCK, HOLDLOCK) WHERE DisplayName = N'312 Full - VNW0018616');
IF @Role IS NULL
BEGIN
    INSERT dbo.dm_Role(DisplayName, Describe)
    VALUES (N'312 Full - VNW0018616', N'Full module 312 access, including essay grading, for VNW0018616.');
    SET @Role = CONVERT(int, SCOPE_IDENTITY());
END;
IF EXISTS (SELECT 1 FROM dbo.dm_UserRole WHERE IdRole = @Role AND IdUser <> @User)
    THROW 51000, 'Dedicated role is assigned to another user; no changes committed.', 1;
DECLARE @Functions TABLE (Id int PRIMARY KEY);
;WITH ModuleFunctions AS (
    SELECT Id FROM dbo.dm_Function WHERE Id = @Module
    UNION ALL
    SELECT f.Id FROM dbo.dm_Function f JOIN ModuleFunctions p ON f.IdParent = p.Id
)
INSERT @Functions SELECT DISTINCT Id FROM ModuleFunctions OPTION (MAXRECURSION 100);
;WITH Ancestors AS (
    SELECT Id, IdParent FROM dbo.dm_Function WHERE Id = @Module
    UNION ALL
    SELECT f.Id, f.IdParent FROM dbo.dm_Function f JOIN Ancestors c ON c.IdParent = f.Id
    WHERE f.Id <> c.Id
)
INSERT @Functions SELECT DISTINCT Id FROM Ancestors a
WHERE NOT EXISTS (SELECT 1 FROM @Functions x WHERE x.Id = a.Id) OPTION (MAXRECURSION 100);
IF NOT EXISTS (SELECT 1 FROM dbo.dm_UserRole WITH (UPDLOCK, HOLDLOCK) WHERE IdUser = @User AND IdRole = @Role)
    INSERT dbo.dm_UserRole(IdUser, IdRole) VALUES (@User, @Role);
INSERT dbo.dm_FunctionRole(IdFunction, IdRole)
SELECT f.Id, @Role FROM @Functions f
WHERE NOT EXISTS (SELECT 1 FROM dbo.dm_FunctionRole WITH (UPDLOCK, HOLDLOCK) WHERE IdFunction = f.Id AND IdRole = @Role);
COMMIT TRANSACTION;
