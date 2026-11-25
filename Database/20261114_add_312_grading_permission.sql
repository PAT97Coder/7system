-- Register a separate action permission; grant to graders through role management.
-- No existing view-only role receives grading permission automatically.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM dbo.dm_Function WHERE ControlName = 'f312_ExamReview_Grading')
BEGIN
    DECLARE @Parent int = (SELECT TOP 1 Id FROM dbo.dm_Function WHERE ControlName = 'uc312_ExamMgmt');
    IF @Parent IS NULL THROW 51000, N'請先登錄考試結果管理功能。', 1;
    DECLARE @Id int = (SELECT ISNULL(MAX(Id), 0) + 1 FROM dbo.dm_Function WITH (UPDLOCK, HOLDLOCK));
    INSERT dbo.dm_Function (Id, IdParent, DisplayName, ControlName, Prioritize, Status, Images)
    VALUES (@Id, @Parent, N'進行評分', 'f312_ExamReview_Grading', 1, 1, 'Edit.svg');
END;
COMMIT TRANSACTION;
