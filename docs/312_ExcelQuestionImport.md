# Nhập trắc nghiệm / tự luận từ Excel — 03/11/2026

Mở nhóm câu hỏi 312 → tải lên `.xlsx` → xem trước từng câu → đến câu cuối và chọn
`確認匯入`. Chọn `取消` hoặc đóng preview thì không lưu dữ liệu/ảnh.

Giữ mẫu cũ A–G và sheet đầu tiên. Hai cột bổ sung là:

- H `題型`: `選擇題` / `申論題`. File cũ thiếu H–I hoặc H trống vẫn là trắc nghiệm.
- I `參考答案`: tham khảo tự luận, không bắt buộc, tối đa 16000 ký tự.

Trắc nghiệm tiếp tục nhập mỗi lựa chọn một dòng; A là số câu, B là nội dung, C là ảnh
câu hỏi, D là số thứ tự lựa chọn, E là chữ của lựa chọn, F là ảnh lựa chọn, G là 1/0.
Thứ tự đáp án vẫn theo thứ tự dòng như code cũ. H có thể ghi ở dòng đầu của câu.
Nếu có nhiều lựa chọn đúng, đánh dấu câu là `IsmultiAns`.

Tự luận mỗi câu một dòng: A/B bắt buộc, C là tên ảnh nếu có, D–G để trống,
H ghi `申論題`, I là tham khảo. Không tạo bản ghi `dt312_Answers` cho tự luận.

Ảnh đặt chung thư mục `Images` cạnh Excel, cột C/F chỉ ghi tên file (có phần mở rộng
nếu tên file thực có). Các tên ảnh cũ không có phần mở rộng vẫn được hỗ trợ.
Code không đọc ảnh dán trực tiếp trong Excel. Tất cả ảnh sau nhập lưu vào `Folder312`
với tên mới để tránh ghi đè, kể cả ảnh đáp án trắc nghiệm; không còn chép nhầm `Folder307`.

Mỗi câu được giữ cùng danh sách đáp án riêng và gán `QuesId` theo Id EF vừa sinh;
không sửa/mượn số câu Excel làm Id database, không yêu cầu số câu liên tục từ 1.
BUS kiểm tra quyền quản lý nhóm câu hỏi và nhóm còn hoạt động; lưu trong transaction.
Khi lưu giữa chừng thất bại, rollback SQL và dọn ảnh mới của lần nhập đó.

Giữ cách thêm/sửa tự luận thủ công. Khi sửa nội dung/tham khảo, ảnh đã nhập được giữ.
`查看題目` dùng `f312_ExamReview` để thấy cả ảnh và tham khảo tự luận; tham khảo chỉ
hiện trong preview/quản lý/kết quả, không hiện trong câu hỏi lúc đang làm bài.

Không đổi SQL, model bảng, logic tạo đề/chấm điểm hoặc form xác nhận của module 307.
File Excel gốc và bản mẫu bổ sung không bị code sửa.

## File chính

- `Exam312QuestionExcel.cs`: đọc sheet đầu bằng ExcelDataReader, kiểm tra A–I và ảnh.
- `dt312_QuestionImportItem.cs`: nhóm câu hỏi/đáp án theo số Excel.
- `dt312_QuestionsBUS.ImportQuestions`: lưu EF và ảnh, kiểm tra quyền, ánh xạ Id.
- `f312_QuesMgmt.cs` / Designer: nối nút tải lên và quyền thao tác.
- `f312_ExamReview.cs` / Designer: dùng lại form xem bài, thêm chế độ preview/xác nhận nhập.
- `Tests/Exam312/ExcelImportTests.cs`: kiểm tra tương thích và nhập thực trên DB tạm.

## Kiểm chứng

Build Debug/AnyCPU thành công; toàn bộ bộ test cũ/mới đạt **386 điều kiện** trên SQL
LocalDB tạm. Bao gồm mẫu 7/9 cột, tự luận có ảnh, nhiều loại câu trộn chung, toàn tự luận,
số câu không liên tục, Unicode/nhiều dòng, ảnh không có phần mở rộng, sai loại/thiếu ảnh,
preview DOM chứa ảnh/tham khảo, hủy không lưu, ánh xạ đúng đáp án, rollback khi SQL lỗi
và sửa thủ công không mất ảnh.

Đọc thử hai file thật `附件：考試題目.xlsx` và `附件：考試題目_含申論題.xlsx`: đều nhận đúng
4 câu trắc nghiệm / 16 đáp án hiện có. Không nhập hai file này vào database ứng dụng.
Ảnh screenshot WebBrowser chỉ chụp bố cục ở chế độ layout-only; nội dung HTML và ảnh
được kiểm tra bằng DOM. Chưa kiểm chứng ghi ảnh lên mọi máy trạm/đường mạng thực tế.
