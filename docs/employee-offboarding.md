# Hồ sơ nghỉ việc

Nhánh: `feature/employee-offboarding`, tách từ `main`.

## Sử dụng

- Sau khi lưu thao tác nghỉ việc thành công, cửa sổ tải tài liệu được mở.
- Tại danh sách nhân viên, dùng bộ lọc trên thanh lệnh: `在職人員` (đang làm), `已離職人員` (đã nghỉ), `全部人員` (tất cả). Nhấp chuột phải đúng nhân viên và chọn `上傳離職文件` để bổ sung hoặc `離職紀錄` để tra cứu.
- Hai dialog riêng: `f401_OffboardingUpload` nhập lý do/file bằng cửa sổ `FixedToolWindow`; `f401_OffboardingDocuments` xem lịch sử, có thể co giãn. Tham khảo trực tiếp `f201_AddNode.Designer.cs` và `f201_UpdateUsrReq_Detail.Designer.cs`: thanh lệnh toàn hàng, SVG 32, font khả dụng 14.25/disabled 12, nền 244, caption/editor 14.25, padding ngoài 10/item 2, cách caption 4. Cả hai hiển thị nhân viên chỉ đọc và dùng layout gốc.
- Tải lên chỉ có lý do và danh sách file, không phân loại văn kiện. Lý do bắt buộc, tối đa 2.000 ký tự; chọn ít nhất một file, mỗi file tối đa 100 MB và không được rỗng.
- Mỗi lần tải lên là một bản ghi riêng có lý do, người tải, thời gian, ngày nghỉ và thông tin nhân viên tại thời điểm tải. Các bản ghi cũ vẫn giữ khi nhân viên phục chức hoặc nghỉ lần tiếp theo.
- Trong lịch sử, chọn bản ghi để xem lý do và file. Menu chuột phải của file có xem trước PDF/PNG/JPG/JPEG và tải xuống. Định dạng khác chỉ tải xuống; không mở file thực thi từ ứng dụng.
- Có thể bỏ file khỏi danh sách chờ trước khi lưu. Phiên bản này không sửa/xóa tài liệu đã lưu để giữ nguyên lịch sử.

## Triển khai

1. Chạy `Database/20261104_EmployeeOffboarding.sql` trên database đích trước khi triển khai bản build. Script tạo hai bảng; không thay đổi trạng thái nhân viên.
2. Bảo đảm kho `FolderData\401\Offboarding` truy cập được từ các máy sử dụng ứng dụng. Có thể cấu hình khóa `EmployeeOffboardingFolder` trong `sys_StaticValue` để dùng vị trí riêng; giá trị phải là đường dẫn đầy đủ, nên dùng UNC cho kho dùng chung. Không cấu hình đường dẫn theo máy người dùng.
3. Sao lưu database và kho file cùng nhau. Kho file tạo tên lưu ngẫu nhiên; tên gốc được giữ trong metadata, tên trùng không ghi đè lẫn nhau.
4. Với database đã triển khai ba quyền riêng, chạy `Database/20261114_remove_offboarding_permissions.sql` để xóa các mục này và liên kết vai trò tương ứng.

Hồ sơ nghỉ việc không phân quyền riêng. Người đã vào được màn hình quản lý nhân viên có thể xem, xem trước, tải lên và tải xuống hồ sơ của nhân viên trong danh sách hiện có. Chỉ tải lên cho nhân viên đã nghỉ hoặc có dự báo nghỉ.

Metadata dùng EF6 của `DataAccessLayer` hiện có; file được ghi vào kho qua service. Ghi file hoàn tất trước khi mở transaction để tránh giữ khóa database trong lúc sao chép. Nếu tải lên thất bại trước khi commit, transaction rollback và dọn những file do lần tải đó tạo. Nếu bổ sung tài liệu bị hủy, trạng thái nghỉ việc đã lưu vẫn được giữ; bổ sung lại từ danh sách nhân viên.

## Kiểm tra

Chạy từ thư mục repository:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/Test-EmployeeOffboarding.ps1
```

Kiểm tra tự động không mở kết nối database:

- Mapping EF và sinh truy vấn; dữ liệu bắt buộc và độ dài lý do.
- Sao chép file thực tế: hai file trùng tên khác nội dung, loại bỏ file nguồn chọn trùng, nội dung tải xuống, lỗi file bị khóa giữa chừng, dọn batch chưa commit, giữ batch hoàn tất, không làm hỏng file đích khi sao chép lỗi, chặn đường dẫn ra ngoài kho, file rỗng và quá 100 MB.
- Bộ lọc nhân viên và giữ lại nhân viên đang chọn sau khi dữ liệu đổi thứ tự; caption menu grid/editor phồn thể; đối chiếu font bar trực tiếp với form ISO; kiểu cửa sổ, padding, màu nền, SVG và `CaptionGlyph`.
- Bố cục ở kích thước mặc định/tối thiểu và khi co giãn; lưu chỉ khi có lý do/file; khóa khi đang lưu; nhận diện thay đổi chỉ nhập lý do.

Ảnh render hai dialog ở `tmp/offboarding-smoke`.

Kiểm tra tích hợp trên môi trường thử nghiệm sau khi áp dụng script:

1. Xử lý nghỉ việc, nhập lý do, tải ít nhất hai file trùng tên từ hai thư mục khác nhau. Mở lại từ máy khác và xác nhận nội dung đúng.
2. Bổ sung một lần nữa và kiểm tra cả hai bản ghi cùng người tải, thời gian, lý do. Xác nhận lịch sử cũ còn sau khi phục chức.
3. Kiểm tra người vào được màn hình có thể xem, xem trước, tải lên và tải xuống; chỉ cho tải lên khi nhân viên đã nghỉ hoặc có dự báo nghỉ.
4. Ngắt quyền ghi kho file khi lưu để xác nhận thông báo lỗi, không có bản ghi/file lưu dở. Khôi phục kết nối và thử lại.

Chưa thực hiện kiểm tra tích hợp ghi dữ liệu trên database thật trong lượt triển khai mã nguồn này.
