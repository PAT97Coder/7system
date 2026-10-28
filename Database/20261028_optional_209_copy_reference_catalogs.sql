-- OPTIONAL: use only after reviewing the 201 reference catalogue for ISO14001.
-- Copies configuration, not documents/attachments/workflow instances/personnel.
-- Articles and document codes are copied literally: these are NOT ISO14001 clauses.
-- Edit the target catalogue to the approved ISO14001 codes/requirements afterwards.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
INSERT dbo.dt209_RecordCode(Id,Code,DisplayName,Articles)
SELECT s.Id,s.Code,s.DisplayName,s.Articles FROM dbo.dt201_RecordCode s
WHERE NOT EXISTS(SELECT 1 FROM dbo.dt209_RecordCode t WHERE t.Id=s.Id);
COMMIT TRANSACTION;
GO
