-- Remove obsolete offboarding function entries and role links. Keep employee records.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @Permissions TABLE (Id int NOT NULL PRIMARY KEY);
INSERT @Permissions(Id)
SELECT Id FROM dbo.dm_Function
WHERE (Id = 40101 AND ControlName = 'EmployeeOffboardingView')
   OR (Id = 40102 AND ControlName = 'EmployeeOffboardingUpload')
   OR (Id = 40103 AND ControlName = 'EmployeeOffboardingDownload');

DELETE r FROM dbo.dm_FunctionRole r JOIN @Permissions p ON p.Id = r.IdFunction;
DELETE f FROM dbo.dm_Function f JOIN @Permissions p ON p.Id = f.Id;

COMMIT TRANSACTION;
