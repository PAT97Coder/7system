-- Align sys_NotesMail with the EF model used by module 312.
-- Nullable to preserve existing notifications; safe to rerun.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.sys_NotesMail', N'U') IS NULL
    THROW 51000, N'Missing dbo.sys_NotesMail.', 1;
IF COL_LENGTH(N'dbo.sys_NotesMail', N'EventKey') IS NULL
    ALTER TABLE dbo.sys_NotesMail ADD EventKey varchar(180) NULL;
COMMIT TRANSACTION;