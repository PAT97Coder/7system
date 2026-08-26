-- ===========================================================
-- HSK Reading Test - Seed du lieu mau
-- ===========================================================
-- USE HskTest;
-- GO

DECLARE @blockId BIGINT;
DECLARE @qId BIGINT;

-- 1) SINGLE_CHOICE HSK4 (1 doan + 1 cau)
INSERT INTO reading_blocks (level, type, question_count, passage_title, passage_text)
VALUES ('hsk4','single_choice',1, N'周末', N'他每天都很忙，但是周末喜欢去公园跑步。');
SET @blockId = SCOPE_IDENTITY();

INSERT INTO questions (block_id, no, text, answer_key)
VALUES (@blockId, 1, N'他周末做什么？', 'A');
SET @qId = SCOPE_IDENTITY();

INSERT INTO question_options (question_id, option_key, option_text) VALUES
(@qId,'A',N'跑步'),
(@qId,'B',N'看书'),
(@qId,'C',N'睡觉'),
(@qId,'D',N'工作');

-- 2) SINGLE_CHOICE HSK5 (1 doan dai + 2 cau dung chung passage)
INSERT INTO reading_blocks (level, type, question_count, passage_title, passage_text)
VALUES ('hsk5','single_choice',2, N'时间管理',
        N'随着生活节奏的加快，越来越多的人感到时间不够用。其实，合理的计划往往比延长工作时间更有效。');
SET @blockId = SCOPE_IDENTITY();

INSERT INTO questions (block_id, no, text, answer_key)
VALUES (@blockId, 1, N'作者认为时间不够用的主要原因是什么？', 'B');
SET @qId = SCOPE_IDENTITY();
INSERT INTO question_options (question_id, option_key, option_text) VALUES
(@qId,'A',N'工作太多'),
(@qId,'B',N'生活节奏加快'),
(@qId,'C',N'睡得太少'),
(@qId,'D',N'缺乏计划');

INSERT INTO questions (block_id, no, text, answer_key)
VALUES (@blockId, 2, N'作者认为什么更有效？', 'C');
SET @qId = SCOPE_IDENTITY();
INSERT INTO question_options (question_id, option_key, option_text) VALUES
(@qId,'A',N'延长工作时间'),
(@qId,'B',N'减少休息'),
(@qId,'C',N'合理的计划'),
(@qId,'D',N'增加员工');

-- 3) FILL_BLANK HSK4 (kho tu chung A-F, 2 cau)
INSERT INTO reading_blocks (level, type, question_count)
VALUES ('hsk4','fill_blank',2);
SET @blockId = SCOPE_IDENTITY();

INSERT INTO shared_options (block_id, option_key, option_text) VALUES
(@blockId,'A',N'经常'),
(@blockId,'B',N'提供'),
(@blockId,'C',N'信心'),
(@blockId,'D',N'估计'),
(@blockId,'E',N'复杂'),
(@blockId,'F',N'受不了');

INSERT INTO questions (block_id, no, text, answer_key) VALUES
(@blockId, 1, N'他____去图书馆看书。', 'A'),
(@blockId, 2, N'这道题太____了。', 'E');

-- 4) ORDERING HSK4 (sap xep 3 cau)
INSERT INTO reading_blocks (level, type, question_count)
VALUES ('hsk4','ordering',1);
SET @blockId = SCOPE_IDENTITY();

INSERT INTO ordering_items (block_id, item_key, item_text, correct_pos) VALUES
(@blockId,'A',N'我打算明天去北京。',1),
(@blockId,'B',N'所以提前买好了火车票。',3),
(@blockId,'C',N'因为有一个重要的会议。',2);

INSERT INTO questions (block_id, no, text, answer_key)
VALUES (@blockId, 1, N'排列顺序', 'A,C,B');

-- 5) TEMPLATE 7/3 = 60 cau, toi da 3 lan lam
INSERT INTO exam_templates (name, total_count, hsk4_count, hsk5_count, max_attempts, is_active)
VALUES (N'HSK Reading 7/3', 60, 42, 18, 3, 1);

-- 6) USER mau
INSERT INTO users (username) VALUES (N'student01');
GO
