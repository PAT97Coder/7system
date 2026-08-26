-- ===========================================================
-- HSK Reading Test - Schema (SQL Server / T-SQL)
-- ===========================================================
-- CREATE DATABASE HskTest;
-- GO
-- USE HskTest;
-- GO

-- ---------- NHOM 1: NGAN HANG CAU HOI (WinForm quan ly) ----------

CREATE TABLE reading_blocks (
    id             BIGINT IDENTITY(1,1) PRIMARY KEY,
    level          VARCHAR(10)  NOT NULL,
    type           VARCHAR(20)  NOT NULL,
    question_count INT          NOT NULL,
    passage_title  NVARCHAR(255) NULL,
    passage_text   NVARCHAR(MAX) NULL,
    created_at     DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT chk_blocks_level CHECK (level IN ('hsk4','hsk5')),
    CONSTRAINT chk_blocks_type  CHECK (type IN ('single_choice','fill_blank','ordering'))
);
GO
CREATE INDEX idx_blocks_level_type ON reading_blocks(level, type);
GO

CREATE TABLE questions (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    block_id    BIGINT        NOT NULL,
    no          INT           NOT NULL,
    text        NVARCHAR(MAX) NOT NULL,
    answer_key  NVARCHAR(20)  NOT NULL,
    CONSTRAINT fk_questions_block FOREIGN KEY (block_id)
        REFERENCES reading_blocks(id) ON DELETE CASCADE
);
GO
CREATE INDEX idx_questions_block ON questions(block_id);
GO

CREATE TABLE question_options (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    question_id BIGINT        NOT NULL,
    option_key  VARCHAR(5)    NOT NULL,
    option_text NVARCHAR(MAX) NOT NULL,
    CONSTRAINT fk_qopt_question FOREIGN KEY (question_id)
        REFERENCES questions(id) ON DELETE CASCADE,
    CONSTRAINT uq_qopt UNIQUE (question_id, option_key)
);
GO

CREATE TABLE shared_options (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    block_id    BIGINT        NOT NULL,
    option_key  VARCHAR(5)    NOT NULL,
    option_text NVARCHAR(MAX) NOT NULL,
    CONSTRAINT fk_sopt_block FOREIGN KEY (block_id)
        REFERENCES reading_blocks(id) ON DELETE CASCADE,
    CONSTRAINT uq_sopt UNIQUE (block_id, option_key)
);
GO

CREATE TABLE ordering_items (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    block_id    BIGINT        NOT NULL,
    item_key    VARCHAR(5)    NOT NULL,
    item_text   NVARCHAR(MAX) NOT NULL,
    correct_pos INT           NOT NULL,
    CONSTRAINT fk_oitem_block FOREIGN KEY (block_id)
        REFERENCES reading_blocks(id) ON DELETE CASCADE,
    CONSTRAINT uq_oitem UNIQUE (block_id, item_key)
);
GO

CREATE TABLE exam_templates (
    id           BIGINT IDENTITY(1,1) PRIMARY KEY,
    name         NVARCHAR(255) NOT NULL,
    total_count  INT           NOT NULL,
    hsk4_count   INT           NOT NULL,
    hsk5_count   INT           NOT NULL,
    max_attempts INT           NOT NULL DEFAULT 3,
    is_active    BIT           NOT NULL DEFAULT 1,
    CONSTRAINT chk_tpl_sum CHECK (hsk4_count + hsk5_count = total_count)
);
GO

-- ---------- NHOM 2: THI & LICH SU (Web FastAPI ghi) ----------

CREATE TABLE users (
    id         BIGINT IDENTITY(1,1) PRIMARY KEY,
    username   NVARCHAR(100) NOT NULL UNIQUE,
    created_at DATETIME2     NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

CREATE TABLE exam_attempts (
    id           BIGINT IDENTITY(1,1) PRIMARY KEY,
    template_id  BIGINT    NULL,
    user_id      BIGINT    NOT NULL,
    attempt_no   INT       NOT NULL DEFAULT 1,
    started_at   DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    submitted_at DATETIME2 NULL,
    score        INT       NULL,
    total        INT       NULL,
    CONSTRAINT fk_attempt_tpl  FOREIGN KEY (template_id) REFERENCES exam_templates(id),
    CONSTRAINT fk_attempt_user FOREIGN KEY (user_id)     REFERENCES users(id)
);
GO
CREATE INDEX idx_attempts_user ON exam_attempts(user_id);
GO

CREATE TABLE attempt_blocks (
    id         BIGINT IDENTITY(1,1) PRIMARY KEY,
    attempt_id BIGINT NOT NULL,
    block_id   BIGINT NOT NULL,
    position   INT    NOT NULL,
    CONSTRAINT fk_ablock_attempt FOREIGN KEY (attempt_id)
        REFERENCES exam_attempts(id) ON DELETE CASCADE,
    CONSTRAINT fk_ablock_block FOREIGN KEY (block_id)
        REFERENCES reading_blocks(id),
    CONSTRAINT uq_ablock UNIQUE (attempt_id, block_id)
);
GO

CREATE TABLE attempt_answers (
    id          BIGINT IDENTITY(1,1) PRIMARY KEY,
    attempt_id  BIGINT       NOT NULL,
    question_id BIGINT       NOT NULL,
    chosen_key  NVARCHAR(20) NULL,
    is_correct  BIT          NULL,
    CONSTRAINT fk_aans_attempt FOREIGN KEY (attempt_id)
        REFERENCES exam_attempts(id) ON DELETE CASCADE,
    CONSTRAINT fk_aans_question FOREIGN KEY (question_id)
        REFERENCES questions(id),
    CONSTRAINT uq_aans UNIQUE (attempt_id, question_id)
);
GO
