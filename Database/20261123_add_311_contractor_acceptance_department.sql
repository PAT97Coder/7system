-- Chạy sau 20261123_add_311_contractor_acceptance.sql.
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF COL_LENGTH(N'dbo.dt311_ContractorAcceptance', N'DepartmentGroup') IS NULL
    BEGIN
        ALTER TABLE dbo.dt311_ContractorAcceptance ADD DepartmentGroup varchar(2) NULL;

        EXEC sys.sp_executesql N'
            UPDATE acceptance
                SET DepartmentGroup = LEFT(LTRIM(employee.IdDepartment), 2)
            FROM dbo.dt311_ContractorAcceptance AS acceptance
            JOIN dbo.dm_User AS employee ON employee.Id = acceptance.CreatedByUserId
            WHERE LEFT(LTRIM(employee.IdDepartment), 2) COLLATE Latin1_General_100_BIN2 LIKE ''[0-9][0-9]'';

            IF EXISTS (SELECT 1 FROM dbo.dt311_ContractorAcceptance WHERE DepartmentGroup IS NULL)
                THROW 51311, N''有驗收單無法判定上傳人的部門群組，請先確認人員資料。'', 1;

            ALTER TABLE dbo.dt311_ContractorAcceptance ALTER COLUMN DepartmentGroup varchar(2) NOT NULL;
            ALTER TABLE dbo.dt311_ContractorAcceptance ADD CONSTRAINT CK_311Acceptance_DepartmentGroup
                CHECK (DepartmentGroup COLLATE Latin1_General_100_BIN2 LIKE ''[0-9][0-9]'');
            CREATE INDEX IX_311Acceptance_DepartmentGroup ON dbo.dt311_ContractorAcceptance (DepartmentGroup, ReportDate);
        ';
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
