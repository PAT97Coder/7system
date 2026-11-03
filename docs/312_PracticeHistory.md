# UC312 thi thử nội bộ / ngoài bộ phận — 02/11/2026

## Khảo sát và phạm vi

Giữ tên `uc312ExamTest` vì đây là Control đã được đăng ký qua màn quản trị chức năng.
Không thêm mục menu bằng code, không cần script đăng ký menu. FluentFrame vẫn mở
UserControl theo `dm_Function.ControlName` và quyền `dm_FunctionRole` hiện có.

Khảo sát ban đầu đã đọc source, EDMX và metadata SQL của database ứng dụng (chỉ đọc `sys.tables`,
`sys.columns`, `sys.types`). Xác nhận:

- Nhân viên: `dm_User.Id` (`varchar(10)`), `DisplayName`, `DisplayNameVN`,
  `IdDepartment` (`varchar(4)`). Bộ phận: `dm_Departments.Id`, `DisplayName`, `DisplayNameVN`.
- Thi chính thức: `dt312_ExamMgmt`, `dt312_ExamUser` (`UserId`, `ExamId`, `SubmitAt`,
  `Score`, `IsPass`, `ExamData`). Không dùng các bảng này để lưu thi thử.
- Ngân hàng đề: `dt312_Groups`, `dt312_Questions`, `dt312_Answers`; cấu hình: `dt312_Setting`.
- Không có bảng lịch sử thi thử / người thi ngoài bộ phận cho module 312.
  Luồng `f312_DoExam(true)` cũ chỉ chấm, hiển thị review rồi đóng, không lưu SQL.

## Giao diện và hành vi

- Cập nhật 03/11/2026: một thanh BarManager chung có `開始內部測驗`, `開始外部測驗`
  và `重新整理`; bên dưới là một bảng kết quả duy nhất, không còn hai vùng chia đôi.
  Sáu cột: loại thi (`內部`/`外部`), mã nhân viên, họ tên, bộ phận, ngày thi và điểm.
  Mỗi lần thi giữ một dòng riêng; không gộp lịch sử theo mã nhân viên. Không hiển thị danh sách kỳ thi.
- Nội bộ dùng chính tài khoản đăng nhập, tra lại `dm_User` khi bắt đầu.
  Chỉ cho phép nếu **mã bộ phận** bắt đầu bằng `7`; không kiểm tra prefix mã nhân viên.
  Hiển thị mã, tên Trung (`DisplayName`) nếu có, sau đó tên Việt (`DisplayNameVN`),
  cuối cùng mã nhân viên; ngày nộp và điểm.
- Ngoài bộ phận mở `f312_ExternalCandidate`: mã nhân viên, họ tên, bộ phận bắt buộc;
  trim, Unicode, giới hạn lần lượt 64/128/128 ký tự. Mã được chuẩn hóa uppercase.
  Người thi không cần có trong `dm_User`. Bộ phận hiển thị từ snapshot của lần thi.
- Điểm null (toàn tự luận) hiển thị `不計分`. Bảng có lọc nhanh và tự co giãn chiều rộng cột.
- Nhấp chuột phải vào một dòng kết quả → `查看詳情`.
  Dùng lại `f312_ExamReview`, lấy JSON từ SQL theo Id lần thi và đúng loại INTERNAL/EXTERNAL.
  Phần đầu hiển thị mã nhân viên, họ tên, bộ phận, ngày thi và mã lần thi; bên dưới là
  điểm, câu hỏi, đáp án đã chọn, đáp án đúng và tự luận/đáp án tham khảo.
  Menu giữ đúng dòng đã nhấp dù focus thay đổi. Bài chưa nộp, mất dữ liệu hoặc JSON
  không hợp lệ sẽ được xử lý bằng thông báo. Không cần thay đổi cấu trúc SQL.
- Người được cấp `uc312ExamTest` có quyền sử dụng thi thử. Các quyền thi/quản lý đề cũ vẫn hoạt động.
  Bảng chung hiển thị lịch sử của module cho người có quyền thi thử; loại thi gốc vẫn được giữ riêng trong dữ liệu.

## Tái sử dụng và lưu kết quả

`f312_DoExam(true)` vẫn thực hiện lấy câu hỏi, phân bổ nhóm, điều hướng, giữ đáp án,
đếm giờ, nộp bài và hiển thị `f312_ExamReview`. `dt312_ExamScoring` không thay đổi.

