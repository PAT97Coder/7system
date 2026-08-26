SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   Legacy seed data for 314 Chinese HSK question bank
   - No images
   - This script is kept for reference only.
   - Use 20260601_reset_and_seed_314_hsk_reading_80.sql or
     20260601_reset_and_seed_314_hsk_demo.sql for the reading-only model.
   - Idempotent by Remark prefix: SEED_314_HSK_20260514
--------------------------------------------------------------------------- */

DECLARE @SeedPrefix NVARCHAR(100) = N'SEED_314_HSK_20260514';

IF OBJECT_ID('dbo.dt314_HskAnswers', 'U') IS NULL
   OR OBJECT_ID('dbo.dt314_HskQuestions', 'U') IS NULL
BEGIN
    RAISERROR(N'Please run 20260514_add_314_chinese_hsk_exam.sql first.', 16, 1);
    RETURN;
END;

RAISERROR(N'This legacy mixed-bank seed is not compatible with the reading-only model. Use the 20260601 reading-only seed scripts instead.', 16, 1);
RETURN;

DELETE ans
FROM dbo.dt314_HskAnswers ans
INNER JOIN dbo.dt314_HskQuestions ques ON ques.Id = ans.QuesId
WHERE ques.Remark LIKE @SeedPrefix + N'%';

DELETE FROM dbo.dt314_HskQuestions
WHERE Remark LIKE @SeedPrefix + N'%';

DECLARE @QuestionPlan TABLE
(
    Id INT IDENTITY(1, 1) PRIMARY KEY,
    LevelCode VARCHAR(10) NOT NULL,
    SectionCode VARCHAR(20) NOT NULL,
    QuestionType VARCHAR(40) NOT NULL,
    QuestionNo INT NOT NULL
);

DECLARE @Groups TABLE
(
    LevelCode VARCHAR(10) NOT NULL,
    SectionCode VARCHAR(20) NOT NULL,
    TotalCount INT NOT NULL
);

INSERT INTO @Groups (LevelCode, SectionCode, TotalCount)
VALUES
    ('HSK4', 'Reading', 40),
    ('HSK5', 'Reading', 40),
    ('HSK4', 'Writing', 10),
    ('HSK5', 'Writing', 10);

DECLARE @Types TABLE
(
    Seq INT NOT NULL PRIMARY KEY,
    QuestionType VARCHAR(40) NOT NULL
);

INSERT INTO @Types (Seq, QuestionType)
VALUES
    (1, 'SingleChoice'),
    (2, 'MultiChoice'),
    (3, 'SentenceOrder'),
    (4, 'PictureChoice'),
    (5, 'DescriptionChoice');

DECLARE @LevelCode VARCHAR(10);
DECLARE @SectionCode VARCHAR(20);
DECLARE @TotalCount INT;
DECLARE @Counter INT;

DECLARE group_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT LevelCode, SectionCode, TotalCount
    FROM @Groups;

OPEN group_cursor;
FETCH NEXT FROM group_cursor INTO @LevelCode, @SectionCode, @TotalCount;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Counter = 1;

    WHILE @Counter <= @TotalCount
    BEGIN
        INSERT INTO @QuestionPlan (LevelCode, SectionCode, QuestionType, QuestionNo)
        SELECT
            @LevelCode,
            @SectionCode,
            QuestionType,
            @Counter
        FROM @Types
        WHERE Seq = ((@Counter - 1) % 5) + 1;

        SET @Counter += 1;
    END;

    FETCH NEXT FROM group_cursor INTO @LevelCode, @SectionCode, @TotalCount;
END;

CLOSE group_cursor;
DEALLOCATE group_cursor;

DECLARE @PlanId INT;
DECLARE @QuestionType VARCHAR(40);
DECLARE @QuestionNo INT;
DECLARE @QuestionId INT;
DECLARE @QuestionText NVARCHAR(MAX);
DECLARE @Remark NVARCHAR(500);
DECLARE @IsMultiAns BIT;

DECLARE question_cursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Id, LevelCode, SectionCode, QuestionType, QuestionNo
    FROM @QuestionPlan
    ORDER BY
        CASE LevelCode WHEN 'HSK4' THEN 1 ELSE 2 END,
        CASE SectionCode WHEN 'Reading' THEN 1 ELSE 2 END,
        QuestionNo;

