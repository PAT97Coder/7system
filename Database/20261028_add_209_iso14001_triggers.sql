IF OBJECT_ID(N'dbo.trg209_trg_UpdateIsDisableAndIsPaperType', N'TR') IS NOT NULL DROP TRIGGER [dbo].[trg209_trg_UpdateIsDisableAndIsPaperType];
GO


CREATE TRIGGER [dbo].[trg209_trg_UpdateIsDisableAndIsPaperType]
ON [dbo].[dt209_Base]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Kiểm tra nếu cột IsDisable thay đổi và có giá trị = 1
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
            INNER JOIN deleted d
                ON i.Id = d.Id
        WHERE i.IsDisable = 1
              AND d.IsDisable != i.IsDisable
    )
    BEGIN
        -- Cập nhật tất cả các bản ghi con với IsDisable = 1, trừ bản ghi có IsFinalNode = 1
        WITH RecursiveUpdate
        AS (SELECT Id
            FROM inserted
            WHERE IsDisable = 1
            UNION ALL
            SELECT db.Id
            FROM [dbo].[dt209_Base] db
                INNER JOIN RecursiveUpdate ru
                    ON db.IdParent = ru.Id)
        UPDATE db
        SET db.IsDisable = 1
        FROM [dbo].[dt209_Base] db
            INNER JOIN RecursiveUpdate ru
                ON db.Id = ru.Id;
    END;

    -- Kiểm tra nếu cột IsDisable thay đổi và có giá trị = 0
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
            INNER JOIN deleted d
                ON i.Id = d.Id
        WHERE i.IsDisable = 0
              AND d.IsDisable != i.IsDisable
    )
    BEGIN
        -- Cập nhật tất cả các bản ghi con có IdParent = Id và IsFinalNode = 1
        UPDATE db
        SET db.IsDisable = 0
        FROM [dbo].[dt209_Base] db
            INNER JOIN inserted i
                ON db.IdParent = i.Id
        WHERE db.IsFinalNode = 1;
    END;

    -- Kiểm tra nếu cột IsPaperType thay đổi và có giá trị = 1
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
            INNER JOIN deleted d
                ON i.Id = d.Id
        WHERE i.IsPaperType = 1
              AND d.IsPaperType != i.IsPaperType
    )
    BEGIN
        -- Cập nhật tất cả các bản ghi con với IsPaperType = 1, trừ bản ghi có IsFinalNode = 1
        WITH RecursiveUpdate
        AS (SELECT Id
            FROM inserted
            WHERE IsPaperType = 1
            UNION ALL
            SELECT db.Id
            FROM [dbo].[dt209_Base] db
                INNER JOIN RecursiveUpdate ru
                    ON db.IdParent = ru.Id)
        UPDATE db
        SET db.IsPaperType = 1
        FROM [dbo].[dt209_Base] db
            INNER JOIN RecursiveUpdate ru
                ON db.Id = ru.Id
        WHERE (
                  db.IsFinalNode != 1
                  OR db.IsFinalNode IS NULL
              )
              AND db.IsDisable != 1;
    END;

    -- Cập nhật DocCode cho các node cuối nếu node Cha có sự thay đổi DocCode
    IF EXISTS
    (
        SELECT 1
        FROM inserted i
            JOIN deleted d
                ON i.Id = d.Id
        WHERE i.DocCode <> d.DocCode
    )
    BEGIN
        -- Cập nhật DocCode cho các bản ghi con có IsFinalNode = 1 và IdParent trùng với Id của bản ghi cha
        UPDATE b
        SET b.DocCode = i.DocCode
        FROM [dbo].[dt209_Base] b
            JOIN inserted i
                ON i.Id = b.IdParent
        WHERE b.IsFinalNode = 1;
    END;
END;
GO