Chỉ bổ sung callback tùy chọn `SavePracticeResult`: UC mới gắn callback để lưu SQL
trước khi hiện review; nút thi thử cũ không gắn callback nên giữ hành vi không lưu như trước.
Luồng thi chính thức không dùng callback.

- Lúc bắt đầu: một transaction lưu/cập nhật hồ sơ ngoài bộ phận (nếu là EXTERNAL)
  và tạo lần thi với `Id int IDENTITY` riêng. SQL lỗi sẽ rollback, không mở bài thi.
- Lúc nộp: lưu `Score`, JSON đáp án `ExamData`, `SubmitAt` vào đúng lần thi và đúng
  tài khoản đã mở phiên. Chưa nộp thì chưa hiển thị trong bảng kết quả.
- SQL lỗi khi nộp: giữ form/đáp án, báo lỗi, chạy tiếp đồng hồ nếu còn thời gian;
  người dùng có thể nộp lại. Nộp lại đúng cùng payload đã lưu được chấp nhận, không tạo dòng mới.
  Không cho ghi đè một kết quả đã lưu bằng điểm/đáp án khác.
- Nếu đã lưu nhưng review lỗi: thông báo kết quả đã lưu rồi đóng phiên;
  không báo nhầm là mất bài hoặc cho sửa kết quả đã hoàn tất.
- Nếu không tạo được đề sau khi đã lưu thông tin bắt đầu, lần thi vẫn ở trạng thái chưa nộp;
  không tạo kết quả giả và không đưa vào bảng kết quả. Không bổ sung cơ chế khôi phục phiên đang làm.

## SQL và triển khai

Review rồi chạy `Database/20261102_add_312_practice_history.sql` trên database đích
trước khi mở UC mới. Script chạy trong transaction, có thể chạy lại; chỉ thêm:

1. `dt312_ExternalParticipant`: khóa int riêng; mã nhân viên ngoài bộ phận duy nhất;
   tên, bộ phận Unicode và thời điểm cập nhật.
2. `dt312_PracticeAttempt`: khóa int IDENTITY từng lần thi; `PracticeKind` = `INTERNAL` / `EXTERNAL`
   theo nút người dùng chọn, không suy đoán từ mã nhân viên; liên kết `InternalUserId`
   hoặc `ExternalParticipantId` với check constraint chỉ cho phép một loại.
   Lưu snapshot mã/tên/bộ phận theo từng lần thi, tài khoản mở phiên, ngày bắt đầu/nộp
   kiểu `datetime`, điểm nullable và JSON bài thi. Có check điểm/trạng thái và index lịch sử.
   **Không tạo khóa ngoại**; các cột liên kết là cột thường, BUS kiểm tra nghiệp vụ.

Hai bảng dùng `Id int IDENTITY(1,1)` theo các bảng 312 cũ. Nếu đã chạy bản nháp dùng
Guid hoặc bản có khóa ngoại, script sẽ dừng để kiểm tra, không tự đổi khóa hay xóa ràng buộc/dữ liệu.

Không thay đổi bảng/cột cũ, không backfill hay đổi nghĩa kết quả cũ. External trùng
mã nội bộ vẫn thuộc bảng ngoài bộ phận, không cập nhật `dm_User`. Thi ngoài nhiều lần
với cùng mã sẽ cập nhật thông tin hồ sơ hiện tại, nhưng snapshot các lần cũ giữ nguyên.
Sau khi người dùng cập nhật SQL và EDMX, BUS dùng trực tiếp `DbSet<dt312_ExternalParticipant>`
và `DbSet<dt312_PracticeAttempt>` để tạo/cập nhật hồ sơ, tạo lần thi và truy vấn lịch sử/chi tiết.
Toàn bộ truy cập dữ liệu của `dt312_PracticeBUS` dùng EF6/LINQ và `SaveChanges`;
không còn `SqlQuery`, `ExecuteSqlCommand` hay SQL thuần trong runtime BUS thi thử.
Transaction Serializable và thử lại khi deadlock/xung đột khóa duy nhất bảo vệ việc
tạo hồ sơ đồng thời; nộp bài đọc và lưu entity trong cùng transaction, so sánh payload
bằng `StringComparison.Ordinal` khi nộp lại. Thời gian dùng `DateTime.Now` theo cách
thi chính thức hiện có, bảo đảm ngày nộp không nhỏ hơn ngày bắt đầu. Không thêm thư viện.

