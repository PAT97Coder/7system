/* ---------------------------------------------------------------------------
   [310] Replace the EHS audit-function catalog

   Notes:
   - Existing Id values are preserved by Code and occurrence order whenever
     possible, so current assignments keep pointing to the same audit number.
   - Assignments for catalog entries that no longer exist are removed before
     those entries are deleted.
   - Blank audit numbers supplied by the business are stored as NULL.
   - Duplicate audit numbers are intentionally preserved as supplied.
   --------------------------------------------------------------------------- */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.dt310_Function', N'U') IS NULL
        THROW 50001, 'Table dbo.dt310_Function does not exist.', 1;

    IF OBJECT_ID(N'dbo.dt310_EHSFunction', N'U') IS NULL
        THROW 50002, 'Table dbo.dt310_EHSFunction does not exist.', 1;

    DECLARE @Source TABLE
    (
        RowNo       INT           NOT NULL PRIMARY KEY,
        Code        VARCHAR(64)   NULL,
        DisplayName NVARCHAR(256) NOT NULL
    );

    INSERT INTO @Source (RowNo, Code, DisplayName)
    VALUES
        ( 1, 'SMB3N203001', N'用電安全查核基準'),
        ( 2, 'SMB3N203101', N'吊掛作業查核基準'),
        ( 3, 'SMB3N203201', N'堆高機安全管理查核基準'),
        ( 4, 'SMB3N203301', N'工安自主檢查查核基準'),
        ( 5, 'SMB3N203401', N'教育訓練查核基準'),
        ( 6, 'SMB3N203501', N'危險性機械設備、壓力錶、氣體偵測器安全管理查核基準'),
        ( 7, 'SMB3N203601', N'承攬商安全管理基準'),
        ( 8, 'SMB3N203701', N'電氣機具安全查核基準'),
        ( 9, 'SMB3N203801', N'電氣室安全管理查核基準'),
        (10, 'SMB3N203901', N'車輛安全管理查核基準'),
        (11, 'SMB3N204101', N'工作安全許可查核基準'),
        (12, 'SMB3N204301', N'電焊機台車查核基準'),
        (13, 'SMB3N204601', N'勞動安全衛生計畫查核基準'),
        (14, 'SMB3N204701', N'高架作業安全查核基準'),
        (15, 'SMB3N205001', N'動火作業專區安全管理查核基準'),
        (16, 'SMB3N205102', N'工具箱會議及早、中、夜班上工前會議查核基準'),
        (17, 'SMB3N205201', N'輻射安全管理查核基準'),
        (18, 'SMB3N205301', N'油品庫安全管理查核基準'),
        (19, 'SMB3N205401', N'吸菸區安全管理查核基準'),
        (20, 'SMB3N205501', N'施工動態看板查核基準'),
        (21, 'SMB3N205801', N'局限空間查核基準'),
        (22, 'SMB3N202206', N'4 春安查核基準'),
        (23, 'SMC3N201401', N'作業環境檢測查核基準'),
        (24, 'SMC3N201501', N'健檢管理查核基準'),
        (25, 'SMC3N201502', N'空氣呼吸器與氧氣急救器查核基準'),
        (26, 'SMC3N201601', N'實物補償管理查核基準'),
        (27, 'SMD3N203009', N'第一級管線外觀自主巡檢查核'),
        (28, 'SME3N201601', N'消防安全管理查核基準'),
        (29, 'SME3N201601', N'緊急應變器材倉庫管理查核表'),
        (30, 'SME3N201701', N'化學藥劑安全查核基準'),
        (31, 'SME3N201801', N'避雷接地系統查核基準'),
        (32, NULL,           N'緊急沖淋器安全檢查查核基準'),
        (33, NULL,           N'電梯作業安全檢核查查核基準'),
        (34, 'SME3N201602', N'火警探測器'),
        (35, NULL,           N'環保檢查基準');

    DECLARE @Desired TABLE
    (
        RowNo       INT           NOT NULL PRIMARY KEY,
        Id          INT           NULL,
        Code        VARCHAR(64)   NULL,
        DisplayName NVARCHAR(256) NOT NULL
    );

    INSERT INTO @Desired (RowNo, Code, DisplayName)
    SELECT RowNo, Code, DisplayName
    FROM @Source;

    /* Match repeated codes by their occurrence order to preserve stable Ids. */
    ;WITH RankedSource AS
    (
        SELECT RowNo,
               ROW_NUMBER() OVER
               (
                   PARTITION BY ISNULL(Code, '')
                   ORDER BY RowNo
               ) AS CodeOccurrence
        FROM @Source
    ),
    RankedExisting AS
    (
        SELECT Id,
               ISNULL(Code, '') AS CodeKey,
               ROW_NUMBER() OVER
               (
                   PARTITION BY ISNULL(Code, '')
                   ORDER BY Id
               ) AS CodeOccurrence
        FROM dbo.dt310_Function
    )
    UPDATE desired
    SET desired.Id = existing.Id
    FROM @Desired AS desired
    INNER JOIN RankedSource AS source
        ON source.RowNo = desired.RowNo
    INNER JOIN RankedExisting AS existing
        ON existing.CodeKey = ISNULL(desired.Code, '')
       AND existing.CodeOccurrence = source.CodeOccurrence;

    DECLARE @NextId INT = ISNULL((SELECT MAX(Id) FROM dbo.dt310_Function), 0);

    ;WITH MissingIds AS
    (
        SELECT RowNo,
               ROW_NUMBER() OVER (ORDER BY RowNo) AS NewIdOffset
        FROM @Desired
        WHERE Id IS NULL
    )
    UPDATE desired
    SET desired.Id = @NextId + missing.NewIdOffset
    FROM @Desired AS desired
    INNER JOIN MissingIds AS missing
        ON missing.RowNo = desired.RowNo;

    /* Update retained catalog rows and insert newly supplied rows. */
    UPDATE target
    SET target.Code = desired.Code,
        target.DisplayName = desired.DisplayName
    FROM dbo.dt310_Function AS target
    INNER JOIN @Desired AS desired
        ON desired.Id = target.Id;

    INSERT INTO dbo.dt310_Function (Id, Code, DisplayName)
    SELECT desired.Id, desired.Code, desired.DisplayName
    FROM @Desired AS desired
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.dt310_Function AS target
        WHERE target.Id = desired.Id
    );

    /* Removed catalog items cannot remain assigned to departments/users. */
    DELETE assignment
    FROM dbo.dt310_EHSFunction AS assignment
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM @Desired AS desired
        WHERE desired.Id = assignment.FunctionId
    );

    DELETE target
    FROM dbo.dt310_Function AS target
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM @Desired AS desired
        WHERE desired.Id = target.Id
    );

    IF (SELECT COUNT(*) FROM dbo.dt310_Function) <> 35
        THROW 50003, 'Catalog replacement failed: expected 35 rows.', 1;

    COMMIT TRANSACTION;

    SELECT Id, Code, DisplayName
    FROM dbo.dt310_Function
    ORDER BY Id;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO
