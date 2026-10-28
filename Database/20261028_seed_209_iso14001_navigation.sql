-- Run after 20261028_add_209_iso14001_documents.sql and the trigger script.
-- Creates independent menus and groups. It does not grant any user membership.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- ISO14001 opens from the existing ISO module; no separate tile on Main.
DECLARE @IsoRoot int=TRY_CONVERT(int,(SELECT ValueT FROM dbo.sys_StaticValue WHERE KeyT='RoleISOAuditDocsMain'));
IF @IsoRoot IS NULL OR NOT EXISTS(SELECT 1 FROM dbo.dm_Function WHERE Id=@IsoRoot)
    THROW 51009,N'找不到ISO主選單設定。',1;

-- Personnel changes are not used by module 209. Keep historical business data.
DELETE r FROM dbo.dm_FunctionRole r JOIN dbo.dm_Function f ON f.Id=r.IdFunction
WHERE f.ControlName='uc209_UpdateUsrReq';
DELETE FROM dbo.dm_Function WHERE ControlName='uc209_UpdateUsrReq';

IF NOT EXISTS (SELECT 1 FROM dbo.sys_StaticValue WHERE KeyT='209DocTypes')
    INSERT dbo.sys_StaticValue(KeyT,ValueT) VALUES('209DocTypes',N'一階;二階;三階');

-- Same signing roles as 201, independent rows. Fixed progress templates can be reused.
INSERT dbo.dt209_Role(Id,DisplayName)
SELECT seed.Id,seed.DisplayName
FROM (VALUES(0,N'呈核'),(1,N'簽名'),(2,N'確認')) seed(Id,DisplayName)
WHERE NOT EXISTS (SELECT 1 FROM dbo.dt209_Role r WHERE r.Id=seed.Id);

DECLARE @Menus TABLE (MenuKey varchar(32), ParentKey varchar(32), Caption nvarchar(32), ControlName varchar(32), Sort int);
INSERT @Menus VALUES
('root',NULL,N'ISO14001文件','mnu209_ISO14001Docs',9),
('documents','root',N'稽核文件','uc209_AuditISOMain',1),
('signing','root',N'電子核簽','mnu209_Signing',2),
('pending','signing',N'待審查','uc209_DigitalSignature',1),
('progress','signing',N'審查進度','uc209_ApprovalProgress',2),
('expired','root',N'過期文件','uc209_ExpiredDocs',3);

DECLARE @Map TABLE (MenuKey varchar(32) PRIMARY KEY, Id int NOT NULL);
IF EXISTS (SELECT m.ControlName FROM @Menus m JOIN dbo.dm_Function f ON f.ControlName=m.ControlName GROUP BY m.ControlName HAVING COUNT(*)>1)
    THROW 51009,N'ISO14001選單代碼重複，請先檢查既有設定。',1;
INSERT @Map SELECT m.MenuKey,f.Id FROM @Menus m JOIN dbo.dm_Function f ON f.ControlName=m.ControlName;

DECLARE @NextId int;
SELECT @NextId=ISNULL(MAX(Id),0) FROM dbo.dm_Function WITH (UPDLOCK,HOLDLOCK);
DECLARE @Key varchar(32),@ParentKey varchar(32),@Caption nvarchar(32),@Control varchar(32),@Sort int,@ParentId int;
WHILE EXISTS(SELECT 1 FROM @Menus m WHERE NOT EXISTS(SELECT 1 FROM @Map x WHERE x.MenuKey=m.MenuKey))
BEGIN
    SELECT TOP(1) @Key=m.MenuKey,@ParentKey=m.ParentKey,@Caption=m.Caption,@Control=m.ControlName,@Sort=m.Sort
    FROM @Menus m WHERE NOT EXISTS(SELECT 1 FROM @Map x WHERE x.MenuKey=m.MenuKey)
      AND (m.ParentKey IS NULL OR EXISTS(SELECT 1 FROM @Map x WHERE x.MenuKey=m.ParentKey))
    ORDER BY CASE WHEN m.ParentKey IS NULL THEN 0 ELSE 1 END,m.Sort;
    SET @ParentId=ISNULL((SELECT Id FROM @Map WHERE MenuKey=@ParentKey),@IsoRoot);
    SET @NextId=@NextId+1;
    INSERT dbo.dm_Function(Id,IdParent,DisplayName,ControlName,Prioritize,Status,Images)
    VALUES(@NextId,@ParentId,@Caption,@Control,@Sort,0,NULL);
    INSERT @Map VALUES(@Key,@NextId);
END;

DECLARE @Root int=(SELECT Id FROM @Map WHERE MenuKey='root');
IF NOT EXISTS(SELECT 1 FROM dbo.sys_StaticValue WHERE KeyT='RoleISO14001DocsMain')
    INSERT dbo.sys_StaticValue(KeyT,ValueT) VALUES('RoleISO14001DocsMain',CONVERT(nvarchar(32),@Root));
ELSE IF EXISTS(SELECT 1 FROM dbo.sys_StaticValue WHERE KeyT='RoleISO14001DocsMain' AND ValueT<>CONVERT(nvarchar(32),@Root))
    THROW 51009,N'ISO14001功能設定與選單不一致。',1;

-- Group definitions only: users are assigned through the existing group editor.
INSERT dbo.dm_Group(DisplayName,Describe,Prioritize,IdDept)
SELECT N'ISO14001組',N'ISO14001文件瀏覽',0,d.IdDept
FROM (SELECT DISTINCT IdDept FROM dbo.dm_Group WHERE DisplayName=N'ISO組') d
WHERE NOT EXISTS(SELECT 1 FROM dbo.dm_Group g WITH(UPDLOCK,HOLDLOCK)
                 WHERE g.DisplayName=N'ISO14001組' AND ISNULL(g.IdDept,'')=ISNULL(d.IdDept,''));
INSERT dbo.dm_Group(DisplayName,Describe,Prioritize,IdDept)
SELECT s.DisplayName,N'ISO14001文件操作權限',0,'7'
FROM (VALUES(N'ISO14001文件【編輯及刪除】'),(N'ISO14001文件【僅編輯】')) s(DisplayName)
WHERE NOT EXISTS(SELECT 1 FROM dbo.dm_Group g WITH(UPDLOCK,HOLDLOCK) WHERE g.DisplayName=s.DisplayName);

-- Menu roles intentionally assigned by the administrator through the existing
-- role editor; 201 roles/memberships do not automatically grant access to 209.
COMMIT TRANSACTION;
GO
