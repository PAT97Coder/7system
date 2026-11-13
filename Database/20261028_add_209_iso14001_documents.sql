-- Module 209: ISO14001 documents. Run on the selected application database.
-- No ISO17025 documents, attachments or group members are copied.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.dt209_Base', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_Base] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdParent] int NOT NULL,
    [DocCode] nvarchar(32) NOT NULL,
    [DisplayName] nvarchar(256) NOT NULL,
    [IdDept] varchar(4) NULL,
    [IsFinalNode] bit NULL,
    [IsDel] bit NULL,
    [DelTime] datetime NULL,
    [IsPaperType] bit NULL,
    [NotifyCycle] int NULL,
    [IsDisable] bit NULL,
    [DisplayNameVN] nvarchar(512) NULL,
    [DocType] nvarchar(8) NULL,
    [IdRecordCode] int NOT NULL,
    CONSTRAINT [PK_dt209_Base] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.dt209_Forms', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_Forms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdBase] int NOT NULL,
    [Code] nvarchar(32) NULL,
    [DisplayName] nvarchar(512) NOT NULL,
    [UploadTime] datetime NOT NULL,
    [UploadUser] varchar(10) NOT NULL,
    [AttId] int NULL,
    [IsProcessing] bit NULL,
    [DigitalSign] bit NULL,
    [DisplayNameVN] nvarchar(512) NULL,
    [IsCancel] bit NULL,
    [NextStepProg] varchar(10) NULL,
    [Descript] nvarchar(256) NULL,
    [IsDel] bit NULL,
    [DelTime] datetime NULL,
    CONSTRAINT [PK_dt209_Forms] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.dt209_ProgInfo', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_ProgInfo] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdForm] int NOT NULL,
    [IdUsr] nvarchar(10) NOT NULL,
    [Desc] nvarchar(255) NOT NULL,
    [RespTime] datetime NOT NULL,
    [SendNoteTime] datetime NULL,
    CONSTRAINT [PK_dt209_ProgInfo] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.dt209_Progress', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_Progress] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdForm] int NOT NULL,
    [IdUsr] nvarchar(10) NOT NULL,
    [IdRole] int NOT NULL,
    CONSTRAINT [PK_dt209_Progress] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.dt209_RecordCode', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_RecordCode] (
    [Id] int NOT NULL,
    [Code] varchar(16) NOT NULL,
    [DisplayName] nvarchar(1024) NOT NULL,
    [Articles] varchar(64) NULL,
    CONSTRAINT [PK_dt209_RecordCode] PRIMARY KEY ([Id])
);
END;


IF OBJECT_ID(N'dbo.dt209_Role', N'U') IS NULL
BEGIN
CREATE TABLE dbo.[dt209_Role] (
    [Id] int NOT NULL,
    [DisplayName] nvarchar(64) NOT NULL,
    CONSTRAINT [PK_dt209_Role] PRIMARY KEY ([Id])
);
END;


COMMIT TRANSACTION;
GO
