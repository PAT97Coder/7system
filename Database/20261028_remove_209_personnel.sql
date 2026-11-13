-- Remove the unused ISO14001 personnel feature after updating the application.
-- Refuses to discard personnel events or customized requirement configuration.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.dt209_UpdateUsrReq',N'U') IS NOT NULL
    IF EXISTS(SELECT 1 FROM dbo.dt209_UpdateUsrReq WITH(UPDLOCK,HOLDLOCK))
        THROW 51009,N'ISO14001人員異動仍有資料，停止移除。',1;
IF OBJECT_ID(N'dbo.dt209_UpdateUsrReq_Detail',N'U') IS NOT NULL
    IF EXISTS(SELECT 1 FROM dbo.dt209_UpdateUsrReq_Detail WITH(UPDLOCK,HOLDLOCK))
        THROW 51009,N'ISO14001人員異動明細仍有資料，停止移除。',1;
IF OBJECT_ID(N'dbo.dt209_ReqUpdateDocs',N'U') IS NOT NULL
BEGIN
    IF EXISTS(
        SELECT Id,TypeChange,Category,Content,DocCode,Descript,IdRecordCode
        FROM dbo.dt209_ReqUpdateDocs WITH(UPDLOCK,HOLDLOCK)
        EXCEPT SELECT Id,TypeChange,Category,Content,DocCode,Descript,IdRecordCode FROM dbo.dt201_ReqUpdateDocs)
        THROW 51009,N'ISO14001人員異動要求已有自訂內容，停止移除。',1;
END;
DELETE r FROM dbo.dm_FunctionRole r JOIN dbo.dm_Function f ON f.Id=r.IdFunction
WHERE f.ControlName='uc209_UpdateUsrReq';
DELETE FROM dbo.dm_Function WHERE ControlName='uc209_UpdateUsrReq';
IF OBJECT_ID(N'dbo.trg209_trgAfterInsertOnUpdateUsrReq',N'TR') IS NOT NULL
    DROP TRIGGER dbo.trg209_trgAfterInsertOnUpdateUsrReq;
IF OBJECT_ID(N'dbo.dt209_UpdateUsrReq_Detail',N'U') IS NOT NULL DROP TABLE dbo.dt209_UpdateUsrReq_Detail;
IF OBJECT_ID(N'dbo.dt209_UpdateUsrReq',N'U') IS NOT NULL DROP TABLE dbo.dt209_UpdateUsrReq;
IF OBJECT_ID(N'dbo.dt209_ReqUpdateDocs',N'U') IS NOT NULL DROP TABLE dbo.dt209_ReqUpdateDocs;
COMMIT TRANSACTION;
GO
