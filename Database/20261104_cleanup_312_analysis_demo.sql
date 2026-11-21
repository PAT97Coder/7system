-- Chạy thủ công trong SSMS sau khi thử xong bộ T312A_20261104.
-- Không reset IDENTITY, không xóa theo khoảng Id, không dùng LIKE '[...]%'.
-- Kiểm tra dấu nhận diện trước khi xóa; có dữ liệu ngoài bộ mẫu thì dừng để kiểm tra.
USE [DBDocumentManagementSystem];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @Tag nvarchar(40)=N'[T312A_20261104]', @Owner varchar(10)='VNW9900311', @Stamp date='20000101';
DECLARE @Users TABLE(Id varchar(10) PRIMARY KEY, TW nvarchar(8), Marker nvarchar(128));
INSERT @Users VALUES
    ('VNW9900311',N'測試甲',@Tag+N' 測試甲'),
    ('VNW9900312',N'測試乙',@Tag+N' 測試乙'),
    ('VNW9900313',N'測試丙',@Tag+N' 測試丙');
DECLARE @Exams TABLE(Id int PRIMARY KEY);
DECLARE @Groups TABLE(Id int PRIMARY KEY);
DECLARE @Questions TABLE(Id int PRIMARY KEY);
BEGIN TRY
    BEGIN TRANSACTION;
    IF EXISTS (SELECT 1 FROM dbo.dm_User u JOIN @Users t ON u.Id=t.Id
        WHERE ISNULL(u.DisplayName,N'')<>t.TW OR ISNULL(u.DisplayNameVN,N'')<>t.Marker OR u.DateCreate<>@Stamp)
        THROW 51000,N'Mã người test không còn đúng dấu nhận diện. Dừng, không xóa tài khoản.',1;
    INSERT @Exams SELECT e.Id FROM dbo.dt312_ExamMgmt e
        WHERE (e.DisplayName=@Tag+N' 2024期' AND e.CreateAt='20240115' AND e.RoundNumber=1)
           OR (e.DisplayName=@Tag+N' 2025期' AND e.CreateAt='20250115' AND e.RoundNumber=2)
           OR (e.DisplayName=@Tag+N' 2026期' AND e.CreateAt='20260115' AND e.RoundNumber=3);
    INSERT @Groups SELECT g.Id FROM dbo.dt312_Groups g
        WHERE g.DisplayName IN(@Tag+N' 安全',@Tag+N' 設備',@Tag+N' 流程') AND g.CreateBy=@Owner AND g.CreateAt=@Stamp;
    INSERT @Questions SELECT q.Id FROM dbo.dt312_Questions q JOIN @Groups g ON q.GroupId=g.Id
        WHERE CHARINDEX(@Tag+N' ',q.DisplayName)=1;

    -- Không để lại bộ mẫu bị sửa dấu nhận diện rồi báo xóa thành công giả.
    IF EXISTS (SELECT 1 FROM dbo.dt312_ExamMgmt e WHERE CHARINDEX(@Tag,e.DisplayName)>0 AND NOT EXISTS(SELECT 1 FROM @Exams t WHERE t.Id=e.Id))
        OR EXISTS (SELECT 1 FROM dbo.dt312_Groups g WHERE CHARINDEX(@Tag,g.DisplayName)>0 AND NOT EXISTS(SELECT 1 FROM @Groups t WHERE t.Id=g.Id))
        THROW 51000,N'Kỳ thi/nhóm mang dấu test đã bị thay đổi. Hãy kiểm tra trước khi xóa.',1;
    IF EXISTS (SELECT 1 FROM dbo.dt312_ExamUser a JOIN @Exams e ON e.Id=a.ExamId WHERE NOT EXISTS(SELECT 1 FROM @Users u WHERE u.Id=a.UserId))
        OR EXISTS (SELECT 1 FROM dbo.dt312_ExamUser a JOIN @Users u ON u.Id=a.UserId WHERE NOT EXISTS(SELECT 1 FROM @Exams e WHERE e.Id=a.ExamId))
        THROW 51000,N'Có bài thi/người ngoài bộ mẫu liên quan kỳ test. Dừng để tránh xóa dữ liệu ngoài phạm vi.',1;
    IF EXISTS (SELECT 1 FROM dbo.dt312_ExamUser a WHERE CHARINDEX(@Tag,a.ExamData)>0 AND NOT EXISTS(SELECT 1 FROM @Exams e WHERE e.Id=a.ExamId))
        THROW 51000,N'Câu hỏi test đã được dùng trong bài thi ngoài bộ mẫu. Hãy kiểm tra trước khi xóa.',1;
    IF EXISTS (SELECT 1 FROM dbo.dt312_Questions q JOIN @Groups g ON q.GroupId=g.Id WHERE NOT EXISTS(SELECT 1 FROM @Questions t WHERE t.Id=q.Id))
        OR EXISTS (SELECT 1 FROM dbo.dt312_Answers a JOIN @Questions q ON q.Id=a.QuesId WHERE CHARINDEX(@Tag+N' ',a.DisplayName)<>1)
        THROW 51000,N'Nhóm/câu hỏi test có câu hoặc đáp án không mang dấu test. Dừng để kiểm tra.',1;
    IF EXISTS (SELECT 1 FROM dbo.dm_UserRole r JOIN @Users u ON u.Id=r.IdUser)
        THROW 51000,N'Người test đã được gán role ngoài script. Hãy kiểm tra và gỡ trước khi cleanup.',1;
    IF OBJECT_ID(N'dbo.dt312_PracticeAttempt',N'U') IS NOT NULL
    BEGIN
        IF EXISTS (SELECT 1 FROM dbo.dt312_PracticeAttempt a WHERE a.InternalUserId IN(SELECT Id FROM @Users) OR a.StartedBy IN(SELECT Id FROM @Users))
            THROW 51000,N'Người test có dữ liệu thi thử ngoài bộ mẫu. Dừng để kiểm tra.',1;
    END;

    DECLARE @papers int,@answers int,@questionsCount int,@groupsCount int,@examsCount int,@usersCount int;
    DELETE a FROM dbo.dt312_ExamUser a JOIN @Exams e ON e.Id=a.ExamId; SET @papers=@@ROWCOUNT;
    DELETE a FROM dbo.dt312_Answers a JOIN @Questions q ON q.Id=a.QuesId; SET @answers=@@ROWCOUNT;
    DELETE q FROM dbo.dt312_Questions q JOIN @Questions t ON t.Id=q.Id; SET @questionsCount=@@ROWCOUNT;
    DELETE g FROM dbo.dt312_Groups g JOIN @Groups t ON t.Id=g.Id; SET @groupsCount=@@ROWCOUNT;
    DELETE e FROM dbo.dt312_ExamMgmt e JOIN @Exams t ON t.Id=e.Id; SET @examsCount=@@ROWCOUNT;
    DELETE u FROM dbo.dm_User u JOIN @Users t ON t.Id=u.Id AND u.DisplayName=t.TW AND u.DisplayNameVN=t.Marker AND u.DateCreate=@Stamp;
    SET @usersCount=@@ROWCOUNT;
    COMMIT TRANSACTION;
    SELECT @papers AS BaiThiDaXoa,@answers AS DapAnDaXoa,@questionsCount AS CauHoiDaXoa,
        @groupsCount AS NhomDaXoa,@examsCount AS KyDaXoa,@usersCount AS NguoiTestDaXoa;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
