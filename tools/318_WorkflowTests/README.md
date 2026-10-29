# Kiểm thử nghiệp vụ 318

Yêu cầu: Windows, Visual Studio 2022/MSBuild, Python, SQL Server LocalDB 2019,
`sqlcmd`, các dependency hiện có của solution.

Từ thư mục gốc repository:

```powershell
python tools/318_WorkflowTests/run.py
```

Runner build ứng dụng, tạo/start instance LocalDB **Codex318WorkflowValidation**,
tạo lại duy nhất database **Codex318WorkflowTests** trong instance đó, dựng fixture
từ EDMX và chạy executable kiểm thử riêng. Không đọc connection string của ứng dụng;
config của executable test luôn trỏ LocalDB và được kiểm tra trước khi chạy.
Config ứng dụng thật không bị thay đổi.

Các trường hợp gồm: mặc định tổ người nhận, phân biệt người tạo/người nhận,
bổ sung Án cũ mà giữ 是/否, quyền xác nhận, chỉnh phần lưu trình chưa xử lý,
khóa bước đã xác nhận, chuyển bước đúng thứ tự và đúng nhóm, một/hai tổ,
điều kiện kết Án, từ chối token cũ, thao tác đồng thời, nhập trùng nguồn,
sao chép file và rollback nhập thất bại, dữ liệu danh sách, khởi tạo giao diện,
đủ năm cột sau khi grid khởi tạo, thêm nhân viên bằng dòng mới, tự điền chức vụ,
nạp lưu trình cố định và bảo toàn phần đã xác nhận khi đổi mẫu,
đối chiếu kích thước/font/thanh nút với form ký tên, form không có tab,
mở đúng một tổ và chuyển bước bằng form một trang,
thanh nút `設定`, form ba đường dẫn, cấu hình cũ dự phòng, lưu và nạp lại đường
dẫn trong file thử riêng, từ chối thư mục trùng/đường dẫn tương đối,
và bảo đảm hàng đợi Notes không thay đổi.

Runner in đường dẫn artifact chứa `fixture.sql`, `build.log` và thư mục file thử.
Ảnh giao diện kiểm chứng được lưu ở `Input/simple-workflow.png`.
Ảnh form thiết lập đường dẫn ở `Input/path-settings.png`; các kiểm tra thiết lập
chỉ ghi vào thư mục artifact, không thay đổi file cấu hình đường dẫn thật.
Executable/config test tạm trong `KnowledgeSystem/bin/Debug` được xóa sau khi chạy.
LocalDB test được giữ lại để có thể kiểm tra dữ liệu; lần chạy sau tạo lại database
test. Có thể dọn riêng instance test khi không sử dụng, không dùng lệnh dọn cho
instance/database của ứng dụng.
