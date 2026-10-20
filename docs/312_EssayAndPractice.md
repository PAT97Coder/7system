# Module 312 — Câu tự luận và thi thử

Nhánh triển khai: `feature/312-essay-practice-exam`, tạo từ `origin/main`.

## Cài đặt

1. Chạy `Database/20261020_add_312_essay_questions.sql` trên database cần triển khai **trước khi chạy bản ứng dụng mới**.
   Script có thể chạy lại, thêm `IsEssay bit NOT NULL DEFAULT 0` và `ReferenceAnswer nvarchar(max)` vào `dt312_Questions`.
   Các câu cũ vẫn là trắc nghiệm. Model EDMX, entity và mapping EF6 được cập nhật tương ứng.
2. Build `KnowledgeSystem` rồi triển khai theo quy trình hiện có.

Script chưa được chạy vào database ứng dụng trong quá trình phát triển. Không tự cấp thêm quyền cho tài khoản.
Quyền thêm/sửa tự luận dùng quyền chức năng `uc312_GroupMgmt`; thi thử dùng quyền `uc312_ExamMain` hoặc quyền quản lý đề.
BUS kiểm tra lại quyền trước khi lưu câu tự luận. Các truy vấn và cập nhật mới đều qua EF6.

## Thêm câu tự luận

- Mở nhóm câu hỏi → **新增申論題**; nhập nội dung bắt buộc, tối đa 2.048 ký tự.
- Có thể nhập **參考答案（不計分）**, tối đa 16.000 ký tự, để người dùng tự đối chiếu sau thi thử.
- Chuột phải câu tự luận → **編輯申論題** để sửa; **查看題目** để xem.
- Tự luận không tạo đáp án trắc nghiệm, không tham gia chấm đúng/sai.
- Quy trình nhập Excel trắc nghiệm cũ được giữ nguyên; tự luận được thêm qua dialog mới.

## Thi và thi thử

- Tại màn hình cá nhân module 312, nút **模擬測驗** mở bài thi thử không cần được phân một kỳ thi chính thức.
- Dùng cấu hình số câu, thời lượng và quy tắc phân bổ theo nhóm hiện có; bỏ các nhóm đã ngừng sử dụng.
  Số câu cấu hình là tổng số câu gồm cả trắc nghiệm và tự luận.
- Tự luận hiển thị ô nhập nhiều dòng riêng. Đổi câu bằng nút điều hướng hoặc nhấp vào danh sách vẫn giữ câu trả lời.
  Nộp bài hoặc hết giờ đều ghi nhận câu đang làm trước khi tính điểm.
- Điểm bằng số câu trắc nghiệm đúng / tổng số câu trắc nghiệm × 100, làm tròn như trước.
  Tự luận không nằm trong mẫu số và không bị coi là đáp án sai.
- Bài toàn tự luận không có điểm và không kết luận đạt/trượt.
- Sau khi nộp thi thử, màn hình kết quả hiển thị điểm, câu đúng/sai, đáp án đã chọn và đáp án đúng;
  tự luận hiển thị câu đã viết và đáp án tham khảo nếu có.
- Đáp án và đáp án tham khảo chỉ hiển thị khi xem kết quả, không hiển thị lúc đang làm bài.
- Thi thử không ghi vào `dt312_ExamUser`, không thay đổi điểm, trạng thái hoặc lượt thi chính thức.
- Thi chính thức vẫn lưu kết quả và toàn bộ câu trả lời (kể cả tự luận) trong JSON `ExamData`.
  Màn hình xem chi tiết cá nhân/quản trị đã hỗ trợ cả bài cũ và bài có tự luận.

## Kiểm tra

Build Debug/AnyCPU rồi chạy:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/Exam312/Run-Tests.ps1
```

Kiểm thử không kết nối database ứng dụng: kiểm tra metadata/mapping EF, chấm trắc nghiệm (đúng/sai/bỏ trống/nhiều đáp án),
bài toàn tự luận, lưu/khôi phục câu trả lời khi chuyển câu và nộp, thi thử không đọc/ghi bài chính thức dù có ID,
hiển thị điểm/đáp án sau nộp, mã hóa HTML, JSON lưu tự luận và bố cục dialog.
Ảnh chụp giao diện bằng dữ liệu giả, kết quả và trang tổng hợp `index.html` được lưu ở
`%TEMP%/7system-Exam312-TestArtifacts`. Chụp trực tiếp màn hình vì `DrawToBitmap` không chụp được
nội dung WebBrowser dù DOM đã hiển thị đầy đủ.

Kiểm thử tích hợp SQL Server:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/Exam312/Run-IntegrationTests.ps1
```

Runner lấy kết nối Debug (hoặc `EXAM312_TEST_SERVER_CONNECTION`), tạo database riêng có tên
`7system_Exam312_Test_<guid>`, dựng cấu trúc cũ tối thiểu, chạy migration hai lần và kiểm thử UI/BUS/EF.
Connection string của tiến trình test chỉ trỏ vào DB tạm; không sửa database ứng dụng.
DB tạm được xóa trong `finally`, kể cả test thất bại; tên DB được kiểm tra trước khi xóa.

Các ca tích hợp bao gồm quyền quản lý/thi thử/không có quyền, chức năng tên gần giống,
thêm và sửa thực tế từ dialog, giới hạn độ dài, nhóm dừng dùng/sai nhóm,
đề đủ/thiếu câu, thiếu đáp án, cấu hình không hợp lệ, làm thi thử và xem điểm/đáp án,
không sửa kết quả chính thức, hết giờ, hủy nộp, đổi câu bằng bàn phím và danh sách,
nộp chính thức lưu JSON/điểm/trạng thái, từ chối bài của người khác và bài đã nộp,
thất bại khi nộp vẫn giữ phiên và chạy lại đồng hồ, bài dài hơn 60 phút, bài toàn tự luận.
Ngoài ra chạy 100 tình huống phân bổ câu hỏi với số lượng/cỡ nhóm khác nhau.

Lỗi phát hiện và đã sửa trong kiểm thử: so khớp tên chức năng quá rộng có thể cấp nhầm quyền;
đồng hồ không chạy tiếp khi nộp lỗi; hiển thị phút quay vòng khi thời lượng hơn 60 phút.
Database triển khai thực tế vẫn chưa được cập nhật; cần chạy migration trước khi phát hành.
