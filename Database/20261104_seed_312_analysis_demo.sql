-- Chạy thủ công trong SSMS. SQL Server 2014 trở lên, không dùng FOR JSON/OPENJSON.
-- Tạo bộ kết quả thi thật GIẢ LẬP đã nộp để thử 4 phạm vi của 錯題分析.
-- Không đổi cấu hình thi, quyền, người dùng hoặc bài thi có sẵn.
-- Xóa bằng file 20261104_cleanup_312_analysis_demo.sql sau khi thử.
USE [DBDocumentManagementSystem];
GO
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @Tag nvarchar(40) = N'[T312A_20261104]';
DECLARE @Owner varchar(10) = 'VNW9900311';
DECLARE @Stamp date = '20000101';
DECLARE @Users TABLE (No int PRIMARY KEY, Id varchar(10), TW nvarchar(8), Marker nvarchar(128));
INSERT @Users VALUES
    (1, 'VNW9900311', N'測試甲', @Tag + N' 測試甲'),
    (2, 'VNW9900312', N'測試乙', @Tag + N' 測試乙'),
    (3, 'VNW9900313', N'測試丙', @Tag + N' 測試丙');
DECLARE @Groups TABLE (No int PRIMARY KEY, Id int, Title nvarchar(256));
INSERT @Groups(No, Title) VALUES (1,@Tag+N' 安全'),(2,@Tag+N' 設備'),(3,@Tag+N' 流程');
DECLARE @Exams TABLE (No int PRIMARY KEY, Id int, Title nvarchar(256), ExamDate date, MC int);
INSERT @Exams(No,Title,ExamDate,MC) VALUES
    (1,@Tag+N' 2024期','20240115',2),
    (2,@Tag+N' 2025期','20250115',3),
    (3,@Tag+N' 2026期','20260115',1);
DECLARE @Questions TABLE (GroupNo int, No int, Id int, GroupId int, Title nvarchar(2048), PRIMARY KEY(GroupNo,No));
DECLARE @Attempts TABLE (ExamNo int, UserNo int, WrongA int, WrongB int, WrongC int, PRIMARY KEY(ExamNo,UserNo));
INSERT @Attempts VALUES
    (1,1,2,1,0),(1,2,1,2,1),(1,3,0,1,2),
    (2,1,1,3,2),(2,2,2,1,0),(2,3,3,0,1),
    (3,1,0,1,1),(3,2,1,0,1),(3,3,0,0,0);

