SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   314 Chinese HSK exam - reading group support
   Goal:
   - Keep existing dt314_HskQuestions / dt314_HskAnswers structure usable
   - Add group metadata so real HSK4 / HSK5 reading blocks can be stored
   - Support shared passages, shared word banks, and group-based randomization

   Notes:
   - Reading only
   - Existing records continue to work with nullable group columns
--------------------------------------------------------------------------- */

IF OBJECT_ID('dbo.dt314_HskSourcePaper', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskSourcePaper
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        LevelCode VARCHAR(10) NOT NULL,
        PaperCode VARCHAR(50) NOT NULL,
        SourceFileName NVARCHAR(260) NULL,
        SourceFilePath NVARCHAR(1000) NULL,
        ExamYear INT NULL,
        PageCount INT NULL,
        ReadingQuestionFrom INT NULL,
        ReadingQuestionTo INT NULL,
        Note NVARCHAR(1000) NULL,
        CreatedBy VARCHAR(10) NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskSourcePaper_CreatedDate DEFAULT (GETDATE()),
        UpdatedBy VARCHAR(10) NULL,
        UpdatedDate DATETIME NULL,
        CONSTRAINT CK_dt314_HskSourcePaper_LevelCode
            CHECK (LevelCode IN ('HSK4', 'HSK5'))
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_dt314_HskSourcePaper_LevelPaper'
      AND object_id = OBJECT_ID('dbo.dt314_HskSourcePaper')
)
BEGIN
    CREATE UNIQUE INDEX UX_dt314_HskSourcePaper_LevelPaper
        ON dbo.dt314_HskSourcePaper (LevelCode, PaperCode);
END;

IF OBJECT_ID('dbo.dt314_HskQuestionGroup', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt314_HskQuestionGroup
    (
        Id INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
        SourcePaperId INT NULL,
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
        RandomWeight DECIMAL(9, 2) NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_RandomWeight DEFAULT (1),
        IsActive BIT NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_IsActive DEFAULT (1),
        Remark NVARCHAR(500) NULL,
        CreatedBy VARCHAR(10) NULL,
        CreatedDate DATETIME NOT NULL
            CONSTRAINT DF_dt314_HskQuestionGroup_CreatedDate DEFAULT (GETDATE()),
        UpdatedBy VARCHAR(10) NULL,
        UpdatedDate DATETIME NULL,
        CONSTRAINT FK_dt314_HskQuestionGroup_SourcePaper
            FOREIGN KEY (SourcePaperId) REFERENCES dbo.dt314_HskSourcePaper(Id),
        CONSTRAINT CK_dt314_HskQuestionGroup_LevelCode
            CHECK (LevelCode IN ('HSK4', 'HSK5')),
        CONSTRAINT CK_dt314_HskQuestionGroup_SectionCode
            CHECK (SectionCode IN ('Reading')),
        CONSTRAINT CK_dt314_HskQuestionGroup_PartCode
            CHECK (PartCode IN ('ReadingPart1', 'ReadingPart2', 'ReadingPart3')),
        CONSTRAINT CK_dt314_HskQuestionGroup_GroupType
            CHECK (GroupType IN ('SingleQuestion', 'SharedPassage', 'SharedWordBank', 'SentenceOrder', 'PassageCloze')),
        CONSTRAINT CK_dt314_HskQuestionGroup_QuestionCount
            CHECK (QuestionCount > 0),
        CONSTRAINT CK_dt314_HskQuestionGroup_RandomWeight
            CHECK (RandomWeight > 0)
    );
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskQuestionGroup_Filter'
      AND object_id = OBJECT_ID('dbo.dt314_HskQuestionGroup')
)
BEGIN
    CREATE INDEX IX_dt314_HskQuestionGroup_Filter
        ON dbo.dt314_HskQuestionGroup (IsActive, LevelCode, SectionCode, PartCode, GroupType);
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'GroupId') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD GroupId INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'SourcePaperId') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD SourcePaperId INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'PartCode') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD PartCode VARCHAR(30) NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'SourceQuestionNo') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD SourceQuestionNo INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'SourceQuestionSubNo') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD SourceQuestionSubNo INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'QuestionCode') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD QuestionCode VARCHAR(50) NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'TopicTag') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD TopicTag NVARCHAR(200) NULL;
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'DifficultyWeight') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD DifficultyWeight DECIMAL(9, 2) NOT NULL
        CONSTRAINT DF_dt314_HskQuestions_DifficultyWeight DEFAULT (1);
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'UsageCount') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD UsageCount INT NOT NULL
        CONSTRAINT DF_dt314_HskQuestions_UsageCount DEFAULT (0);
END;

IF COL_LENGTH('dbo.dt314_HskQuestions', 'LastUsedDate') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD LastUsedDate DATETIME NULL;
END;

GO

