-- Minimal pre-migration fixture for a disposable Exam312 test database.

-- Never execute against an application database.

CREATE TABLE dbo.[dm_Function] (
    [Id] int NOT NULL,
    [IdParent] int NOT NULL,
    [DisplayName] nvarchar(32) NOT NULL,
    [ControlName] varchar(32) NULL,
    [Prioritize] int NULL,
    [Status] bit NULL,
    [Images] varchar(32) NULL,
    CONSTRAINT [PK_dm_Function] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dm_FunctionRole] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdFunction] int NOT NULL,
    [IdRole] int NOT NULL,
    CONSTRAINT [PK_dm_FunctionRole] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dm_JobTitle] (
    [Id] varchar(8) NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_dm_JobTitle] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dm_User] (
    [Id] varchar(10) NOT NULL,
    [DisplayName] nvarchar(8) NULL,
    [SecondaryPassword] varchar(64) NULL,
    [DateCreate] date NOT NULL,
    [IdDepartment] varchar(4) NULL,
    [DisplayNameVN] nvarchar(128) NULL,
    [DOB] date NULL,
    [CitizenID] varchar(16) NULL,
    [Nationality] nvarchar(16) NULL,
    [JobCode] varchar(16) NULL,
    [PCName] varchar(32) NULL,
    [IPAddress] varchar(15) NULL,
    [Addr] nvarchar(128) NULL,
    [PhoneNum1] varchar(16) NULL,
    [PhoneNum2] varchar(16) NULL,
    [Status] int NULL,
    [Sex] bit NULL,
    [ActualJobCode] varchar(16) NULL,
    [LastUpdate] datetime NULL,
    [ResignPlan] date NULL,
    [RecognizedEducation] nvarchar(64) NULL,
    [ResignDate] date NULL,
    [JobEffectiveDate] date NULL,
    [EducationName] nvarchar(64) NULL,
    CONSTRAINT [PK_dm_User] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dm_UserRole] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [IdUser] varchar(10) NOT NULL,
    [IdRole] int NOT NULL,
    CONSTRAINT [PK_dm_UserRole] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_Answers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [QuesId] int NOT NULL,
    [DisplayName] nvarchar(2048) NOT NULL,
    [ImageName] varchar(128) NULL,
    [TrueAns] bit NOT NULL,
    CONSTRAINT [PK_dt312_Answers] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_ExamMgmt] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DisplayName] nvarchar(256) NOT NULL,
    [CreateAt] date NOT NULL,
    [RoundNumber] int NOT NULL,
    [FinishNotifyDate] date NULL,
    CONSTRAINT [PK_dt312_ExamMgmt] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_ExamUser] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ExamId] int NOT NULL,
    [UserId] varchar(10) NOT NULL,
    [SubmitAt] datetime NULL,
    [Score] int NULL,
    [IsPass] bit NULL,
    [ExamData] nvarchar(max) NULL,
    CONSTRAINT [PK_dt312_ExamUser] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_Groups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DisplayName] nvarchar(256) NOT NULL,
    [CreateAt] date NOT NULL,
    [CreateBy] varchar(10) NOT NULL,
    [RemoveAt] date NULL,
    [RemoveBy] varchar(10) NULL,
    CONSTRAINT [PK_dt312_Groups] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_Questions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [GroupId] int NOT NULL,
    [DisplayName] nvarchar(2048) NOT NULL,
    [ImageName] varchar(128) NULL,
    [IsmultiAns] bit NOT NULL,
    CONSTRAINT [PK_dt312_Questions] PRIMARY KEY ([Id])
);

CREATE TABLE dbo.[dt312_Setting] (
    [id] int NOT NULL,
    [CountEmp] int NOT NULL,
    [PassingScore] int NOT NULL,
    [QuesCount] int NOT NULL,
    [TestDuration] int NOT NULL,
    [CycleCreate] float NOT NULL,
    CONSTRAINT [PK_dt312_Setting] PRIMARY KEY ([id])
);
