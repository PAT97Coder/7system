SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   314 Chinese HSK exam
   - Create independent question bank and exam tables for HSK4 / HSK5
   - Reading only for HSK4 / HSK5
   - Exam generation uses section-level 50/50 split between HSK4 and HSK5
--------------------------------------------------------------------------- */

IF OBJECT_ID('dbo.dt314_HskQuestions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskQuestions
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        LevelCode VARCHAR(10) NOT NULL,
        SectionCode VARCHAR(20) NOT NULL,
        QuestionType VARCHAR(40) NOT NULL,
        DisplayText NVARCHAR(MAX) NOT NULL,
        ImageName NVARCHAR(256) NULL,
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
        CONSTRAINT CK_dt314_HskQuestions_LevelCode
            CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskQuestions_SectionCode
            CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskQuestions_QuestionType
            CHECK (QuestionType IN ('SingleChoice', 'SentenceOrder'))
    );
END;

IF OBJECT_ID('dbo.dt314_HskAnswers', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskAnswers
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        QuesId INT NOT NULL,
        DisplayText NVARCHAR(MAX) NOT NULL,
        ImageName NVARCHAR(256) NULL,
        TrueAns BIT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_TrueAns DEFAULT (0),
        DisplayOrder INT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_DisplayOrder DEFAULT (1),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt314_HskAnswers_IsActive DEFAULT (1),
        CONSTRAINT FK_dt314_HskAnswers_Question
            FOREIGN KEY (QuesId) REFERENCES dbo.dt314_HskQuestions(Id)
    );
END;

IF OBJECT_ID('dbo.dt314_HskExamMgmt', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskExamMgmt
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        Code VARCHAR(30) NOT NULL,
        DisplayName NVARCHAR(256) NOT NULL,
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
        Remark NVARCHAR(1000) NULL,
        CONSTRAINT CK_dt314_HskExamMgmt_TestDuration
            CHECK (TestDuration > 0),
        CONSTRAINT CK_dt314_HskExamMgmt_PassingScore
            CHECK (PassingScore BETWEEN 0 AND 100),
        CONSTRAINT CK_dt314_HskExamMgmt_QuestionCounts
            CHECK (ReadingCount > 0 AND WritingCount >= 0),
        CONSTRAINT CK_dt314_HskExamMgmt_SectionEvenSplit
            CHECK (ReadingCount % 2 = 0)
    );
END;

IF OBJECT_ID('dbo.dt314_HskExamUser', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskExamUser
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        ExamCode VARCHAR(30) NOT NULL,
        IdUser VARCHAR(10) NOT NULL,
        SubmitTime DATETIME NULL,
        Score INT NULL,
        IsPass BIT NULL,
        ExamData NVARCHAR(MAX) NULL,
        CONSTRAINT CK_dt314_HskExamUser_Score
            CHECK (Score IS NULL OR Score BETWEEN 0 AND 100)
    );
END;

IF OBJECT_ID('dbo.dt314_HskExamQuestion', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskExamQuestion
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        ExamCode VARCHAR(30) NOT NULL,
        QuestionId INT NOT NULL,
        LevelCode VARCHAR(10) NOT NULL,
        SectionCode VARCHAR(20) NOT NULL,
        QuestionType VARCHAR(40) NOT NULL,
        DisplayOrder INT NOT NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskExamQuestion_CreatedDate DEFAULT (GETDATE()),
        CONSTRAINT FK_dt314_HskExamQuestion_Question
            FOREIGN KEY (QuestionId) REFERENCES dbo.dt314_HskQuestions(Id),
        CONSTRAINT CK_dt314_HskExamQuestion_LevelCode
            CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskExamQuestion_SectionCode
            CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskExamQuestion_QuestionType
            CHECK (QuestionType IN ('SingleChoice', 'SentenceOrder'))
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskQuestions_Filter'
      AND object_id = OBJECT_ID('dbo.dt314_HskQuestions')
)
BEGIN
    CREATE INDEX IX_dt314_HskQuestions_Filter
        ON dbo.dt314_HskQuestions (IsActive, LevelCode, SectionCode, QuestionType);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskAnswers_QuesId'
      AND object_id = OBJECT_ID('dbo.dt314_HskAnswers')
)
BEGIN
    CREATE INDEX IX_dt314_HskAnswers_QuesId
        ON dbo.dt314_HskAnswers (QuesId, IsActive, DisplayOrder);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_dt314_HskExamMgmt_Code'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamMgmt')
)
BEGIN
    CREATE UNIQUE INDEX UX_dt314_HskExamMgmt_Code
        ON dbo.dt314_HskExamMgmt (Code);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskExamUser_User'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamUser')
)
BEGIN
    CREATE INDEX IX_dt314_HskExamUser_User
        ON dbo.dt314_HskExamUser (IdUser, SubmitTime, ExamCode);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskExamUser_ExamCode'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamUser')
)
BEGIN
    CREATE INDEX IX_dt314_HskExamUser_ExamCode
        ON dbo.dt314_HskExamUser (ExamCode);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_dt314_HskExamQuestion_ExamOrder'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamQuestion')
)
BEGIN
    CREATE UNIQUE INDEX UX_dt314_HskExamQuestion_ExamOrder
        ON dbo.dt314_HskExamQuestion (ExamCode, DisplayOrder);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskExamQuestion_ExamSection'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamQuestion')
)
BEGIN
    CREATE INDEX IX_dt314_HskExamQuestion_ExamSection
        ON dbo.dt314_HskExamQuestion (ExamCode, SectionCode, LevelCode);
END;