Sau khi build và cập nhật ứng dụng, menu đã đăng ký tiếp tục dùng `uc312ExamTest`.
Thiếu migration thì UC thông báo và khóa nút bắt đầu; có thể bấm làm mới sau khi triển khai SQL.
Người dùng đã tạo hai bảng trên database ứng dụng và cập nhật EDMX. Đã xác nhận chỉ đọc:
`dt312_ExternalParticipant` có 5 cột, `dt312_PracticeAttempt` có 12 cột, cả hai có khóa
int IDENTITY. Agent không chạy migration hay ghi bài thi vào database ứng dụng.

Đã bỏ các lớp entity tạm khai báo trùng trong `dt312_PracticeModels.cs` và đăng ký hai
file entity sinh từ EDMX vào `.csproj`; file này chỉ còn DTO của giao diện.
Khôi phục constructor `ModelDocManager.Context.cs` dùng `SingleConnection.ConString`
theo cơ chế kết nối cũ; sửa cùng dòng trong `ModelDocManager.Context.tt` để cập nhật
model lần sau không ghi đè lại thành connection string có tên trong config.

## File

Tạo mới cho phần nâng cấp:

- `DataAccessLayer/dt312_PracticeModels.cs`: DTO kết quả/chi tiết, khóa int;
  hai entity bảng dùng file được sinh từ EDMX do người dùng cập nhật.
- `BusinessLayer/dt312/dt312_PracticeBUS.cs`: kiểm tra người thi, transaction bắt đầu,
  lưu nộp bài và truy vấn lịch sử.
- `KnowledgeSystem/Views/03_DepartmentManage/12_WireRodEmployeeEval/f312_ExternalCandidate.cs`
  và `.Designer.cs`: form nhập thông tin ngoài bộ phận.
- `Database/20261102_add_312_practice_history.sql`: migration bổ sung.
- `Tests/Exam312/PracticeHistoryTests.cs`: kiểm thử SQL/UI của tính năng mới.
- Tài liệu này.

Chỉnh sửa:

- `uc312ExamTest.cs`, `uc312ExamTest.Designer.cs`: nâng cấp UC trống đã tạo trước đó.
- `f312_DoExam.cs`: callback lưu thi thử tùy chọn và xử lý lỗi review sau khi lưu.
- `f312_ExamReview.cs`: overload hiển thị thông tin người thi từ lịch sử; các API cũ giữ nguyên.
- `dt312_QuestionsBUS.cs`: nhận quyền của UC thi thử mới.
- Các `.csproj` DAL/BUS/UI: đăng ký file.
- `Tests/Exam312/Run-Tests.ps1`, `Run-IntegrationTests.ps1`, `Exam312Tests.cs`,
  `Exam312DbTests.cs`: hỗ trợ thư mục build riêng, migration và test mới;
  dùng lại bộ tự động đóng dialog trong test cũ.

Không sửa `uc312_ExamMain`, Designer của nó hay `f00_FluentFrame` trong phần nâng cấp này.
Các thay đổi `Program.cs` có sẵn trong workspace không thuộc công việc này.

## Kiểm thử

Áp dụng quy tắc giao diện trong phạm vi UC thi thử và dialog ngoài bộ phận:

- Layout gốc là DevExpress LayoutControl; thanh lệnh chung phía trên và bảng kết quả chung co giãn phía dưới.
- Thanh lệnh BarManager/BarButtonItem đặt trong StandaloneBarDockControl, khai báo
  cố định ở Designer; icon SVG 32x32, caption đen. Giữ yêu cầu riêng của người dùng:
  chữ nút UC 16pt ở Normal/Hovered/Pressed; Disabled 12pt. Dialog dùng chuẩn 14.25pt.
- Tiêu đề cột 14.25pt, dữ liệu và menu 12pt; focus theo dòng, lọc nhanh, không sửa tại ô.
- Localizer `Helpers/TraditionalGridLocalizer.cs` dùng chung cho caption GridView;
  menu gốc được giữ lại, hành động xem chi tiết được phân nhóm bằng BeginGroup.
- Phân quyền kiểm tra trước thao tác và tại BUS; làm mới giữ Id dòng đang chọn.
- Thông báo lỗi dự kiến dùng zh-Hant; lỗi kỹ thuật được ghi Trace và hiển thị thông báo
  zh-Hant chung. Không đưa nguyên lỗi SQL/EF tiếng Anh vào UC thi thử.
- Không đổi tên cột nhân sự, không chuyển namespace sang Winform4System, không đổi
  khung FluentFrame của 7system và không sửa schema/database gốc.

