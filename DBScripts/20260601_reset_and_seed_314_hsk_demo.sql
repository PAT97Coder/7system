SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   314 HSK demo reset + seed

   Prerequisites:
   1. Run 20260514_add_314_chinese_hsk_exam.sql
   2. Run 20260601_add_314_hsk_reading_group_support.sql
   3. Run 20260601_adjust_314_hsk_group_types.sql

   Demo scope:
   - Reading only
   - HSK4: 10 questions
     * 46-50 SharedWordBank
     * 56-60 SentenceOrder
   - HSK5: 10 questions
     * 61-66 SingleQuestion
     * 71-74 SharedPassage

   Suggested exam setup for first run:
   - ReadingCount = 20
   - WritingCount = 0
--------------------------------------------------------------------------- */

DECLARE @SeedPrefix NVARCHAR(100) = N'DEMO_314_HSK_20260601';

IF OBJECT_ID('dbo.dt314_HskQuestionGroup', 'U') IS NULL
   OR COL_LENGTH('dbo.dt314_HskQuestions', 'GroupId') IS NULL
BEGIN
    RAISERROR(N'Please run the 20260601 group-support scripts first.', 16, 1);
    RETURN;
END;

/* ---------------------------------------------------------------------------
   1. Clean 314 data
--------------------------------------------------------------------------- */

DELETE FROM dbo.dt314_HskExamUser;
DELETE FROM dbo.dt314_HskExamQuestion;
DELETE FROM dbo.dt314_HskExamMgmt;

DELETE ans
FROM dbo.dt314_HskAnswers ans
INNER JOIN dbo.dt314_HskQuestions q ON q.Id = ans.QuesId;

DELETE FROM dbo.dt314_HskQuestions;
DELETE FROM dbo.dt314_HskQuestionGroup;

IF OBJECT_ID('dbo.dt314_HskSourcePaper', 'U') IS NOT NULL
BEGIN
    DELETE FROM dbo.dt314_HskSourcePaper;
END;

