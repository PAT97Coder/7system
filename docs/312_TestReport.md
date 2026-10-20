# Kết quả kiểm thử module 312 — 20/10/2026

Phạm vi: nhánh `feature/312-essay-practice-exam`, thêm câu tự luận và thi thử.

## Kết quả

- Build Debug/AnyCPU và Release/AnyCPU: thành công; còn cảnh báo sẵn có ở các module khác.
- Bộ kiểm thử UI, logic và tích hợp SQL Server: **276 điều kiện kiểm tra đạt**,
  bao gồm 100 tình huống phân bổ câu hỏi với cỡ nhóm và số câu khác nhau.
- Migration chạy hai lần trên cấu trúc cũ trong DB tạm, giữ nguyên câu trắc nghiệm và đáp án cũ.
- Thêm/sửa tự luận từ dialog, lưu/đọc thực tế qua EF6; kiểm tra nội dung bắt buộc,
  độ dài biên, nhóm bị ngừng dùng/không tồn tại, sửa sai nhóm và quyền không hợp lệ.
- Thi thử hỗn hợp và toàn tự luận; chuyển câu bằng nút/phím/danh sách; giữ câu đang viết;
  nộp/hủy nộp/hết giờ; điểm và đáp án đúng/sai/đáp án tham khảo sau nộp.
- Thi thử không thay đổi bất kỳ bản ghi bài thi chính thức nào trong DB test.
- Thi chính thức lưu điểm/đạt/timestamp/JSON tự luận; từ chối bài của người khác,
  bài đã nộp; bài toàn tự luận có điểm và đạt/trượt là null.
- Từ chối cấu hình số câu/thời lượng/điểm đạt không hợp lệ và đề trắc nghiệm thiếu đáp án.
- Xem lại snapshot JSON cũ, không lộ đáp án trước khi nộp, mã hóa nội dung HTML.

## Lỗi tìm thấy và đã sửa

1. Kiểm tra quyền dùng `Contains` khiến tên chức năng gần giống được nhận nhầm.
   Đã đổi sang so khớp đúng control hoặc tên đầy đủ kết thúc bằng tên control.
2. Đồng hồ dừng khi nộp thất bại. Đã tiếp tục đồng hồ nếu phiên chưa kết thúc và còn thời gian.
3. Phút hiển thị quay vòng khi bài dài hơn một giờ. Đã hiển thị tổng số phút còn lại.
4. Cách chụp `DrawToBitmap` tạo ảnh WebBrowser trắng dù nội dung đã render.
   Bộ test đã chuyển sang chụp màn hình thật với WebBrowser; control thông thường giữ chụp bitmap.

## Bằng chứng

Mở `%TEMP%/7system-Exam312-TestArtifacts/index.html` để xem ảnh và kết quả:

- Danh sách quản lý câu hỏi và nút thi thử.
- Thêm/sửa câu tự luận đã lưu thực tế.
- Màn hình thi thử tự luận và trắc nghiệm.
- Kết quả thi thử 50/100, câu sai/đáp án đúng, phần tự luận không tính điểm.
- Bài chính thức 100/100 đã lưu và bài toàn tự luận không tính điểm.

## Giới hạn

Test SQL dùng database tạm `7system_Exam312_Test_<guid>` với cấu trúc tối thiểu theo EDMX
và dữ liệu giả, được xóa sau mỗi lượt chạy (kể cả khi thất bại). Không chạy migration hoặc
sửa dữ liệu database ứng dụng. Chưa kiểm tra trên mọi máy trạm, mức DPI và dữ liệu thực tế;
database triển khai vẫn cần chạy migration trước khi dùng bản ứng dụng mới.

Chạy lại bằng `Tests/Exam312/Run-IntegrationTests.ps1`; cần quyền tạo/xóa DB test.
