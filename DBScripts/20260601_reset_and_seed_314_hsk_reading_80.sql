SET NOCOUNT ON;

/* ---------------------------------------------------------------------------
   Full reading seed for 314 mixed HSK bank

   Prerequisites:
   1. 20260514_add_314_chinese_hsk_exam.sql
   2. 20260601_add_314_hsk_reading_group_support.sql
   3. 20260601_adjust_314_hsk_group_types.sql

   Purpose:
   - reset all 314 data
   - seed enough reading questions to run ReadingCount = 80
   - HSK4 bank: 40 questions
   - HSK5 bank: 41 questions

   Suggested first run:
   - ReadingCount = 80
   - WritingCount = 0
--------------------------------------------------------------------------- */

DECLARE @SeedPrefix NVARCHAR(100) = N'FULL_314_HSK_20260601';

IF OBJECT_ID('dbo.dt314_HskQuestionGroup', 'U') IS NULL
   OR COL_LENGTH('dbo.dt314_HskQuestions', 'GroupId') IS NULL
BEGIN
    RAISERROR(N'Please run the 20260601 group-support scripts first.', 16, 1);
    RETURN;
END;

/* ---------------------------------------------------------------------------
   Reset 314 data
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
    DBCC CHECKIDENT ('dbo.dt314_HskSourcePaper', RESEED, 0) WITH NO_INFOMSGS;
END;

DBCC CHECKIDENT ('dbo.dt314_HskExamUser', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskExamQuestion', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskExamMgmt', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskAnswers', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskQuestions', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('dbo.dt314_HskQuestionGroup', RESEED, 0) WITH NO_INFOMSGS;

DECLARE @GroupId INT;

/* ---------------------------------------------------------------------------
   Helper pattern:
   - insert group
   - bulk insert questions with OUTPUT to @InsertedQuestions
   - bulk insert answers mapped by SourceQuestionNo
--------------------------------------------------------------------------- */

/* ========================= HSK4 ========================= */

