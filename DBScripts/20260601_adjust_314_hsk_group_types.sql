SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   Align dt314_HskQuestionGroup.GroupType with the simplified mixed-bank model
   used by the application code.
--------------------------------------------------------------------------- */

IF OBJECT_ID('dbo.dt314_HskQuestionGroup', 'U') IS NULL
BEGIN
    RAISERROR(N'Please run 20260601_add_314_hsk_reading_group_support.sql first.', 16, 1);
    RETURN;
END;

IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_dt314_HskQuestionGroup_GroupType'
      AND parent_object_id = OBJECT_ID('dbo.dt314_HskQuestionGroup')
)
BEGIN
    ALTER TABLE dbo.dt314_HskQuestionGroup
    DROP CONSTRAINT CK_dt314_HskQuestionGroup_GroupType;
END;

ALTER TABLE dbo.dt314_HskQuestionGroup
ADD CONSTRAINT CK_dt314_HskQuestionGroup_GroupType
    CHECK (GroupType IN ('SingleQuestion', 'SharedPassage', 'SharedWordBank', 'SentenceOrder', 'PassageCloze'));