Kiểm thử bổ sung: đồng thời tạo hai lần thi cùng mã ngoài bộ phận chỉ có một hồ sơ;
hai lần nộp khác điểm đồng thời không ghi đè nhau; tên Trung → Việt → mã và chuẩn hóa
tài khoản; giữ menu mặc định, phân nhóm/font menu, localizer và độ rộng nút khi thu nhỏ.

Kết quả sau khi gộp thanh lệnh và bảng kết quả ngày 03/11/2026: build Debug/AnyCPU thành công; **342 điều kiện kiểm tra đạt**
trên SQL LocalDB và UI. Database tạm được xóa sau khi chạy; không ghi vào database ứng dụng.

Build Debug/AnyCPU vào `%TEMP%/7system-practice-history-build` để không ghi đè DLL
đang được Visual Studio/ứng dụng sử dụng.

Chạy toàn bộ kiểm thử cũ + mới bằng SQL LocalDB, database tạm tự tạo và xóa:

```powershell
$env:EXAM312_TEST_SERVER_CONNECTION = 'Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True;MultipleActiveResultSets=True'
$env:EXAM312_LAYOUT_ONLY = '1'
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/Exam312/Run-IntegrationTests.ps1 -BinPath "$env:TEMP/7system-practice-history-build"
```

Đã kiểm tra migration chạy hai lần, quyền UC mới/không quyền, bộ phận 7710/7410/7820
và không bắt đầu bằng 7, dữ liệu bắt buộc/giới hạn/Unicode/trim, người ngoài chưa có
trong nhân sự, mã trùng nội bộ, hồ sơ nhiều lần không trùng và giữ snapshot,
lọc riêng INTERNAL/EXTERNAL, không hiển thị bài chưa nộp, điểm null, đúng tài khoản
nộp bài, lỗi SQL thật bằng trigger trong DB test khi bắt đầu/nộp và rollback/retry,
lưu JSON/điểm từ chính handler nộp bài, bảng chung 6 cột, mở lại UC từ SQL, bố cục ở
1250x800 và 900x600. Bộ regression kiểm tra thi chính thức/thi thử cũ vẫn chạy.
Kiểm tra thêm menu chuột phải thật qua hit-test cho cả hai loại thi trên cùng bảng, chọn đúng dòng dù focus
đã đổi, họ tên/bộ phận/ngày thi/Id trong DOM, tự luận và đáp án tham khảo lịch sử,
quyền xem, không lộ bài chưa nộp, không trộn loại, không có menu ở vùng trống,
xử lý JSON thiếu/hỏng và mã hóa HTML thông tin người thi.

Giao diện kết quả F312 theo ảnh module 314: nền xám, khung trắng ở đầu với thông tin
và điểm thành từng dòng; mỗi câu hiển thị trực tiếp trên nền xám, trạng thái thành
dải màu ngang, đáp án có màu phân biệt đúng/đã chọn/bỏ sót và phần tổng kết ngắn
bên dưới. Phần tổng hợp giữ dòng thành tích, bỏ mã lần thi và số câu đúng/sai/tự luận;
thông tin người thi và ngày thi vẫn hiển thị. Tự luận hiển thị riêng câu trả lời/đáp án tham khảo và không tính
đúng/sai. Giữ nguyên nội dung xem trước câu hỏi/Excel. Build thành công và 395
điều kiện kiểm tra đạt trên database tạm/DOM/UI; không ghi dữ liệu production.

In kết quả: mở `查看詳情` từ dòng lịch sử, chọn `列印／PDF` trên form
`f312_ExamReview`, chọn máy in `Microsoft Print to PDF`, rồi chọn nơi lưu PDF.
Nút chỉ bật sau khi nội dung đã tải xong; không xuất hiện trong xem trước câu hỏi/Excel.
Nội dung in dùng chính bài đang xem, gồm thông tin người thi, điểm, câu hỏi, ảnh,
câu trả lời và đáp án tham khảo. Không thay đổi database. Việc lưu PDF dùng hộp thoại
in của Windows và cần máy in PDF khả dụng; chưa kiểm chứng tạo file PDF thực tế.

Ảnh và kết quả: `%TEMP%/7system-Exam312-TestArtifacts/index.html`.
Ảnh UC và dialog mới đã được xem trực tiếp. Ảnh WebBrowser của bài thi/review chỉ
chụp bố cục vì chạy ở chế độ layout-only; DOM vẫn được kiểm tra bởi test cũ.
Chưa kiểm chứng triển khai migration, khởi động lại toàn ứng dụng với dữ liệu production,
tải đồng thời lớn hoặc mọi mức DPI/máy trạm. Không nhận các kiểm tra đó là đã đạt.
