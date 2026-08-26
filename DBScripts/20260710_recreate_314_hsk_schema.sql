USE DBDocumentManagementSystem;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRAN;

    IF OBJECT_ID('dbo.vw314_HskReadingQuestionBank', 'V') IS NOT NULL
        DROP VIEW dbo.vw314_HskReadingQuestionBank;

    IF OBJECT_ID('dbo.dt314_HskExamUser', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskExamUser;

    IF OBJECT_ID('dbo.dt314_HskExamQuestion', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskExamQuestion;

    IF OBJECT_ID('dbo.dt314_HskAnswers', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskAnswers;

    IF OBJECT_ID('dbo.dt314_HskQuestions', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskQuestions;

    IF OBJECT_ID('dbo.dt314_HskQuestionGroup', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskQuestionGroup;

    IF OBJECT_ID('dbo.dt314_HskExamMgmt', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskExamMgmt;

    IF OBJECT_ID('dbo.dt314_HskSourcePaper', 'U') IS NOT NULL
        DROP TABLE dbo.dt314_HskSourcePaper;

    CREATE TABLE dbo.dt314_HskExamMgmt
    (
        Id INT IDENTITY(1,1) NOT NULL,
        Code VARCHAR(30) NOT NULL,
        DisplayName NVARCHAR(200) NOT NULL,
        CreateTime DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_CreateTime DEFAULT (GETDATE()),
        StartTime DATETIME NULL,
        FinishTime DATETIME NULL,
        TestDuration INT NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_TestDuration DEFAULT (125),
        PassingScore INT NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_PassingScore DEFAULT (60),
        ReadingCount INT NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_ReadingCount DEFAULT (80),
        WritingCount INT NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_WritingCount DEFAULT (0),
        CreatedBy VARCHAR(10) NULL,
        ExamType NVARCHAR(50) NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_ExamType DEFAULT (N'模擬考試'),
        HskRatio NVARCHAR(20) NOT NULL
            CONSTRAINT DF_dt314_HskExamMgmt_HskRatio DEFAULT (N'9:1'),
        Remark NVARCHAR(500) NULL,

        CONSTRAINT PK_dt314_HskExamMgmt PRIMARY KEY (Id),
        CONSTRAINT CK_dt314_HskExamMgmt_TestDuration CHECK (TestDuration > 0),
        CONSTRAINT CK_dt314_HskExamMgmt_PassingScore CHECK (PassingScore BETWEEN 0 AND 100),
        CONSTRAINT CK_dt314_HskExamMgmt_QuestionCounts CHECK (ReadingCount >= 0 AND WritingCount >= 0 AND ReadingCount + WritingCount > 0)
    );

    CREATE UNIQUE INDEX UX_dt314_HskExamMgmt_Code
        ON dbo.dt314_HskExamMgmt (Code);

    CREATE TABLE dbo.dt314_HskQuestionGroup
    (
        Id INT IDENTITY(1,1) NOT NULL,
        LevelCode VARCHAR(10) NOT NULL,
        SectionCode VARCHAR(20) NOT NULL,
        PartCode VARCHAR(30) NOT NULL,
        GroupType VARCHAR(40) NOT NULL,
        GroupCode VARCHAR(50) NULL,
        Title NVARCHAR(300) NULL,
        InstructionText NVARCHAR(MAX) NULL,
        SharedPassage NVARCHAR(MAX) NULL,
        SharedOptionPool NVARCHAR(MAX) NULL,
        SourceQuestionFrom INT NULL,
        SourceQuestionTo INT NULL,
        QuestionCount INT NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_QuestionCount DEFAULT (1),
        RandomAsUnit BIT NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_RandomAsUnit DEFAULT (1),
        RandomWeight DECIMAL(9,2) NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_RandomWeight DEFAULT (1),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_IsActive DEFAULT (1),
        Remark NVARCHAR(500) NULL,
        CreatedBy VARCHAR(10) NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_CreatedDate DEFAULT (GETDATE()),

        CONSTRAINT PK_dt314_HskQuestionGroup PRIMARY KEY (Id),
        CONSTRAINT CK_dt314_HskQuestionGroup_LevelCode CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskQuestionGroup_SectionCode CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskQuestionGroup_PartCode CHECK (PartCode IN ('ReadingPart1', 'ReadingPart2', 'ReadingPart3')),
        CONSTRAINT CK_dt314_HskQuestionGroup_GroupType CHECK (GroupType IN ('SingleQuestion', 'SharedPassage', 'SharedWordBank', 'SentenceOrder', 'PassageCloze')),
        CONSTRAINT CK_dt314_HskQuestionGroup_QuestionCount CHECK (QuestionCount > 0),
        CONSTRAINT CK_dt314_HskQuestionGroup_RandomWeight CHECK (RandomWeight > 0)
    );

    CREATE INDEX IX_dt314_HskQuestionGroup_Filter
        ON dbo.dt314_HskQuestionGroup (IsActive, LevelCode, SectionCode, PartCode, GroupType);

    CREATE TABLE dbo.dt314_HskQuestions
    (
        Id INT IDENTITY(1,1) NOT NULL,
        LevelCode VARCHAR(10) NOT NULL,
        SectionCode VARCHAR(20) NOT NULL,
        QuestionType VARCHAR(40) NOT NULL,
        DisplayText NVARCHAR(MAX) NOT NULL,
        ImageName NVARCHAR(260) NULL,
        IsMultiAns BIT NOT NULL
            CONSTRAINT DF_dt314_HskQuestions_IsMultiAns DEFAULT (0),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt314_HskQuestions_IsActive DEFAULT (1),
        CreatedBy VARCHAR(10) NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskQuestions_CreatedDate DEFAULT (GETDATE()),
        UpdatedBy VARCHAR(10) NULL,
        UpdatedDate DATETIME NULL,
        Remark NVARCHAR(500) NULL,
        GroupId INT NULL,
        PartCode VARCHAR(30) NULL,
        SourceQuestionNo INT NULL,
        QuestionCode VARCHAR(50) NULL,
        UsageCount INT NOT NULL
            CONSTRAINT DF_dt314_HskQuestions_UsageCount DEFAULT (0),
        LastUsedDate DATETIME NULL,

        CONSTRAINT PK_dt314_HskQuestions PRIMARY KEY (Id),
        CONSTRAINT FK_dt314_HskQuestions_Group
            FOREIGN KEY (GroupId) REFERENCES dbo.dt314_HskQuestionGroup(Id),
        CONSTRAINT CK_dt314_HskQuestions_LevelCode CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskQuestions_SectionCode CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskQuestions_QuestionType CHECK (QuestionType IN ('SingleChoice', 'SentenceOrder')),
        CONSTRAINT CK_dt314_HskQuestions_PartCode CHECK (PartCode IS NULL OR PartCode IN ('ReadingPart1', 'ReadingPart2', 'ReadingPart3'))
    );

    CREATE INDEX IX_dt314_HskQuestions_Filter
        ON dbo.dt314_HskQuestions (IsActive, LevelCode, SectionCode, QuestionType);

    CREATE INDEX IX_dt314_HskQuestions_Group
        ON dbo.dt314_HskQuestions (GroupId, IsActive, SourceQuestionNo);

    CREATE INDEX IX_dt314_HskQuestions_Part
        ON dbo.dt314_HskQuestions (LevelCode, SectionCode, PartCode, IsActive);

    CREATE TABLE dbo.dt314_HskAnswers
    (
        Id INT IDENTITY(1,1) NOT NULL,
        QuesId INT NOT NULL,
        DisplayText NVARCHAR(MAX) NOT NULL,
        ImageName NVARCHAR(260) NULL,
        TrueAns BIT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_TrueAns DEFAULT (0),
        DisplayOrder INT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_DisplayOrder DEFAULT (1),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_IsActive DEFAULT (1),

        CONSTRAINT PK_dt314_HskAnswers PRIMARY KEY (Id),
        CONSTRAINT FK_dt314_HskAnswers_Question
            FOREIGN KEY (QuesId) REFERENCES dbo.dt314_HskQuestions(Id)
    );

    CREATE INDEX IX_dt314_HskAnswers_QuesId
        ON dbo.dt314_HskAnswers (QuesId, IsActive, DisplayOrder);

    CREATE TABLE dbo.dt314_HskExamUser
    (
        Id INT IDENTITY(1,1) NOT NULL,
        ExamCode VARCHAR(30) NOT NULL,
        IdUser VARCHAR(10) NOT NULL,
        SubmitTime DATETIME NULL,
        Score INT NULL,
        IsPass BIT NULL,
        ExamData NVARCHAR(MAX) NULL,

        CONSTRAINT PK_dt314_HskExamUser PRIMARY KEY (Id),
        CONSTRAINT CK_dt314_HskExamUser_Score CHECK (Score IS NULL OR Score BETWEEN 0 AND 100)
    );

    CREATE INDEX IX_dt314_HskExamUser_User
        ON dbo.dt314_HskExamUser (IdUser, SubmitTime, ExamCode);

    CREATE INDEX IX_dt314_HskExamUser_ExamCode
        ON dbo.dt314_HskExamUser (ExamCode);

    CREATE TABLE dbo.dt314_HskExamQuestion
    (
        Id INT IDENTITY(1,1) NOT NULL,
        ExamCode VARCHAR(30) NOT NULL,
        QuestionId INT NOT NULL,
        LevelCode VARCHAR(10) NOT NULL,
        SectionCode VARCHAR(20) NOT NULL,
        QuestionType VARCHAR(40) NOT NULL,
        DisplayOrder INT NOT NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskExamQuestion_CreatedDate DEFAULT (GETDATE()),
        GroupId INT NULL,
        GroupDisplayOrder INT NULL,
        QuestionOrderInGroup INT NULL,

        CONSTRAINT PK_dt314_HskExamQuestion PRIMARY KEY (Id),
        CONSTRAINT FK_dt314_HskExamQuestion_Question
            FOREIGN KEY (QuestionId) REFERENCES dbo.dt314_HskQuestions(Id),
        CONSTRAINT FK_dt314_HskExamQuestion_Group
            FOREIGN KEY (GroupId) REFERENCES dbo.dt314_HskQuestionGroup(Id),
        CONSTRAINT CK_dt314_HskExamQuestion_LevelCode CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskExamQuestion_SectionCode CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskExamQuestion_QuestionType CHECK (QuestionType IN ('SingleChoice', 'SentenceOrder'))
    );

    CREATE UNIQUE INDEX UX_dt314_HskExamQuestion_ExamOrder
        ON dbo.dt314_HskExamQuestion (ExamCode, DisplayOrder);

    CREATE INDEX IX_dt314_HskExamQuestion_ExamSection
        ON dbo.dt314_HskExamQuestion (ExamCode, SectionCode, LevelCode);

    CREATE INDEX IX_dt314_HskExamQuestion_Group
        ON dbo.dt314_HskExamQuestion (ExamCode, GroupDisplayOrder, QuestionOrderInGroup);

    COMMIT TRAN;

    PRINT 'Recreated HSK 314 schema successfully.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;

    PRINT 'FAILED. Rolled back.';
    THROW;
END CATCH;
GO