OPEN question_cursor;
FETCH NEXT FROM question_cursor INTO @PlanId, @LevelCode, @SectionCode, @QuestionType, @QuestionNo;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Remark = CONCAT(@SeedPrefix, N'|', @LevelCode, N'|', @SectionCode, N'|', @QuestionNo);
    SET @IsMultiAns = CASE WHEN @QuestionType = 'MultiChoice' THEN 1 ELSE 0 END;

    SET @QuestionText =
        CASE @QuestionType
            WHEN 'SingleChoice' THEN
                CONCAT(N'[', @LevelCode, N' ', @SectionCode, N'] 第 ', @QuestionNo, N' 題：他每天都去圖書館學習。這句話主要說什麼？')
            WHEN 'MultiChoice' THEN
                CONCAT(N'[', @LevelCode, N' ', @SectionCode, N'] 第 ', @QuestionNo, N' 題：根據內容，哪些說法是正確的？')
            WHEN 'SentenceOrder' THEN
                CONCAT(N'[', @LevelCode, N' ', @SectionCode, N'] 第 ', @QuestionNo, N' 題：請選出正確的語序。')
            WHEN 'PictureChoice' THEN
                CONCAT(N'[', @LevelCode, N' ', @SectionCode, N'] 第 ', @QuestionNo, N' 題：請選出最符合圖片內容的句子。本 seed 不使用圖片，只測試題型流程。')
            ELSE
                CONCAT(N'[', @LevelCode, N' ', @SectionCode, N'] 第 ', @QuestionNo, N' 題：請選出最合適的描述。本 seed 不使用圖片，只測試題型流程。')
        END;

    INSERT INTO dbo.dt314_HskQuestions
    (
        LevelCode,
        SectionCode,
        QuestionType,
        DisplayText,
        ImageName,
        IsMultiAns,
        IsActive,
        CreatedBy,
        CreatedDate,
        Remark
    )
    VALUES
    (
        @LevelCode,
        @SectionCode,
        @QuestionType,
        @QuestionText,
        NULL,
        @IsMultiAns,
        1,
        NULL,
        GETDATE(),
        @Remark
    );

    SET @QuestionId = SCOPE_IDENTITY();

    IF @QuestionType = 'SingleChoice'
    BEGIN
        INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, ImageName, TrueAns, DisplayOrder, IsActive)
        VALUES
            (@QuestionId, N'他每天學習。', NULL, 1, 1, 1),
            (@QuestionId, N'他每天運動。', NULL, 0, 2, 1),
            (@QuestionId, N'他每天做飯。', NULL, 0, 3, 1),
            (@QuestionId, N'他每天買東西。', NULL, 0, 4, 1);
    END
    ELSE IF @QuestionType = 'MultiChoice'
    BEGIN
        INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, ImageName, TrueAns, DisplayOrder, IsActive)
        VALUES
            (@QuestionId, N'公司下個月搬辦公室。', NULL, 1, 1, 1),
            (@QuestionId, N'員工需要提前整理資料。', NULL, 1, 2, 1),
            (@QuestionId, N'會議已經取消。', NULL, 0, 3, 1),
            (@QuestionId, N'所有人明天放假。', NULL, 0, 4, 1);
    END
    ELSE IF @QuestionType = 'SentenceOrder'
    BEGIN
        INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, ImageName, TrueAns, DisplayOrder, IsActive)
        VALUES
            (@QuestionId, N'我把這本書看完了。', NULL, 1, 1, 1),
            (@QuestionId, N'我看完了把這本書。', NULL, 0, 2, 1),
            (@QuestionId, N'把我這本書看完了。', NULL, 0, 3, 1),
            (@QuestionId, N'這本書把我看完了。', NULL, 0, 4, 1);
    END
    ELSE IF @QuestionType = 'PictureChoice'
    BEGIN
        INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, ImageName, TrueAns, DisplayOrder, IsActive)
        VALUES
            (@QuestionId, N'這個人在修理自行車。', NULL, 1, 1, 1),
            (@QuestionId, N'這個人在做飯。', NULL, 0, 2, 1),
            (@QuestionId, N'這個人在買票。', NULL, 0, 3, 1),
            (@QuestionId, N'這個人在唱歌。', NULL, 0, 4, 1);
    END
    ELSE
    BEGIN
        INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, ImageName, TrueAns, DisplayOrder, IsActive)
        VALUES
            (@QuestionId, N'交通標誌', NULL, 1, 1, 1),
            (@QuestionId, N'天氣預報', NULL, 0, 2, 1),
            (@QuestionId, N'餐廳菜單', NULL, 0, 3, 1),
            (@QuestionId, N'公司通知', NULL, 0, 4, 1);
    END;

    FETCH NEXT FROM question_cursor INTO @PlanId, @LevelCode, @SectionCode, @QuestionType, @QuestionNo;
END;

CLOSE question_cursor;
DEALLOCATE question_cursor;

SELECT
    LevelCode,
    SectionCode,
    QuestionType,
    COUNT(*) AS QuestionCount
FROM dbo.dt314_HskQuestions
WHERE Remark LIKE @SeedPrefix + N'%'
GROUP BY LevelCode, SectionCode, QuestionType
ORDER BY LevelCode, SectionCode, QuestionType;