/* HSK4 H41011 46-50 SharedWordBank */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart1', 'SharedWordBank', 'HSK4_H41011_46_50', N'HSK4 H41011 46-50', N'第 46-50 题：选词填空。', N'A 重  B 首先  C 观众  D 坚持  E 擦  F 地点', 46, 50, 5, 1, 1, 1, @SeedPrefix + N'|HSK4|H41011|46-50', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_4650 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_4650
VALUES
('HSK4', 'Reading', 'SingleChoice', N'爷爷，为什么橡皮能（ ）掉铅笔写的字？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|46', @GroupId, 'ReadingPart1', 46, 'HSK4_H41011_Q46', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'这部电影非常感人，很多（ ）都被感动得哭了。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|47', @GroupId, 'ReadingPart1', 47, 'HSK4_H41011_Q47', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'不管别人怎么说，（ ）你要对自己有信心才行。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|48', @GroupId, 'ReadingPart1', 48, 'HSK4_H41011_Q48', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'这次聚会的（ ）是小李选的，时间也是他定的。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|49', @GroupId, 'ReadingPart1', 49, 'HSK4_H41011_Q49', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'谢谢，不用了，这个行李箱一点儿都不（ ），里面都是衣服。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|50', @GroupId, 'ReadingPart1', 50, 'HSK4_H41011_Q50', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_4650 q
JOIN (VALUES
 (46, N'重',0,1),(46, N'首先',0,2),(46, N'观众',0,3),(46, N'坚持',0,4),(46, N'擦',1,5),(46, N'地点',0,6),
 (47, N'重',0,1),(47, N'首先',0,2),(47, N'观众',1,3),(47, N'坚持',0,4),(47, N'擦',0,5),(47, N'地点',0,6),
 (48, N'重',0,1),(48, N'首先',1,2),(48, N'观众',0,3),(48, N'坚持',0,4),(48, N'擦',0,5),(48, N'地点',0,6),
 (49, N'重',0,1),(49, N'首先',0,2),(49, N'观众',0,3),(49, N'坚持',0,4),(49, N'擦',0,5),(49, N'地点',1,6),
 (50, N'重',1,1),(50, N'首先',0,2),(50, N'观众',0,3),(50, N'坚持',0,4),(50, N'擦',0,5),(50, N'地点',0,6)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK4 H41011 51-55 SharedWordBank */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart1', 'SharedWordBank', 'HSK4_H41011_51_55', N'HSK4 H41011 51-55', N'第 51-55 题：选词填空。', N'A 难受  B 郊区  C 温度  D 流行  E 香  F 恐怕', 51, 55, 5, 1, 1, 1, @SeedPrefix + N'|HSK4|H41011|51-55', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_5155 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_5155
VALUES
('HSK4', 'Reading', 'SingleChoice', N'A：家里的果汁够吗？要不要再买几瓶？ B：要是明天来的人多，（ ）会不够，再买两瓶吧。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|51', @GroupId, 'ReadingPart1', 51, 'HSK4_H41011_Q51', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：听说公司明年要搬到（ ） ，到时候我又得重新租房子了。 B：这个消息准确吗？我怎么不知道？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|52', @GroupId, 'ReadingPart1', 52, 'HSK4_H41011_Q52', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：你稍微开慢点儿，我有点儿（ ）。 B：你怎么了？实在不行，我们在路边停下来休息会儿。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|53', @GroupId, 'ReadingPart1', 53, 'HSK4_H41011_Q53', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：面条儿做好了，快来吃吧。 B：真（ ）啊，我最喜欢吃你做的西红柿鸡蛋面了。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|54', @GroupId, 'ReadingPart1', 54, 'HSK4_H41011_Q54', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：姐，你觉得这条蓝色的裙子怎么样？ B：挺好的，今年（ ）蓝色，而且夏天穿这样的裙子也凉快。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|55', @GroupId, 'ReadingPart1', 55, 'HSK4_H41011_Q55', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_5155 q
JOIN (VALUES
 (51, N'难受',0,1),(51, N'郊区',0,2),(51, N'温度',0,3),(51, N'流行',0,4),(51, N'香',0,5),(51, N'恐怕',1,6),
 (52, N'难受',0,1),(52, N'郊区',1,2),(52, N'温度',0,3),(52, N'流行',0,4),(52, N'香',0,5),(52, N'恐怕',0,6),
 (53, N'难受',1,1),(53, N'郊区',0,2),(53, N'温度',0,3),(53, N'流行',0,4),(53, N'香',0,5),(53, N'恐怕',0,6),
 (54, N'难受',0,1),(54, N'郊区',0,2),(54, N'温度',0,3),(54, N'流行',0,4),(54, N'香',1,5),(54, N'恐怕',0,6),
 (55, N'难受',0,1),(55, N'郊区',0,2),(55, N'温度',0,3),(55, N'流行',1,4),(55, N'香',0,5),(55, N'恐怕',0,6)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK4 H41010 46-50 SharedWordBank */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart1', 'SharedWordBank', 'HSK4_H41010_46_50', N'HSK4 H41010 46-50', N'第 46-50 题：选词填空。', N'A 世纪  B 引起  C 辛苦  D 坚持  E 味道  F 份', 46, 50, 5, 1, 1, 1, @SeedPrefix + N'|HSK4|H41010|46-50', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_B4650 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_B4650
VALUES
('HSK4', 'Reading', 'SingleChoice', N'先生，请您先在入口处填一（ ）表格。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B46', @GroupId, 'ReadingPart1', 46, 'HSK4_H41010_Q46', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'最近 10 年这个省经济增长很快，（ ）了很多人的关注。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B47', @GroupId, 'ReadingPart1', 47, 'HSK4_H41010_Q47', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'生活中少了幽默，就好像菜里忘了加盐，总让人感觉少了些（ ）。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B48', @GroupId, 'ReadingPart1', 48, 'HSK4_H41010_Q48', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'今天的演出非常精彩，大家都（ ）了，早点儿回去休息吧。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B49', @GroupId, 'ReadingPart1', 49, 'HSK4_H41010_Q49', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'我家门前的那条马路是 1920 年修的，到现在都快一个（ ）了。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B50', @GroupId, 'ReadingPart1', 50, 'HSK4_H41010_Q50', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_B4650 q
JOIN (VALUES
 (46, N'世纪',0,1),(46, N'引起',0,2),(46, N'辛苦',0,3),(46, N'坚持',0,4),(46, N'味道',0,5),(46, N'份',1,6),
 (47, N'世纪',0,1),(47, N'引起',1,2),(47, N'辛苦',0,3),(47, N'坚持',0,4),(47, N'味道',0,5),(47, N'份',0,6),
 (48, N'世纪',0,1),(48, N'引起',0,2),(48, N'辛苦',0,3),(48, N'坚持',0,4),(48, N'味道',1,5),(48, N'份',0,6),
 (49, N'世纪',0,1),(49, N'引起',0,2),(49, N'辛苦',1,3),(49, N'坚持',0,4),(49, N'味道',0,5),(49, N'份',0,6),
 (50, N'世纪',1,1),(50, N'引起',0,2),(50, N'辛苦',0,3),(50, N'坚持',0,4),(50, N'味道',0,5),(50, N'份',0,6)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK4 H41010 51-55 SharedWordBank */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedOptionPool, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart1', 'SharedWordBank', 'HSK4_H41010_51_55', N'HSK4 H41010 51-55', N'第 51-55 题：选词填空。', N'A 尊重  B 乱  C 温度  D 演员  E 估计  F 倒', 51, 55, 5, 1, 1, 1, @SeedPrefix + N'|HSK4|H41010|51-55', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_B5155 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_B5155
VALUES
('HSK4', 'Reading', 'SingleChoice', N'A：听说这次活动会邀请许多著名（ ），是真的吗？ B：我也不太清楚，这次活动是小张负责的，你可以去问问他。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B51', @GroupId, 'ReadingPart1', 51, 'HSK4_H41010_Q51', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：你的头发长了，看上去有些（ ）。 B：是啊，我正准备下午去理发呢。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B52', @GroupId, 'ReadingPart1', 52, 'HSK4_H41010_Q52', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：不好意思，会议推迟了，我（ ）4 点多才能跟你见面。 B：没关系，我在大使馆对面的餐厅等你。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B53', @GroupId, 'ReadingPart1', 53, 'HSK4_H41010_Q53', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：我考虑了很久，还是决定离开现在的公司。 B：既然这样，那我们（ ）你的选择。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B54', @GroupId, 'ReadingPart1', 54, 'HSK4_H41010_Q54', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'A：桌子上怎么这么多水？ B：不好意思，我刚才不小心把杯子弄（ ）了，还没来得及擦。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|B55', @GroupId, 'ReadingPart1', 55, 'HSK4_H41010_Q55', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_B5155 q
JOIN (VALUES
 (51, N'尊重',0,1),(51, N'乱',0,2),(51, N'温度',0,3),(51, N'演员',1,4),(51, N'估计',0,5),(51, N'倒',0,6),
 (52, N'尊重',0,1),(52, N'乱',1,2),(52, N'温度',0,3),(52, N'演员',0,4),(52, N'估计',0,5),(52, N'倒',0,6),
 (53, N'尊重',0,1),(53, N'乱',0,2),(53, N'温度',0,3),(53, N'演员',0,4),(53, N'估计',1,5),(53, N'倒',0,6),
 (54, N'尊重',1,1),(54, N'乱',0,2),(54, N'温度',0,3),(54, N'演员',0,4),(54, N'估计',0,5),(54, N'倒',0,6),
 (55, N'尊重',0,1),(55, N'乱',0,2),(55, N'温度',0,3),(55, N'演员',0,4),(55, N'估计',0,5),(55, N'倒',1,6)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK4 H41011 56-65 SentenceOrder */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart2', 'SentenceOrder', 'HSK4_H41011_56_65', N'HSK4 H41011 56-65', N'第 56-65 题：排列顺序。', 56, 65, 10, 1, 1, 1, @SeedPrefix + N'|HSK4|H41011|56-65', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_5665 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_5665
VALUES
('HSK4', 'Reading', 'SentenceOrder', N'A 意思是希望朋友之间的友好关系  B 能够一直继续下去，越久越好  C 人们常说“友谊地久天长”', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|56', @GroupId, 'ReadingPart2', 56, 'HSK4_H41011_Q56', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 只要找出文中的关键信息  B 就可以在短时间内了解文章的大意  C 做到快速阅读其实不难，简单来说', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|57', @GroupId, 'ReadingPart2', 57, 'HSK4_H41011_Q57', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 请不要在园区内抽烟，谢谢  B 欢迎大家来到国家森林公园  C 为了保证您和他人的安全', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|58', @GroupId, 'ReadingPart2', 58, 'HSK4_H41011_Q58', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 如果你不能勇敢地走出第一步  B 所以，千万不要因为害怕失败而不敢开始  C 就永远没有机会获得成功', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|59', @GroupId, 'ReadingPart2', 59, 'HSK4_H41011_Q59', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 不过这里以前比较安静  B 我对这里当然熟悉了，我家原来就住这儿附近  C 不像现在这么热闹', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|60', @GroupId, 'ReadingPart2', 60, 'HSK4_H41011_Q60', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 就是有时语法上会有点儿小错误  B 他的中文说得很流利  C 但我们交流起来完全没问题', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|61', @GroupId, 'ReadingPart2', 61, 'HSK4_H41011_Q61', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 这样到时候你才不会手忙脚乱  B 无论干什么事情  C 最好都能提前做好计划', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|62', @GroupId, 'ReadingPart2', 62, 'HSK4_H41011_Q62', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 意思是希望孩子能健健康康地长大  B “虎头鞋”因其前半部分像老虎头而得名  C 有些地方父母会给一岁左右的孩子穿上这种鞋', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|63', @GroupId, 'ReadingPart2', 63, 'HSK4_H41011_Q63', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 4 年的留学生活很快就要结束了  B 相信这些都会成为我日后的美好回忆  C 我在这里经历了很多，也学到了很多', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|64', @GroupId, 'ReadingPart2', 64, 'HSK4_H41011_Q64', 1, 0),
('HSK4', 'Reading', 'SentenceOrder', N'A 我们还是把它推到里面去吧  B 把这个地方空出来  C 沙发太大了，放这儿容易堵着门，进出不方便', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|65', @GroupId, 'ReadingPart2', 65, 'HSK4_H41011_Q65', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_5665 q
JOIN (VALUES
 (56, N'C-A-B',1,1),(56, N'A-B-C',0,2),(56, N'B-C-A',0,3),(56, N'C-B-A',0,4),
 (57, N'C-A-B',1,1),(57, N'A-C-B',0,2),(57, N'B-A-C',0,3),(57, N'C-B-A',0,4),
 (58, N'B-C-A',1,1),(58, N'C-A-B',0,2),(58, N'A-B-C',0,3),(58, N'B-A-C',0,4),
 (59, N'A-C-B',1,1),(59, N'C-A-B',0,2),(59, N'B-A-C',0,3),(59, N'A-B-C',0,4),
 (60, N'B-A-C',1,1),(60, N'C-B-A',0,2),(60, N'A-C-B',0,3),(60, N'B-C-A',0,4),
 (61, N'B-A-C',1,1),(61, N'A-B-C',0,2),(61, N'C-B-A',0,3),(61, N'B-C-A',0,4),
 (62, N'B-C-A',1,1),(62, N'C-A-B',0,2),(62, N'A-B-C',0,3),(62, N'C-B-A',0,4),
 (63, N'B-C-A',1,1),(63, N'A-B-C',0,2),(63, N'C-A-B',0,3),(63, N'B-A-C',0,4),
 (64, N'A-C-B',1,1),(64, N'B-A-C',0,2),(64, N'C-B-A',0,3),(64, N'A-B-C',0,4),
 (65, N'C-B-A',1,1),(65, N'B-A-C',0,2),(65, N'A-C-B',0,3),(65, N'C-A-B',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK4 H41011 66-75 SingleQuestion */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK4', 'Reading', 'ReadingPart3', 'SingleQuestion', 'HSK4_H41011_66_75', N'HSK4 H41011 66-75', N'第 66-75 题：请选出正确答案。', 10, 1, 1, 1, @SeedPrefix + N'|HSK4|H41011|66-75', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK4_6675 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK4_6675
VALUES
('HSK4', 'Reading', 'SingleChoice', N'各位乘客，大家好，感谢大家乘坐此次航班，我们的飞机将于 20 分钟后降落在北京首都国际机场。飞机：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|66', @GroupId, 'ReadingPart3', 66, 'HSK4_H41011_Q66', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'上午来应聘的那个小伙子是学电子技术的，成绩很优秀，通过面试时和他的对话，感觉他的性格也不错，我觉得他挺适合这份工作的。他觉得那个小伙子怎么样？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|67', @GroupId, 'ReadingPart3', 67, 'HSK4_H41011_Q67', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'生活中有这样两种人：一种总是看别人怎么生活，另一种喜欢生活给别人看。其实，每个人有每个人的生活，不用羡慕他人，也用不着向别人证明什么，只要用心走好自己的路，幸福就在前方。根据这段话，我们应该：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|68', @GroupId, 'ReadingPart3', 68, 'HSK4_H41011_Q68', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'除了正式的名字，中国人一般都有个小名。往往孩子还没出生，父母就已经起好了小名。小名一般都比较好听好记，而且多数是两个相同的字，例如“乐乐”“笑笑”“聪聪”等。小名往往：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|69', @GroupId, 'ReadingPart3', 69, 'HSK4_H41011_Q69', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'我叫张远，今天上午在图书馆丢了一张饭卡，卡上有我的姓名和学号。如果有同学看见了我的饭卡，请速与我联系，非常感谢。他写这段话的目的是：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|70', @GroupId, 'ReadingPart3', 70, 'HSK4_H41011_Q70', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'时间是无价的，一个人再怎么有钱，也买不到时间。知识忘了可以重新学，钱花光了可以再赚，可是时间过去了就永远回不来了。这段话主要想告诉我们：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|71', @GroupId, 'ReadingPart3', 71, 'HSK4_H41011_Q71', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'很多网站上都说，刷牙时在牙膏上加点儿盐，坚持一段时间，就能使牙变白。我打算试试，看看这个方法究竟有没有效。“这个方法”指的是：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|72', @GroupId, 'ReadingPart3', 72, 'HSK4_H41011_Q72', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'山东省烟台市是中国著名的“苹果之都”。由于气候等自然条件较好，那儿的苹果个儿大，味道香甜，颜色也漂亮，吸引了很多人前去购买。烟台：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|73', @GroupId, 'ReadingPart3', 73, 'HSK4_H41011_Q73', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'很多人习惯在早上锻炼身体，但室外锻炼并不是越早越好，尤其是冬天，日出前温度较低，并不适合运动。医生建议：冬季锻炼最好选在日出后，而且运动量不要太大。冬季锻炼最好：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|74', @GroupId, 'ReadingPart3', 74, 'HSK4_H41011_Q74', 1, 0),
('HSK4', 'Reading', 'SingleChoice', N'这是本介绍最新科学发现和研究的杂志，它的语言简单易懂，而且十分幽默。像我这种对科学完全不感兴趣的人，读起来竟然也会觉得很有趣。那本杂志：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK4|75', @GroupId, 'ReadingPart3', 75, 'HSK4_H41011_Q75', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK4_6675 q
JOIN (VALUES
 (66, N'晚点了',0,1),(66, N'要降落了',1,2),(66, N'由北京出发',0,3),(66, N'刚起飞不久',0,4),
 (67, N'很帅',0,1),(67, N'不诚实',0,2),(67, N'性格好',1,3),(67, N'能力一般',0,4),
 (68, N'学会拒绝',0,1),(68, N'少发脾气',0,2),(68, N'保护自己',0,3),(68, N'过好自己的生活',1,4),
 (69, N'比较好记',1,1),(69, N'都很浪漫',0,2),(69, N'不受重视',0,3),(69, N'是一种玩笑',0,4),
 (70, N'道歉',0,1),(70, N'找回饭卡',1,2),(70, N'通知朋友',0,3),(70, N'申请奖学金',0,4),
 (71, N'要勇敢',0,1),(71, N'知识很重要',0,2),(71, N'要管理好钱',0,3),(71, N'不要浪费时间',1,4),
 (72, N'吃 7 分饱',0,1),(72, N'自备塑料袋',0,2),(72, N'皮肤增白法',0,3),(72, N'牙膏里加盐',1,4),
 (73, N'空气差',0,1),(73, N'常下雪',0,2),(73, N'苹果很有名',1,3),(73, N'到处是葡萄树',0,4),
 (74, N'在室内',0,1),(74, N'穿厚点儿',0,2),(74, N'日出后进行',1,3),(74, N'别超过半小时',0,4),
 (75, N'页数很多',0,1),(75, N'很有意思',1,2),(75, N'很难理解',0,3),(75, N'是关于艺术的',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* ========================= HSK5 ========================= */

/* HSK5 H51004 46-48 PassageCloze */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart1', 'PassageCloze', 'HSK5_H51004_46_48', N'HSK5 H51004 46-48', N'第 46-48 题：请选出正确答案。', N'有一个年轻人在一家公司做得很出色，他为自己设计了一个美好的未来，对 46 充满信心。然而这家公司突然因为某些原因破产了，这位青年变得很悲观，认为自己是世界上最不幸、最 47 的人。但是他的经理，一位中年人拍了拍他的肩说：“你很幸运，小伙子。”“幸运？”青年人叫道。“对，很幸运！”经理重复了一遍，他解释道：“凡是青年时期受过挫折的人都很幸运，因为你可以学到如何 48。现在重新开始，一点儿都不晚。”', 46, 48, 3, 1, 1, 1, @SeedPrefix + N'|HSK5|46-48', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_4648 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_4648
VALUES
('HSK5', 'Reading', 'SingleChoice', N'46．对 46 充满信心', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|46', @GroupId, 'ReadingPart1', 46, 'HSK5_H51004_Q46', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'47．最不幸、最 47 的人', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|47', @GroupId, 'ReadingPart1', 47, 'HSK5_H51004_Q47', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'48．学到如何 48', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|48', @GroupId, 'ReadingPart1', 48, 'HSK5_H51004_Q48', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_4648 q
JOIN (VALUES
 (46, N'记忆',0,1),(46, N'前途',1,2),(46, N'命运',0,3),(46, N'价值',0,4),
 (47, N'善良',0,1),(47, N'谨慎',0,2),(47, N'糟糕',0,3),(47, N'倒霉',1,4),
 (48, N'坚强',1,1),(48, N'宝贵',0,2),(48, N'明显',0,3),(48, N'熟练',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 49-52 PassageCloze */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart1', 'PassageCloze', 'HSK5_H51004_49_52', N'HSK5 H51004 49-52', N'第 49-52 题：请选出正确答案。', N'乘坐电梯时，如果电梯突然停住了，也没有其他人发现电梯坏了，你应该怎么办？首先不要 49，确定电梯是不是真的无法正常运行。然后，立刻按红色的电梯门铃，求救铃声一响，就会有 50 的救援人员来救你。同时，也可以大声地呼救，电梯外的人有可能会听到，帮助你脱离困境。千万不要 51 激动地用力拍打电梯门，那样的话，电梯很可能会不正常地上升或下降，52。', 49, 52, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|49-52', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_4952 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_4952
VALUES
('HSK5', 'Reading', 'SingleChoice', N'49．首先不要 49', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|49', @GroupId, 'ReadingPart1', 49, 'HSK5_H51004_Q49', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'50．会有 50 的救援人员来救你', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|50', @GroupId, 'ReadingPart1', 50, 'HSK5_H51004_Q50', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'51．千万不要 51 激动地用力拍打电梯门', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|51', @GroupId, 'ReadingPart1', 51, 'HSK5_H51004_Q51', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'52．那样的话会 52', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|52', @GroupId, 'ReadingPart1', 52, 'HSK5_H51004_Q52', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_4952 q
JOIN (VALUES
 (49, N'委屈',0,1),(49, N'慌张',1,2),(49, N'沉默',0,3),(49, N'犹豫',0,4),
 (50, N'完美',0,1),(50, N'时髦',0,2),(50, N'成熟',0,3),(50, N'专业',1,4),
 (51, N'情绪',1,1),(51, N'心理',0,2),(51, N'逻辑',0,3),(51, N'思想',0,4),
 (52, N'改变危险的状况',0,1),(52, N'威胁到他人安全',0,2),(52, N'造成不必要的危险',1,3),(52, N'直到引起人们的注意',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 53-56 PassageCloze */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart1', 'PassageCloze', 'HSK5_H51004_53_56', N'HSK5 H51004 53-56', N'第 53-56 题：请选出正确答案。', N'一位教育家曾这样讲过：“孩子需要鼓励，就如植物需要浇水一样。离开鼓励，孩子就不能生存。”周宏是一位普通的技术员，但是他非常懂得怎样鼓励别人。他女儿小时候特别不喜欢数学，53。有一天，周宏给女儿出了 10 道题，结果女儿竟然做错了 9 道。周宏并没有生气，而是对女儿大加 54。第二天晚上，周宏 55 准备了 10 道难度降低了的题目，再让女儿做，结果一下做对了 5 道。他又鼓励女儿说：“天哪，你真是太 56 了！一天之内，你可以进步这么大！”', 53, 56, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|53-56', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_5356 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_5356
VALUES
('HSK5', 'Reading', 'SingleChoice', N'53．她女儿小时候特别不喜欢数学，53。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|53', @GroupId, 'ReadingPart1', 53, 'HSK5_H51004_Q53', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'54．周宏并没有生气，而是对女儿大加 54。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|54', @GroupId, 'ReadingPart1', 54, 'HSK5_H51004_Q54', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'55．周宏 55 准备了 10 道难度降低了的题目。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|55', @GroupId, 'ReadingPart1', 55, 'HSK5_H51004_Q55', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'56．天哪，你真是太 56 了！', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|56', @GroupId, 'ReadingPart1', 56, 'HSK5_H51004_Q56', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_5356 q
JOIN (VALUES
 (53, N'对数学很好奇',0,1),(53, N'所以数学成绩很差',1,2),(53, N'被一所大学录取了',0,3),(53, N'学校里的老师都很喜欢她',0,4),
 (54, N'轻视',0,1),(54, N'确认',0,2),(54, N'称赞',1,3),(54, N'询问',0,4),
 (55, N'特意',1,1),(55, N'逐步',0,2),(55, N'分别',0,3),(55, N'始终',0,4),
 (56, N'专心',0,1),(56, N'意外',0,2),(56, N'不要紧',0,3),(56, N'了不起',1,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 57-60 PassageCloze */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart1', 'PassageCloze', 'HSK5_H51004_57_60', N'HSK5 H51004 57-60', N'第 57-60 题：请选出正确答案。', N'春秋时期，齐国和楚国是著名的强国。有一回，齐国的国王派自己的大臣晏子访问楚国。楚王想趁这个机会，找一个办法让齐国的使者丢脸，借此来 57 楚国的强大。楚王了解到晏子身材矮小，就命令手下的人在城门旁边开了一个很低的洞。晏子来到楚国的时候，楚王命令守门的士兵关闭了城门，让晏子从旁边的洞口爬进去。晏子看到这样的 58，明白了楚王的目的，他思考了一小会儿，便对 59 的人说：“这是一个狗洞，不是城门。我要是访问狗国，当然可以爬狗洞。请你们替我问一下楚王，我来访问的国家到底是楚国还是狗国？”楚王听了士兵的报告，60，迎接晏子进城。', 57, 60, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|57-60', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_5760 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_5760
VALUES
('HSK5', 'Reading', 'SingleChoice', N'57．借此来 57 楚国的强大', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|57', @GroupId, 'ReadingPart1', 57, 'HSK5_H51004_Q57', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'58．晏子看到这样的 58', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|58', @GroupId, 'ReadingPart1', 58, 'HSK5_H51004_Q58', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'59．便对 59 的人说', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|59', @GroupId, 'ReadingPart1', 59, 'HSK5_H51004_Q59', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'60．楚王听了士兵的报告，60，迎接晏子进城。', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|60', @GroupId, 'ReadingPart1', 60, 'HSK5_H51004_Q60', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_5760 q
JOIN (VALUES
 (57, N'具备',0,1),(57, N'显示',1,2),(57, N'发表',0,3),(57, N'领导',0,4),
 (58, N'情景',1,1),(58, N'景色',0,2),(58, N'背景',0,3),(58, N'奇迹',0,4),
 (59, N'说服',0,1),(59, N'批准',0,2),(59, N'咨询',0,3),(59, N'接待',1,4),
 (60, N'变得非常愤怒',0,1),(60, N'思考了很长时间',0,2),(60, N'只好命令打开城门',1,3),(60, N'发现晏子是个很有智慧的人',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 61-70 SingleQuestion - split into 10 groups of size 1 so 40 can be composed exactly */
DECLARE @HSK5SingleQuestions TABLE
(
    SourceQuestionNo INT,
    QuestionText NVARCHAR(MAX),
    A NVARCHAR(200),
    B NVARCHAR(200),
    C NVARCHAR(200),
    D NVARCHAR(200),
    CorrectOption CHAR(1)
);

INSERT INTO @HSK5SingleQuestions
(SourceQuestionNo, QuestionText, A, B, C, D, CorrectOption)
VALUES
(61, N'拿着尺子上街，只量别人不量自己是行不通的。生活的多样性、复杂性要求我们必须接受不同的性格、不同的思想。所有这些不同的东西需要我们有一颗包容的心，而不是拿着自己的标准去要求别人。', N'要尊重个性', N'人生充满挑战', N'对自己要严格要求', N'我们总会有相同的地方', 'A'),
(62, N'这是一本十分有趣的书，书中讲了 12 个关于胆小鬼的故事。它希望让孩子明白一个道理：要想干成事情，首先就得克服胆子小的毛病。为了给孩子们的阅读带来更大的乐趣和方便，书中还配有大量插图和汉语拼音。', N'这本书配有光盘', N'作者小时候胆子很小', N'这本书的读者是孩子', N'这本书里有 12 个人物', 'C'),
(63, N'说到健康食品，大家通常都会想到蔬菜、水果，而把肉类看做健康的敌人。其实，很多肉类对人体健康有很重要的作用。至今，很多国家并没有规定什么才是健康食品。因此，现在市场上所谓的健康食品其实没有统一的标准。', N'饮食要规律', N'肉类不是健康食品', N'蔬菜水果营养成分少', N'健康食品没有统一标准', 'D'),
(64, N'冬天是一年中最寒冷的季节，很多植物没有了绿叶，一些动物会选择休眠，许多鸟儿飞到较为温暖的地方过冬。这个世界仿佛一下子安静下来了，然而，这所有的一切都是在为明年做打算。', N'冬季有很多节日', N'人们在冬天都很忙', N'冬天是一年中最长的季节', N'冬天是为来年做准备的季节', 'D'),
(65, N'优秀的员工奉行这样的理念：不找借口找办法，办法总比问题多。这是一个充满自信的理念，也是一个更具建设性、创造性的理念。世上少有解决不了的问题，只有不会解决问题的人。问题只要被发现了，在认真分析清楚后，一般总能找到相应的解决办法。', N'生活中需要借口', N'发现问题的能力很重要', N'总会有解决问题的办法', N'优秀员工常会提出许多问题', 'C'),
(66, N'日出而作，日落而息。人们一般习惯在晚上睡觉，在黑暗中睡觉，关灯并用窗帘挡住室外照进来的光线。亮着灯睡觉会使人推迟入睡时间，而且较难进入深睡阶段。光照会提高脑的兴奋度，因而去除光照刺激，减少卧室光线，对预防失眠有很大帮助。', N'开灯睡觉影响睡眠', N'光照使人神经放松', N'缺乏睡眠危害健康', N'白天睡眠质量更高', 'A'),
(67, N'用茶量的多少与消费者的习惯有密切关系。在中国西北部的一些少数民族地区，人们喜欢喝浓茶，并在茶中加糖、奶或者盐，每次茶叶用量也比较多。华北和东北广大地区的人们喜欢喝花茶，通常用较大的茶壶泡茶，但茶叶用量比较少。', N'花茶在南方更受欢迎', N'用茶量取决于茶的质量', N'一些少数民族喜欢喝浓茶', N'茶中加糖、奶或者盐不好', 'C'),
(68, N'山西省位于黄河中游，黄土高原的东部，是中华民族文明的发祥地之一，历史悠久，源远流长，素有“中国古代艺术博物馆”“文献之邦”的美称，保留有全国 70%的地面古代建筑。', N'山西的历史不长', N'山西旅游资源丰富', N'山西的风俗很特别', N'山西的发展速度很快', 'B'),
(69, N'什么是时尚？《时尚的哲学》一书的作者说过：“如果一种现象消失得像它出现时那样匆匆，那么我们就把它称做时尚。”时尚关系到生活的各个方面，包括服装、饮食、日用品等一切可以向别人展示的东西。', N'时尚指流行服饰', N'时尚是不断变化的', N'时尚脱离了实际生活', N'时尚是一个抽象的概念', 'B'),
(70, N'体育现场广告，有赛场地面广告，如篮球场开球区、摩托车赛车跑道等地面广告；还有赛场场地广告，如足球场四周的挡板广告、田径场跑道两边的广告牌。随着运动员的移动，作为背景的广告牌也不断展现在观众眼前。', N'广告让观众感到很不耐烦', N'广告应该被设计成移动的', N'在比赛现场做广告效果不错', N'广告会令运动员注意力不集中', 'C');

DECLARE @No INT, @Text NVARCHAR(MAX), @A NVARCHAR(200), @B NVARCHAR(200), @C NVARCHAR(200), @D NVARCHAR(200), @Correct CHAR(1), @QuestionId INT;
DECLARE cur_hsk5_single CURSOR LOCAL FAST_FORWARD FOR
    SELECT SourceQuestionNo, QuestionText, A, B, C, D, CorrectOption
    FROM @HSK5SingleQuestions
    ORDER BY SourceQuestionNo;

OPEN cur_hsk5_single;
FETCH NEXT FROM cur_hsk5_single INTO @No, @Text, @A, @B, @C, @D, @Correct;

WHILE @@FETCH_STATUS = 0
BEGIN
    INSERT INTO dbo.dt314_HskQuestionGroup
    (LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
    VALUES
    ('HSK5', 'Reading', 'ReadingPart2', 'SingleQuestion', CONCAT('HSK5_H51004_', @No), CONCAT(N'HSK5 H51004 ', @No), N'第 61-70 题：请选出与试题内容一致的一项。', 1, 1, 1, 1, CONCAT(@SeedPrefix, N'|HSK5|', @No), GETDATE());
    SET @GroupId = SCOPE_IDENTITY();

    INSERT INTO dbo.dt314_HskQuestions
    (LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
    VALUES
    ('HSK5', 'Reading', 'SingleChoice', @Text, 0, 1, GETDATE(), CONCAT(@SeedPrefix, N'|Q|HSK5|', @No), @GroupId, 'ReadingPart2', @No, CONCAT('HSK5_H51004_Q', @No), 1, 0);
    SET @QuestionId = SCOPE_IDENTITY();

    INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
    VALUES
    (@QuestionId, @A, CASE WHEN @Correct = 'A' THEN 1 ELSE 0 END, 1, 1),
    (@QuestionId, @B, CASE WHEN @Correct = 'B' THEN 1 ELSE 0 END, 2, 1),
    (@QuestionId, @C, CASE WHEN @Correct = 'C' THEN 1 ELSE 0 END, 3, 1),
    (@QuestionId, @D, CASE WHEN @Correct = 'D' THEN 1 ELSE 0 END, 4, 1);

    FETCH NEXT FROM cur_hsk5_single INTO @No, @Text, @A, @B, @C, @D, @Correct;
END;

CLOSE cur_hsk5_single;
DEALLOCATE cur_hsk5_single;

/* HSK5 H51004 71-74 SharedPassage */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart3', 'SharedPassage', 'HSK5_H51004_71_74', N'HSK5 H51004 71-74', N'第 71-74 题：请根据短文选出正确答案。', N'一个冬天，一个人带着猎狗去打猎。那个人一枪击中了一只兔子的腿，受伤的兔子拼命地跑，猎狗在它后面一直追。可是追了一阵儿，兔子跑得越来越远。猎狗知道实在是追不上了，只好回到猎人身边。那个人非常生气地说：“你真没用，连一只受伤的兔子都追不到！”猎狗听了很不服气地说：“我已经尽力而为了！”那只兔子带着枪伤成功地逃回家里，同伴们都围过来惊讶地问它：“那只猎狗很凶呀，你又带了伤，是怎么甩掉它的呢？”兔子说：“它是尽力而为，我是用尽全力呀！它没追上我，最多挨一顿骂，而我若不用尽全力地跑，可就没命了！”每个人都有很大的潜能。谁要想成功，创造奇迹，仅仅做到尽力而为还远远不够，必须用尽全力才行。', 71, 74, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|71-74', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_7174 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_7174
VALUES
('HSK5', 'Reading', 'SingleChoice', N'兔子的腿怎么了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|71', @GroupId, 'ReadingPart3', 71, 'HSK5_H51004_Q71', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'猎狗为什么被主人骂了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|72', @GroupId, 'ReadingPart3', 72, 'HSK5_H51004_Q72', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'兔子最后怎么了？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|73', @GroupId, 'ReadingPart3', 73, 'HSK5_H51004_Q73', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'这个故事说明了什么道理？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|74', @GroupId, 'ReadingPart3', 74, 'HSK5_H51004_Q74', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_7174 q
JOIN (VALUES
 (71, N'摔断了',0,1),(71, N'被砍伤了',0,2),(71, N'被枪打中了',1,3),(71, N'被猎狗咬伤了',0,4),
 (72, N'没找到猎物',0,1),(72, N'没有追到兔子',1,2),(72, N'把兔子咬死了',0,3),(72, N'偷偷放走了兔子',0,4),
 (73, N'逃跑了',1,1),(73, N'捉住了猎狗',0,2),(73, N'被同伴救了',0,3),(73, N'被猎人捉住了',0,4),
 (74, N'时间就是生命',0,1),(74, N'要敢于承认错误',0,2),(74, N'尽全力才能成功',1,3),(74, N'做事要有合作精神',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 75-78 SharedPassage */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart3', 'SharedPassage', 'HSK5_H51004_75_78', N'HSK5 H51004 75-78', N'第 75-78 题：请根据短文选出正确答案。', N'尽管方便快捷的“网络阅读”已经成为了一种生活时尚，但纸质阅读仍然发挥着很大的作用。日前，我们通过问卷调查、现场采访的方式对不同阶层的读者进行了调查，结果显示，市民电子阅读的兴趣日渐提高，但很多人仍在坚守传统的纸质阅读。调查发现，经常上网浏览书籍的读者占被调查者的 60%，而喜欢纸质阅读的读者高达 90%。许多读者表示传统图书提供了非常明了、有用的信息，阅读时没有广告等干扰。另外，多数读者认为长期对着屏幕阅读，也容易带来眼干、肩膀疼、腰疼等问题。纸质阅读更有利于保护眼睛。', 75, 78, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|75-78', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_7578 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_7578
VALUES
('HSK5', 'Reading', 'SingleChoice', N'被调查者的阅读习惯有：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|75', @GroupId, 'ReadingPart3', 75, 'HSK5_H51004_Q75', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'这次调查的结论是什么？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|76', @GroupId, 'ReadingPart3', 76, 'HSK5_H51004_Q76', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'与网络阅读比起来，纸质阅读：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|77', @GroupId, 'ReadingPart3', 77, 'HSK5_H51004_Q77', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'根据上文，下列哪项正确？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|78', @GroupId, 'ReadingPart3', 78, 'HSK5_H51004_Q78', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_7578 q
JOIN (VALUES
 (75, N'少数人接受纸质阅读',0,1),(75, N'年轻人喜欢网络阅读',0,2),(75, N'多数人经常上网阅读',1,3),(75, N'大部分记者习惯网上阅读',0,4),
 (76, N'人们的阅读量在减少',0,1),(76, N'人们还不熟悉网络阅读',0,2),(76, N'传统阅读仍有很大市场',1,3),(76, N'网络阅读将取代纸质阅读',0,4),
 (77, N'读者更少',0,1),(77, N'更损害眼睛',0,2),(77, N'比较浪费时间',0,3),(77, N'不受广告影响',1,4),
 (78, N'调查对象是年轻人',0,1),(78, N'许多人接受了电子阅读',1,2),(78, N'阅读方式决定阅读质量',0,3),(78, N'电子阅读器的技术发展较慢',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 79-82 SharedPassage */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart3', 'SharedPassage', 'HSK5_H51004_79_82', N'HSK5 H51004 79-82', N'第 79-82 题：请根据短文选出正确答案。', N'有三个孩子在树林里玩儿，都不小心让树枝挂破了裤子。面对裤腿上的破洞和孩子不安的脸，三位母亲用不同的态度处理了这件事情。第一位母亲大声教训了孩子之后，用一根线绳像系麻袋一样把那个破洞扎紧。第二位母亲不打也不骂，默默地把那个破洞一针一线缝补好。第三位母亲安慰孩子，并用彩线在破洞上绣了朵漂亮的小红花。第三位母亲用裤子上的花朵启发了孩子美好的想象，让孩子在成长的路上充满自信并富有创造力。', 79, 82, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|79-82', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_7982 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_7982
VALUES
('HSK5', 'Reading', 'SingleChoice', N'三个孩子在树林里玩儿时发生了什么？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|79', @GroupId, 'ReadingPart3', 79, 'HSK5_H51004_Q79', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'关于第二位母亲，下列哪项正确？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|80', @GroupId, 'ReadingPart3', 80, 'HSK5_H51004_Q80', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'第三位母亲让孩子：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|81', @GroupId, 'ReadingPart3', 81, 'HSK5_H51004_Q81', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'最适合做上文标题的是：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|82', @GroupId, 'ReadingPart3', 82, 'HSK5_H51004_Q82', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_7982 q
JOIN (VALUES
 (79, N'迷路了',0,1),(79, N'吵架了',0,2),(79, N'发现了山洞',0,3),(79, N'把裤子弄破了',1,4),
 (80, N'很平静',1,1),(80, N'批评了孩子',0,2),(80, N'对孩子很严格',0,3),(80, N'给孩子买了新裤子',0,4),
 (81, N'充满自信',1,1),(81, N'更加独立',0,2),(81, N'懂得珍惜',0,3),(81, N'学会服从',0,4),
 (82, N'我的母亲',0,1),(82, N'成长的教训',0,2),(82, N'家庭的温暖',0,3),(82, N'裤腿上的小红花',1,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* HSK5 H51004 83-86 SharedPassage */
INSERT INTO dbo.dt314_HskQuestionGroup
(LevelCode, SectionCode, PartCode, GroupType, GroupCode, Title, InstructionText, SharedPassage, SourceQuestionFrom, SourceQuestionTo, QuestionCount, RandomAsUnit, RandomWeight, IsActive, Remark, CreatedDate)
VALUES
('HSK5', 'Reading', 'ReadingPart3', 'SharedPassage', 'HSK5_H51004_83_86', N'HSK5 H51004 83-86', N'第 83-86 题：请根据短文选出正确答案。', N'我们每个人都希望自己生活得快乐。快乐不是别人给的，而是来自于自己的心态，是自己内心的感受。同一件事，不同的人有不同的心态，因而便产生不同的结果。有三句看似很简单的话，曾经帮助很多人找到并体验了人生的快乐。这三句话就是“太好了！”“我能行！”“我帮你！”乐观的人、自信的人和助人为乐的人，更容易体验人生的快乐。', 83, 86, 4, 1, 1, 1, @SeedPrefix + N'|HSK5|83-86', GETDATE());
SET @GroupId = SCOPE_IDENTITY();
DECLARE @Q_HSK5_8386 TABLE (QuestionId INT, SourceQuestionNo INT);
INSERT INTO dbo.dt314_HskQuestions
(LevelCode, SectionCode, QuestionType, DisplayText, IsMultiAns, IsActive, CreatedDate, Remark, GroupId, PartCode, SourceQuestionNo, QuestionCode, DifficultyWeight, UsageCount)
OUTPUT INSERTED.Id, INSERTED.SourceQuestionNo INTO @Q_HSK5_8386
VALUES
('HSK5', 'Reading', 'SingleChoice', N'根据上文，快乐是由什么决定的？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|83', @GroupId, 'ReadingPart3', 83, 'HSK5_H51004_Q83', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'根据上文，为什么有人会失败？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|84', @GroupId, 'ReadingPart3', 84, 'HSK5_H51004_Q84', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'作者认为应该如何实现人生的价值？', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|85', @GroupId, 'ReadingPart3', 85, 'HSK5_H51004_Q85', 1, 0),
('HSK5', 'Reading', 'SingleChoice', N'上文主要讲的是：', 0, 1, GETDATE(), @SeedPrefix + N'|Q|HSK5|86', @GroupId, 'ReadingPart3', 86, 'HSK5_H51004_Q86', 1, 0);
INSERT INTO dbo.dt314_HskAnswers (QuesId, DisplayText, TrueAns, DisplayOrder, IsActive)
SELECT q.QuestionId, a.DisplayText, a.TrueAns, a.DisplayOrder, 1
FROM @Q_HSK5_8386 q
JOIN (VALUES
 (83, N'个人内心的看法',1,1),(83, N'良好的人际关系',0,2),(83, N'丰富的人生体验',0,3),(83, N'别人的支持和鼓励',0,4),
 (84, N'不够虚心',0,1),(84, N'没有计划',0,2),(84, N'缺少耐心',0,3),(84, N'没有信心',1,4),
 (85, N'帮助他人',1,1),(85, N'获取更多知识',0,2),(85, N'提高自己的能力',0,3),(85, N'减少自己的烦恼',0,4),
 (86, N'怎样才能快乐起来',1,1),(86, N'每个人都是幸运的',0,2),(86, N'心情对健康的影响',0,3),(86, N'要重视兴趣的培养',0,4)
) a(SourceQuestionNo, DisplayText, TrueAns, DisplayOrder)
  ON a.SourceQuestionNo = q.SourceQuestionNo;

/* ---------------------------------------------------------------------------
   Summary
--------------------------------------------------------------------------- */
SELECT
    q.LevelCode,
    q.SectionCode,
    ISNULL(q.PartCode, '') AS PartCode,
    COUNT(*) AS QuestionCount
FROM dbo.dt314_HskQuestions q
GROUP BY q.LevelCode, q.SectionCode, q.PartCode
ORDER BY q.LevelCode, q.SectionCode, q.PartCode;

SELECT
    g.LevelCode,
    g.GroupType,
    COUNT(*) AS GroupCount,
    SUM(g.QuestionCount) AS TotalQuestions
FROM dbo.dt314_HskQuestionGroup g
GROUP BY g.LevelCode, g.GroupType
ORDER BY g.LevelCode, g.GroupType;