BEGIN TRY
    BEGIN TRANSACTION;
    -- Dừng khi chạy lại/có trùng mã; không ghi đè tài khoản hoặc trộn hai bộ dữ liệu.
    IF EXISTS (SELECT 1 FROM dbo.dm_User u WITH (UPDLOCK,HOLDLOCK) JOIN @Users t ON u.Id=t.Id)
        THROW 51000, N'Mã người test đã tồn tại. Không ghi đè. Hãy kiểm tra/chạy script cleanup trước.', 1;
    IF EXISTS (SELECT 1 FROM dbo.dt312_Groups WITH (UPDLOCK,HOLDLOCK) WHERE CHARINDEX(@Tag,DisplayName)>0)
        OR EXISTS (SELECT 1 FROM dbo.dt312_ExamMgmt WITH (UPDLOCK,HOLDLOCK) WHERE CHARINDEX(@Tag,DisplayName)>0)
        THROW 51000, N'Bộ T312A_20261104 đã tồn tại. Hãy cleanup trước khi tạo lại.', 1;

    -- Hồ sơ dành riêng cho chọn bộ lọc; không cấp role, không tạo tài khoản AD/đăng nhập.
    INSERT dbo.dm_User(Id,DisplayName,DisplayNameVN,DateCreate)
        SELECT Id,TW,Marker,@Stamp FROM @Users;

    DECLARE @g int=1, @q int, @id int, @title nvarchar(2048), @groupId int;
    WHILE @g<=3
    BEGIN
        SELECT @title=Title FROM @Groups WHERE No=@g;
        -- Đánh dấu nhóm ngừng sử dụng: không lọt vào bốc câu hỏi của bài thi thật đang hoạt động.
        -- Phân tích vẫn đọc tên nhóm này từ lịch sử, đúng logic hiện tại.
        INSERT dbo.dt312_Groups(DisplayName,CreateAt,CreateBy,RemoveAt,RemoveBy)
            VALUES(@title,@Stamp,@Owner,@Stamp,@Owner);
        SET @groupId=CONVERT(int,SCOPE_IDENTITY());
        UPDATE @Groups SET Id=@groupId WHERE No=@g;
        SET @q=1;
        WHILE @q<=4
        BEGIN
            SET @title=@Tag+N' '+CASE @g WHEN 1 THEN N'安全' WHEN 2 THEN N'設備' ELSE N'流程' END
                +CASE @q WHEN 3 THEN N'複選題' WHEN 4 THEN N'申論題' ELSE N'單選題' END+CONVERT(nvarchar(8),@q);
            INSERT dbo.dt312_Questions(GroupId,DisplayName,ImageName,IsmultiAns,IsEssay,ReferenceAnswer)
                VALUES(@groupId,@title,NULL,CASE WHEN @q=3 THEN 1 ELSE 0 END,
                    CASE WHEN @q=4 THEN 1 ELSE 0 END,CASE WHEN @q=4 THEN N'先確認安全，再依標準程序作業。' ELSE NULL END);
            SET @id=CONVERT(int,SCOPE_IDENTITY());
            INSERT @Questions VALUES(@g,@q,@id,@groupId,@title);
            IF @q<>4
                INSERT dbo.dt312_Answers(QuesId,DisplayName,ImageName,TrueAns)
                SELECT @id,@Tag+N' 選項'+CONVERT(nvarchar(8),n.No),NULL,
                    CASE WHEN n.No=1 OR (@q=3 AND n.No=3) THEN 1 ELSE 0 END
                FROM (VALUES(1),(2),(3),(4)) n(No);
            SET @q+=1;
        END;
        SET @g+=1;
    END;

    DECLARE @e int=1, @u int, @mc int, @examDate date, @examId int, @userId varchar(10);
    DECLARE @wrongA int, @wrongB int, @wrongC int, @score int, @data nvarchar(max);
    WHILE @e<=3
    BEGIN
        SELECT @title=Title,@examDate=ExamDate,@mc=MC FROM @Exams WHERE No=@e;
        INSERT dbo.dt312_ExamMgmt(DisplayName,CreateAt,RoundNumber,FinishNotifyDate)
            VALUES(@title,@examDate,@e,@examDate);
        SET @examId=CONVERT(int,SCOPE_IDENTITY());
        UPDATE @Exams SET Id=@examId WHERE No=@e;
        SET @u=1;
        WHILE @u<=3
        BEGIN
            SELECT @userId=Id FROM @Users WHERE No=@u;
            SELECT @wrongA=WrongA,@wrongB=WrongB,@wrongC=WrongC FROM @Attempts WHERE ExamNo=@e AND UserNo=@u;
            -- JSON có cùng cấu trúc f312_DoExam.ExamResult.
            -- Id đáp án trong snapshot là số thứ tự 1–4, đúng bước renumber khi thi;
            -- không dùng Id IDENTITY của dt312_Answers làm lựa chọn trong JSON.
            ;WITH items AS (
                SELECT q.*,ROW_NUMBER() OVER(ORDER BY q.GroupNo,q.No) AS QuestionIndex,
                    CASE WHEN q.No<>4 AND q.No<=CASE q.GroupNo WHEN 1 THEN @wrongA WHEN 2 THEN @wrongB ELSE @wrongC END THEN 1 ELSE 0 END AS Wrong
                FROM @Questions q WHERE q.No<=@mc OR q.No=4
            )
            SELECT @data=N'['+STUFF((
                SELECT N',{"Questions":{"Id":'+CONVERT(nvarchar(12),i.Id)
                    +N',"GroupId":'+CONVERT(nvarchar(12),i.GroupId)+N',"DisplayName":"'+i.Title
                    +N'","ImageName":null,"IsmultiAns":'+CASE WHEN i.No=3 THEN N'true' ELSE N'false' END
                    +N',"IsEssay":'+CASE WHEN i.No=4 THEN N'true' ELSE N'false' END
                    +N',"ReferenceAnswer":'+CASE WHEN i.No=4 THEN N'"先確認安全，再依標準程序作業。"' ELSE N'null' END+N'},"Answers":'
                    +CASE WHEN i.No=4 THEN N'[]' ELSE N'['+STUFF((
                        SELECT N',{"Id":'+CONVERT(nvarchar(8),n.No)+N',"QuesId":'+CONVERT(nvarchar(12),i.Id)
                            +N',"DisplayName":"'+@Tag+N' 選項'+CONVERT(nvarchar(8),n.No)
                            +N'","ImageName":null,"TrueAns":'+CASE WHEN n.No=1 OR (i.No=3 AND n.No=3) THEN N'true' ELSE N'false' END+N'}'
                        FROM (VALUES(1),(2),(3),(4)) n(No) ORDER BY n.No
                        FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,1,N'')+N']' END
                    +N',"QuestionIndex":'+CONVERT(nvarchar(12),i.QuestionIndex)
                    +N',"CorrectAnswer":"'+CASE WHEN i.No=4 THEN N'' WHEN i.No=3 THEN N'1,3' ELSE N'1' END
                    +N'","UserAnswer":"'+CASE WHEN i.No=4 THEN N'測試申論作答，不列入計分。'
                        WHEN i.Wrong=1 AND @u=3 AND i.GroupNo=1 AND i.No=1 THEN N''
                        WHEN i.Wrong=1 THEN CASE WHEN i.No=3 THEN N'1' ELSE N'2' END
                        WHEN i.No=3 THEN N'3,1' ELSE N'1' END
                    +N'","IsCorrect":'+CASE WHEN i.No<>4 AND i.Wrong=0 THEN N'true' ELSE N'false' END
                    +N',"IsMultiChoice":'+CASE WHEN i.No=3 THEN N'true' ELSE N'false' END+N'}'
                FROM items i ORDER BY i.QuestionIndex
                FOR XML PATH(''),TYPE).value('.','nvarchar(max)'),1,1,N'')+N']';
            -- Math.Round mặc định của .NET: làm tròn gần nhất, điểm giữa về số chẵn.
            -- Mẫu có 3/6/9 câu trắc nghiệm nên không có trường hợp x.5 khác biệt với ROUND SQL.
            SET @score=CONVERT(int,ROUND((@mc*3-@wrongA-@wrongB-@wrongC)*100.0/(@mc*3),0));
            INSERT dbo.dt312_ExamUser(ExamId,UserId,SubmitAt,Score,IsPass,ExamData)
                VALUES(@examId,@userId,DATEADD(minute,@u*5,DATEADD(hour,9,CONVERT(datetime,@examDate))),
                    @score,CASE WHEN @score>=80 THEN 1 ELSE 0 END,@data);
            SET @u+=1;
        END;
        SET @e+=1;
    END;
    COMMIT TRANSACTION;
    SELECT N'Đã tạo bộ T312A_20261104: 3 người, 3 kỳ, 3 nhóm, 12 câu, 36 đáp án, 9 bài đã nộp.' AS KetQua;
    SELECT No AS Ky,Id AS ExamId,Title AS TenKy,MC AS TracNghiemMoiNhom FROM @Exams ORDER BY No;
    SELECT Id AS UserId,TW AS Ten FROM @Users ORDER BY No;
    SELECT Id AS GroupId,Title AS TenNhom FROM @Groups ORDER BY No;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT>0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