DBCC CHECKIDENT ('dbo.dt314_HskExamUser', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskExamQuestion', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskExamMgmt', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskAnswers', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskQuestions', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskQuestionGroup', RESEED, 0) WITH NO_INFOMSGS;

IF OBJECT_ID('dbo.dt314_HskSourcePaper', 'U') IS NOT NULL
BEGIN
    DBCC CHECKIDENT ('dbo.dt314_HskSourcePaper', RESEED, 0) WITH NO_INFOMSGS;
END;

/* ---------------------------------------------------------------------------
   2. Seed groups and questions
--------------------------------------------------------------------------- */

DECLARE @GroupId INT;
DECLARE @QuestionId INT;

/* ========================= HSK4 / SharedWordBank / 46-50 ========================= */
INSERT INTO dbo.dt314_HskQuestionGroup
(
    LevelCode,
    SectionCode,
    PartCode,
    GroupType,
    GroupCode,
    Title,
    InstructionText,
    SharedPassage,
    SharedOptionPool,
    SourceQuestionFrom,
    SourceQuestionTo,
    QuestionCount,
    RandomAsUnit,
    RandomWeight,
    IsActive,
    Remark,
    CreatedDate
)
VALUES
(
    'HSK4',
    'Reading',
    'ReadingPart1',
    'SharedWordBank',
    'HSK4_R_46_50',
    N'HSK4 Reading 46-50',
    N'第 46-50 题：选词填空。',
    NULL,
    N'A 重  B 首先  C 观众  D 坚持  E 擦  F 地点',
    46,
    50,
    5,
    1,
    1,
    1,
    @SeedPrefix + N'|GROUP|HSK4|46-50',
    GETDATE()
);
SET @GroupId = SCOPE_IDENTITY();

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, ImageName, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SingleChoice', N'爷爷，为什么橡皮能（ ）掉铅笔写的字？', NULL, 0, 1, GETDATE(), @SeedPrefix + N'|Q|46', @GroupId, 'ReadingPart1', 46, 'HSK4_Q46', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'重', 0, 1, 1), (@QuestionId, N'首先', 0, 2, 1), (@QuestionId, N'观众', 0, 3, 1),
(@QuestionId, N'坚持', 0, 4, 1), (@QuestionId, N'擦', 1, 5, 1), (@QuestionId, N'地点', 0, 6, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, ImageName, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SingleChoice', N'这部电影非常感人，很多（ ）都被感动得哭了。', NULL, 0, 1, GETDATE(), @SeedPrefix + N'|Q|47', @GroupId, 'ReadingPart1', 47, 'HSK4_Q47', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'重', 0, 1, 1), (@QuestionId, N'首先', 0, 2, 1), (@QuestionId, N'观众', 1, 3, 1),
(@QuestionId, N'坚持', 0, 4, 1), (@QuestionId, N'擦', 0, 5, 1), (@QuestionId, N'地点', 0, 6, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, ImageName, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SingleChoice', N'不管别人怎么说，（ ）你要对自己有信心才行。', NULL, 0, 1, GETDATE(), @SeedPrefix + N'|Q|48', @GroupId, 'ReadingPart1', 48, 'HSK4_Q48', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'重', 0, 1, 1), (@QuestionId, N'首先', 1, 2, 1), (@QuestionId, N'观众', 0, 3, 1),
(@QuestionId, N'坚持', 0, 4, 1), (@QuestionId, N'擦', 0, 5, 1), (@QuestionId, N'地点', 0, 6, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, ImageName, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SingleChoice', N'这次聚会的（ ）是小李选的，时间也是他定的。', NULL, 0, 1, GETDATE(), @SeedPrefix + N'|Q|49', @GroupId, 'ReadingPart1', 49, 'HSK4_Q49', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'重', 0, 1, 1), (@QuestionId, N'首先', 0, 2, 1), (@QuestionId, N'观众', 0, 3, 1),
(@QuestionId, N'坚持', 0, 4, 1), (@QuestionId, N'擦', 0, 5, 1), (@QuestionId, N'地点', 1, 6, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, ImageName, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SingleChoice', N'谢谢，不用了，这个行李箱一点儿都不（ ），里面都是衣服。', NULL, 0, 1, GETDATE(), @SeedPrefix + N'|Q|50', @GroupId, 'ReadingPart1', 50, 'HSK4_Q50', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'重', 1, 1, 1), (@QuestionId, N'首先', 0, 2, 1), (@QuestionId, N'观众', 0, 3, 1),
(@QuestionId, N'坚持', 0, 4, 1), (@QuestionId, N'擦', 0, 5, 1), (@QuestionId, N'地点', 0, 6, 1);

/* ========================= HSK4 / SentenceOrder / 56-60 ========================= */
INSERT INTO dbo.dt314_HskQuestionGroup
(
    LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText,
    SharedPassage, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount,
    RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate
)
VALUES
(
    'HSK4', 'Reading', 'ReadingPart2', 'SentenceOrder', 'HSK4_R_56_60',
    N'HSK4 Reading 56-60', N'第 56-60 题：排列顺序。', NULL, NULL,
    56, 60, 5, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK4|56-60', GETDATE()
);
SET @GroupId = SCOPE_IDENTITY();

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 意思是希望朋友之间的友好关系  B 能够一直继续下去，越久越好  C 人们常说“友谊地久天长”', 0, 1, GETDATE(), @SeedPrefix + N'|Q|56', @GroupId, 'ReadingPart2', 56, 'HSK4_Q56', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'C-A-B', 1, 1, 1), (@QuestionId, N'A-B-C', 0, 2, 1), (@QuestionId, N'B-C-A', 0, 3, 1), (@QuestionId, N'C-B-A', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 只要找出文中的关键信息  B 就可以在短时间内了解文章的大意  C 做到快速阅读其实不难，简单来说', 0, 1, GETDATE(), @SeedPrefix + N'|Q|57', @GroupId, 'ReadingPart2', 57, 'HSK4_Q57', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'C-A-B', 1, 1, 1), (@QuestionId, N'A-C-B', 0, 2, 1), (@QuestionId, N'B-A-C', 0, 3, 1), (@QuestionId, N'C-B-A', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 请不要在园区内抽烟，谢谢  B 欢迎大家来到国家森林公园  C 为了保证您和他人的安全', 0, 1, GETDATE(), @SeedPrefix + N'|Q|58', @GroupId, 'ReadingPart2', 58, 'HSK4_Q58', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'B-C-A', 1, 1, 1), (@QuestionId, N'C-A-B', 0, 2, 1), (@QuestionId, N'A-B-C', 0, 3, 1), (@QuestionId, N'B-A-C', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 如果你不能勇敢地走出第一步  B 所以，千万不要因为害怕失败而不敢开始  C 就永远没有机会获得成功', 0, 1, GETDATE(), @SeedPrefix + N'|Q|59', @GroupId, 'ReadingPart2', 59, 'HSK4_Q59', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'A-C-B', 1, 1, 1), (@QuestionId, N'C-A-B', 0, 2, 1), (@QuestionId, N'B-A-C', 0, 3, 1), (@QuestionId, N'A-B-C', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 不过这里以前比较安静  B 我对这里当然熟悉了，我家原来就住这儿附近  C 不像现在这么热闹', 0, 1, GETDATE(), @SeedPrefix + N'|Q|60', @GroupId, 'ReadingPart2', 60, 'HSK4_Q60', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'B-A-C', 1, 1, 1), (@QuestionId, N'C-B-A', 0, 2, 1), (@QuestionId, N'A-C-B', 0, 3, 1), (@QuestionId, N'B-C-A', 0, 4, 1);

/* ========================= HSK5 / SingleQuestion / 61-66 ========================= */
DECLARE @SingleGroupCode VARCHAR(50);

/* 61 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_61', N'HSK5 Reading 61', N'第 61 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|61', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'拿着尺子上街，只量别人不量自己是行不通的。生活的多样性、复杂性要求我们必须接受不同的性格、不同的思想。所有这些不同的东西需要我们有一颗包容的心，而不是拿着自己的标准去要求别人。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|61', @GroupId, 'ReadingPart2', 61, 'HSK5_Q61', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'要尊重个性', 1, 1, 1), (@QuestionId, N'人生充满挑战', 0, 2, 1), (@QuestionId, N'对自己要严格要求', 0, 3, 1), (@QuestionId, N'我们总会有相同的地方', 0, 4, 1);

/* 62 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_62', N'HSK5 Reading 62', N'第 62 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|62', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'这是一本十分有趣的书，书中讲了 12 个关于胆小鬼的故事。它希望让孩子明白一个道理：要想干成事情，首先就得克服胆子小的毛病。为了给孩子们的阅读带来更大的乐趣和方便，书中还配有大量插图和汉语拼音。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|62', @GroupId, 'ReadingPart2', 62, 'HSK5_Q62', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'这本书配有光盘', 0, 1, 1), (@QuestionId, N'作者小时候胆子很小', 0, 2, 1), (@QuestionId, N'这本书的读者是孩子', 1, 3, 1), (@QuestionId, N'这本书里有 12 个人物', 0, 4, 1);

/* 63 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_63', N'HSK5 Reading 63', N'第 63 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|63', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'说到健康食品，大家通常都会想到蔬菜、水果，而把肉类看做健康的敌人。其实，很多肉类对人体健康有很重要的作用。至今，很多国家并没有规定什么才是健康食品。因此，现在市场上所谓的健康食品其实没有统一的标准。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|63', @GroupId, 'ReadingPart2', 63, 'HSK5_Q63', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'饮食要规律', 0, 1, 1), (@QuestionId, N'肉类不是健康食品', 0, 2, 1), (@QuestionId, N'蔬菜水果营养成分少', 0, 3, 1), (@QuestionId, N'健康食品没有统一标准', 1, 4, 1);

/* 64 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_64', N'HSK5 Reading 64', N'第 64 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|64', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'冬天是一年中最寒冷的季节，很多植物没有了绿叶，一些动物会选择休眠，许多鸟儿飞到较为温暖的地方过冬。这个世界仿佛一下子安静下来了，然而，这所有的一切都是在为明年做打算。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|64', @GroupId, 'ReadingPart2', 64, 'HSK5_Q64', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'冬季有很多节日', 0, 1, 1), (@QuestionId, N'人们在冬天都很忙', 0, 2, 1), (@QuestionId, N'冬天是一年中最长的季节', 0, 3, 1), (@QuestionId, N'冬天是为来年做准备的季节', 1, 4, 1);

/* 65 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_65', N'HSK5 Reading 65', N'第 65 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|65', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'优秀的员工奉行这样的理念：不找借口找办法，办法总比问题多。这是一个充满自信的理念，也是一个更具建设性、创造性的理念。世上少有解决不了的问题，只有不会解决问题的人。问题只要被发现了，在认真分析清楚后，一般总能找到相应的解决办法。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|65', @GroupId, 'ReadingPart2', 65, 'HSK5_Q65', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'生活中需要借口', 0, 1, 1), (@QuestionId, N'发现问题的能力很重要', 0, 2, 1), (@QuestionId, N'总会有解决问题的办法', 1, 3, 1), (@QuestionId, N'优秀员工常会提出许多问题', 0, 4, 1);

/* 66 */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', 'HSK5_R_66', N'HSK5 Reading 66', N'第 66 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|66', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'日出而作，日落而息。人们一般习惯在晚上睡觉，在黑暗中睡觉，关灯并用窗帘挡住室外照进来的光线。亮着灯睡觉会使人推迟入睡时间，而且较难进入深睡阶段。光照会提高脑的兴奋度，因而去除光照刺激，减少卧室光线，对预防失眠有很大帮助。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|66', @GroupId, 'ReadingPart2', 66, 'HSK5_Q66', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'开灯睡觉影响睡眠', 1, 1, 1), (@QuestionId, N'光照使人神经放松', 0, 2, 1), (@QuestionId, N'缺乏睡眠危害健康', 0, 3, 1), (@QuestionId, N'白天睡眠质量更高', 0, 4, 1);

/* ========================= HSK5 / SharedPassage / 71-74 ========================= */
INSERT INTO dbo.dt314_HskQuestionGroup
(
    LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText,
    SharedPassage, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount,
    RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate
)
VALUES
(
    'HSK5', 'Reading', 'ReadingPart3', 'SharedPassage', 'HSK5_R_71_74',
    N'HSK5 Reading 71-74', N'第 71-74 题：请根据短文选出正确答案。',
    N'一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑，猎狗在它后面一直追。可是追了一阵儿，兔子跑得越来越远。猎狗知道实在是追不上了，只好回到猎人身边。那个人非常生气地说：“你真没用，连一只受伤的兔子都追不到！”猎狗听了很不服气地说：“我已经尽力而为了！”那只兔子带着枪伤成功地逃回家里，同伴们都围过来惊讶地问它：“那只猎狗很凶呀，你又带了伤，是怎么甩掉它的呢？”兔子说：“它是尽力而为，我是用尽全力呀！它没追上我，最多挨一顿骂，而我若不用尽全力地跑，可就没命了！”每个人都有很大的潜能。谁要想成功，创造奇迹，仅仅做到尽力而为还远远不够，必须用尽全力才行。',
    NULL,
    71, 74, 4, 1, 1, 1, @SeedPrefix + N'|GROUP|HSK5|71-74', GETDATE()
);
SET @GroupId = SCOPE_IDENTITY();

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'兔子的腿怎么了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|71', @GroupId, 'ReadingPart3', 71, 'HSK5_Q71', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'摔断了', 0, 1, 1), (@QuestionId, N'被砍伤了', 0, 2, 1), (@QuestionId, N'被枪打中了', 1, 3, 1), (@QuestionId, N'被猎狗咬伤了', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'猎狗为什么被主人骂了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|72', @GroupId, 'ReadingPart3', 72, 'HSK5_Q72', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'没找到猎物', 0, 1, 1), (@QuestionId, N'没有追到兔子', 1, 2, 1), (@QuestionId, N'把兔子咬死了', 0, 3, 1), (@QuestionId, N'偷偷放走了兔子', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'兔子最后怎么了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|73', @GroupId, 'ReadingPart3', 73, 'HSK5_Q73', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'逃跑了', 1, 1, 1), (@QuestionId, N'捉住了猎狗', 0, 2, 1), (@QuestionId, N'被同伴救了', 0, 3, 1), (@QuestionId, N'被猎人捉住了', 0, 4, 1);

INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
VALUES
('HSK5', 'Reading', 'SingleChoice', N'这个故事说明了什么道理？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|74', @GroupId, 'ReadingPart3', 74, 'HSK5_Q74', 1, 0);
SET @QuestionId = SCOPE_IDENTITY();
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive) VALUES
(@QuestionId, N'时间就是生命', 0, 1, 1), (@QuestionId, N'要敢于承认错误', 0, 2, 1), (@QuestionId, N'尽全力才能成功', 1, 3, 1), (@QuestionId, N'做事要有合作精神', 0, 4, 1);

/* ---------------------------------------------------------------------------
   3. Summary
--------------------------------------------------------------------------- */

SELECT
    q.LevelCode,
    q.SectionCode,
    ISNULL(q.PartCode, '') AS PartCode,
    COUNT(*) AS QuestionCount
FROM dbo.dt314_HskQuestions q
GROUP BY
    q.LevelCode,
    q.SectionCode,
    q.PartCode
ORDER BY
    q.LevelCode,
    q.SectionCode,
    q.PartCode;

SELECT
    g.LevelCode,
    g.GroupType,
    COUNT(*) AS GroupCount,
    SUM(g.QuestionCount) AS TotalQuestions
FROM dbo.dt314_HskQuestionGroup g
GROUP BY
    g.LevelCode,
    g.GroupType
ORDER BY
    g.LevelCode,
    g.GroupType;
