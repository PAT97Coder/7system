/*
Dữ liệu test HIỂN THỊ UC320. Chạy toàn bộ trong SSMS trên server bạn chọn.
Không tạo bảng/cột và không sửa dữ liệu 316.
Tạo 4 kế hoạch với chu kỳ M/H/Q/Y; mỗi kế hoạch có 2 người cho mỗi nhóm.
Người/đơn vị được lấy từ danh mục thật có mã đơn vị đầu 7.
File chỉ là tên mẫu để xem giao diện, KHÔNG có file vật lý để tải/mở.
Không gán file báo cáo thành file kế hoạch (schema chưa có liên kết 計劃表).
Chạy lại sẽ dừng nếu đã có tên kế hoạch test, không chèn thêm bản trùng.
*/
USE [DBDocumentManagementSystem];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    IF EXISTS (SELECT 1 FROM dbo.dt320_Plan WHERE DisplayName LIKE N'[[]TEST320] %')
        THROW 51000, 'TEST320 plans already exist. No new test data was inserted.', 1;

    DECLARE @People TABLE (RowNo int PRIMARY KEY, IdUser varchar(10), IdDept varchar(4));
    INSERT INTO @People (RowNo, IdUser, IdDept)
    SELECT TOP (4) ROW_NUMBER() OVER (ORDER BY u.IdDepartment, u.Id), u.Id, u.IdDepartment
    FROM dbo.dm_User u
    INNER JOIN dbo.dm_Departments d ON d.Id = u.IdDepartment
    WHERE u.Status = 0 AND u.IdDepartment LIKE '7%'
    ORDER BY u.IdDepartment, u.Id;

    IF (SELECT COUNT(*) FROM @People) < 4
        THROW 51001, 'At least four active employees in 7-prefix departments are required.', 1;

    DECLARE @Creator varchar(10) = (SELECT IdUser FROM @People WHERE RowNo = 1);
    DECLARE @Now datetime = GETDATE();
    DECLARE @Items TABLE (RowNo int PRIMARY KEY, CycleType char(1), ItemName nvarchar(200));
    INSERT INTO @Items VALUES
        (1, 'M', N'[TEST320] 每月稽核'),
        (2, 'H', N'[TEST320] 每半年稽核'),
        (3, 'Q', N'[TEST320] 每季稽核'),
        (4, 'Y', N'[TEST320] 每年稽核');

    DECLARE @Index int = 1, @IdItem int, @IdPlan int, @IdDepartment int, @IdReport int;
    DECLARE @Cycle char(1), @Name nvarchar(200), @Period tinyint;
    DECLARE @Type tinyint, @Dept varchar(4), @IdAttachment int, @FileNo int;

    WHILE @Index <= 4
    BEGIN
        SELECT @Cycle = CycleType, @Name = ItemName FROM @Items WHERE RowNo = @Index;
        INSERT dbo.dt320_AuditItem (DisplayName, CycleType, Description, IsActive, CreateAt, CreateBy)
        VALUES (@Name, @Cycle, N'Dữ liệu test hiển thị; không dùng cho nghiệp vụ thật.', 1, @Now, @Creator);
        SET @IdItem = CONVERT(int, SCOPE_IDENTITY());

        SET @Period = CASE @Cycle WHEN 'M' THEN MONTH(@Now)
            WHEN 'H' THEN CASE WHEN MONTH(@Now) <= 6 THEN 1 ELSE 2 END
            WHEN 'Q' THEN DATEPART(QUARTER, @Now) ELSE 1 END;
        INSERT dbo.dt320_Plan
            (IdAuditItem, DisplayName, AuditAt, CycleType, PeriodYear, PeriodNo, Remark, CreateAt, CreateBy)
        VALUES (@IdItem, @Name + N'計劃', CONVERT(date, @Now), @Cycle, YEAR(@Now), @Period,
            N'Test danh sách kế hoạch và ba tab chi tiết.', @Now, @Creator);
        SET @IdPlan = CONVERT(int, SCOPE_IDENTITY());

        -- Nhóm 1 lấy người số 1,2; nhóm 2 lấy người số 3,4.
        -- Một đơn vị có thể xuất hiện trong hai nhóm trong bộ dữ liệu test này.
        DECLARE DepartmentCursor CURSOR LOCAL FAST_FORWARD FOR
            SELECT DISTINCT CASE WHEN RowNo <= 2 THEN 1 ELSE 2 END, IdDept FROM @People;
        OPEN DepartmentCursor;
        FETCH NEXT FROM DepartmentCursor INTO @Type, @Dept;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            INSERT dbo.dt320_PlanDepartment (IdPlan, IdDept, ParticipationType, CreateAt, CreateBy)
            VALUES (@IdPlan, @Dept, @Type, @Now, @Creator);
            SET @IdDepartment = CONVERT(int, SCOPE_IDENTITY());

            INSERT dbo.dt320_PlanUser (IdPlanDepartment, IdUser, CreateAt, CreateBy)
            SELECT @IdDepartment, IdUser, @Now, @Creator FROM @People
            WHERE IdDept = @Dept AND CASE WHEN RowNo <= 2 THEN 1 ELSE 2 END = @Type;

            INSERT dbo.dt320_Report (IdPlanDepartment, Remark, CreateAt, CreateBy)
            VALUES (@IdDepartment, N'Báo cáo test hiển thị.', @Now, @Creator);
            SET @IdReport = CONVERT(int, SCOPE_IDENTITY());

            SET @FileNo = 1;
            WHILE @FileNo <= 2
            BEGIN
                INSERT dbo.dm_Attachment (Thread, EncryptionName, ActualName)
                VALUES ('320', CONVERT(varchar(36), NEWID()) + '.pdf',
                    N'TEST320_' + CONVERT(nvarchar(10), @IdPlan) + N'_' + @Dept + N'_' +
                    CONVERT(nvarchar(2), @Type) + N'_File' + CONVERT(nvarchar(2), @FileNo) + N'.pdf');
                SET @IdAttachment = CONVERT(int, SCOPE_IDENTITY());

                INSERT dbo.dt320_ReportFile (IdReport, IdAdt, CreateAt, CreateBy)
                VALUES (@IdReport, @IdAttachment, DATEADD(MINUTE, @FileNo, @Now),
                    (SELECT IdUser FROM @People WHERE RowNo = @FileNo));
                SET @FileNo = @FileNo + 1;
            END;
            FETCH NEXT FROM DepartmentCursor INTO @Type, @Dept;
        END;
        CLOSE DepartmentCursor;
        DEALLOCATE DepartmentCursor;
        SET @Index = @Index + 1;
    END;

    COMMIT TRANSACTION;
    SELECT Id, DisplayName, AuditAt, CycleType, PeriodYear, PeriodNo
    FROM dbo.dt320_Plan WHERE DisplayName LIKE N'[[]TEST320] %' ORDER BY Id;
    PRINT N'Test data created. Open module 320 or click 刷新.';
END TRY
BEGIN CATCH
    IF CURSOR_STATUS('local', 'DepartmentCursor') >= 0 CLOSE DepartmentCursor;
    IF CURSOR_STATUS('local', 'DepartmentCursor') >= -1 DEALLOCATE DepartmentCursor;
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
