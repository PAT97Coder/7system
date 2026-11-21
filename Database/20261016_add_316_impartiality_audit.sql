-- Chạy trên database ứng dụng trước khi triển khai nhánh feature/impartiality-audit-software.
-- Schema đối chiếu ModelDocManager.edmx tại commit a23a7b4.
-- Có thể chạy lại: không xóa dữ liệu, không tự sửa bảng đã tồn tại sai cấu trúc.
-- Giữ nguyên tên cột của model hiện tại. Report.IdDept có giá trị kỹ thuật PLAN,
-- vì vậy không tạo FK từ IdDept sang dm_Departments.
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.dt316_Plan', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt316_Plan
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt316_Plan PRIMARY KEY,
            DisplayName nvarchar(64) NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveAt datetime NULL,
            RemoveBy varchar(10) NULL
        );
    END;

    IF OBJECT_ID(N'dbo.dt316_PlanUser', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt316_PlanUser
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt316_PlanUser PRIMARY KEY,
            IdPlan int NOT NULL,
            IdUser varchar(10) NOT NULL,
            IdDept varchar(10) NOT NULL
        );
    END;

    IF OBJECT_ID(N'dbo.dt316_Report', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.dt316_Report
        (
            Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_dt316_Report PRIMARY KEY,
            IdPlan int NOT NULL,
            IdDept varchar(4) NOT NULL,
            IdAdt int NULL,
            CreateAt datetime NOT NULL,
            CreateBy varchar(10) NOT NULL,
            RemoveBy varchar(10) NULL,
            RemoveAt datetime NULL
        );
    END;

    -- sys.columns.max_length tính theo byte (nvarchar(64) = 128 byte).
    DECLARE @Expected TABLE
    (
        TableName sysname NOT NULL,
        ColumnName sysname NOT NULL,
        TypeName sysname NOT NULL,
        MaxLength smallint NOT NULL,
        IsNullable bit NOT NULL,
        IsIdentity bit NOT NULL
    );
    INSERT INTO @Expected VALUES
        ('dt316_Plan', 'Id', 'int', 4, 0, 1),
        ('dt316_Plan', 'DisplayName', 'nvarchar', 128, 1, 0),
        ('dt316_Plan', 'CreateAt', 'datetime', 8, 0, 0),
        ('dt316_Plan', 'CreateBy', 'varchar', 10, 0, 0),
        ('dt316_Plan', 'RemoveAt', 'datetime', 8, 1, 0),
        ('dt316_Plan', 'RemoveBy', 'varchar', 10, 1, 0),
        ('dt316_PlanUser', 'Id', 'int', 4, 0, 1),
        ('dt316_PlanUser', 'IdPlan', 'int', 4, 0, 0),
        ('dt316_PlanUser', 'IdUser', 'varchar', 10, 0, 0),
        ('dt316_PlanUser', 'IdDept', 'varchar', 10, 0, 0),
        ('dt316_Report', 'Id', 'int', 4, 0, 1),
        ('dt316_Report', 'IdPlan', 'int', 4, 0, 0),
        ('dt316_Report', 'IdDept', 'varchar', 4, 0, 0),
        ('dt316_Report', 'IdAdt', 'int', 4, 1, 0),
        ('dt316_Report', 'CreateAt', 'datetime', 8, 0, 0),
        ('dt316_Report', 'CreateBy', 'varchar', 10, 0, 0),
        ('dt316_Report', 'RemoveBy', 'varchar', 10, 1, 0),
        ('dt316_Report', 'RemoveAt', 'datetime', 8, 1, 0);

    IF EXISTS
    (
        SELECT 1 FROM @Expected e
        LEFT JOIN sys.columns c
            ON c.object_id = OBJECT_ID(N'dbo.' + e.TableName, N'U') AND c.name = e.ColumnName
        WHERE c.column_id IS NULL OR TYPE_NAME(c.user_type_id) <> e.TypeName
            OR c.max_length <> e.MaxLength OR c.is_nullable <> e.IsNullable
            OR c.is_identity <> e.IsIdentity OR c.is_computed <> 0
    )
    BEGIN
        SELECT e.*, TYPE_NAME(c.user_type_id) AS ActualType,
            c.max_length AS ActualLength, c.is_nullable AS ActualNullable,
            c.is_identity AS ActualIdentity
        FROM @Expected e
        LEFT JOIN sys.columns c
            ON c.object_id = OBJECT_ID(N'dbo.' + e.TableName, N'U') AND c.name = e.ColumnName
        WHERE c.column_id IS NULL OR TYPE_NAME(c.user_type_id) <> e.TypeName
            OR c.max_length <> e.MaxLength OR c.is_nullable <> e.IsNullable
            OR c.is_identity <> e.IsIdentity OR c.is_computed <> 0;
        THROW 51316, N'模組316資料表結構與EF模型不符，請先檢查差異。', 1;
    END;

    IF EXISTS
    (
        SELECT 1 FROM (SELECT DISTINCT TableName FROM @Expected) t
        WHERE NOT EXISTS
        (
            SELECT 1 FROM sys.indexes i
            JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
            JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
            WHERE i.object_id = OBJECT_ID(N'dbo.' + t.TableName, N'U')
                AND i.is_primary_key = 1 AND c.name = 'Id' AND ic.key_ordinal = 1
                AND NOT EXISTS
                (
                    SELECT 1 FROM sys.index_columns other
                    WHERE other.object_id = i.object_id AND other.index_id = i.index_id
                        AND other.key_ordinal > 1
                )
        )
    )
        THROW 51316, N'模組316資料表必須以Id作為單欄主鍵。', 1;

    COMMIT TRANSACTION;
    PRINT N'模組316資料表已就緒。';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