SET NOCOUNT ON;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_dt314_HskQuestions_Group'
)
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD CONSTRAINT FK_dt314_HskQuestions_Group
        FOREIGN KEY (GroupId) REFERENCES dbo.dt314_HskQuestionGroup(Id);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_dt314_HskQuestions_SourcePaper'
)
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD CONSTRAINT FK_dt314_HskQuestions_SourcePaper
        FOREIGN KEY (SourcePaperId) REFERENCES dbo.dt314_HskSourcePaper(Id);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = 'CK_dt314_HskQuestions_PartCode'
)
BEGIN
    ALTER TABLE dbo.dt314_HskQuestions
    ADD CONSTRAINT CK_dt314_HskQuestions_PartCode
        CHECK (PartCode IS NULL OR PartCode IN ('ReadingPart1', 'ReadingPart2', 'ReadingPart3'));
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskQuestions_Group'
      AND object_id = OBJECT_ID('dbo.dt314_HskQuestions')
)
BEGIN
    CREATE INDEX IX_dt314_HskQuestions_Group
        ON dbo.dt314_HskQuestions (GroupId, IsActive, SourceQuestionNo);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskQuestions_Source'
      AND object_id = OBJECT_ID('dbo.dt314_HskQuestions')
)
BEGIN
    CREATE INDEX IX_dt314_HskQuestions_Source
        ON dbo.dt314_HskQuestions (SourcePaperId, SectionCode, PartCode, SourceQuestionNo);
END;

IF COL_LENGTH('dbo.dt314_HskExamQuestion', 'GroupId') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskExamQuestion
    ADD GroupId INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskExamQuestion', 'GroupDisplayOrder') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskExamQuestion
    ADD GroupDisplayOrder INT NULL;
END;

IF COL_LENGTH('dbo.dt314_HskExamQuestion', 'QuestionOrderInGroup') IS NULL
BEGIN
    ALTER TABLE dbo.dt314_HskExamQuestion
    ADD QuestionOrderInGroup INT NULL;
END;

GO

SET NOCOUNT ON;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_dt314_HskExamQuestion_Group'
)
BEGIN
    ALTER TABLE dbo.dt314_HskExamQuestion
    ADD CONSTRAINT FK_dt314_HskExamQuestion_Group
        FOREIGN KEY (GroupId) REFERENCES dbo.dt314_HskQuestionGroup(Id);
END;

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE name = 'IX_dt314_HskExamQuestion_Group'
      AND object_id = OBJECT_ID('dbo.dt314_HskExamQuestion')
)
BEGIN
    CREATE INDEX IX_dt314_HskExamQuestion_Group
        ON dbo.dt314_HskExamQuestion (ExamCode, GroupDisplayOrder, QuestionOrderInGroup);
END;

IF OBJECT_ID('dbo.vw314_HskReadingQuestionBank', 'V') IS NOT NULL
BEGIN
    DROP VIEW dbo.vw314_HskReadingQuestionBank;
END;
GO

CREATE VIEW dbo.vw314_HskReadingQuestionBank
AS
SELECT
    q.Id AS QuestionId,
    q.LevelCode,
    q.SectionCode,
    q.PartCode,
    q.QuestionType,
    q.DisplayText,
    q.GroupId,
    g.GroupType,
    g.GroupCode,
    g.Title AS GroupTitle,
    g.InstructionText,
    g.SharedPassage,
    g.SharedOptionPool,
    g.SourceQuestionFrom,
    g.SourceQuestionTo,
    q.SourceQuestionNo,
    q.SourceQuestionSubNo,
    q.QuestionCode,
    q.DifficultyWeight,
    q.TopicTag,
    q.UsageCount,
    q.LastUsedDate,
    p.PaperCode,
    p.SourceFileName
FROM dbo.dt314_HskQuestions q
LEFT JOIN dbo.dt314_HskQuestionGroup g ON g.Id = q.GroupId
LEFT JOIN dbo.dt314_HskSourcePaper p ON p.Id = q.SourcePaperId
WHERE q.IsActive = 1
  AND q.SectionCode = 'Reading';
GO

IF OBJECT_ID('dbo.sp314_HskPickReadingGroups', 'P') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.sp314_HskPickReadingGroups;
END;
GO

CREATE PROCEDURE dbo.sp314_HskPickReadingGroups
    @Hsk4Target INT,
    @Hsk5Target INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH GroupPool AS
    (
        SELECT
            g.Id,
            g.LevelCode,
            g.PartCode,
            g.GroupType,
            g.QuestionCount,
            g.RandomWeight,
            g.SourceQuestionFrom,
            g.SourceQuestionTo,
            ROW_NUMBER() OVER
            (
                PARTITION BY g.LevelCode
                ORDER BY ISNULL(MAX(q.LastUsedDate), '19000101'), NEWID()
            ) AS PickRank
        FROM dbo.dt314_HskQuestionGroup g
        INNER JOIN dbo.dt314_HskQuestions q ON q.GroupId = g.Id AND q.IsActive = 1
        WHERE g.IsActive = 1
          AND g.SectionCode = 'Reading'
          AND g.RandomAsUnit = 1
        GROUP BY
            g.Id,
            g.LevelCode,
            g.PartCode,
            g.GroupType,
            g.QuestionCount,
            g.RandomWeight,
            g.SourceQuestionFrom,
            g.SourceQuestionTo
    )
    SELECT *
    FROM GroupPool
    WHERE (LevelCode = 'HSK4' AND PickRank <= @Hsk4Target)
       OR (LevelCode = 'HSK5' AND PickRank <= @Hsk5Target)
    ORDER BY LevelCode, PickRank;
END;
GO
