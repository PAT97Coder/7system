# Build bộ cài 7system

Nhấp đúp `Build-Installer.bat` ở thư mục gốc, hoặc chạy:

```powershell
.\Installer\Build-Installer.ps1
.\Installer\Build-Installer.ps1 -OpenOutput
```

Script dùng project Advanced Installer hiện có tại `SetupDocumentSystem/SetupDocumentSystem.aip`, build `KnowledgeSystem` và các thư viện ở `Release | x64` vào `Installer/Staging`, rồi đóng gói thành `SetupDocumentSystem.exe`. Output riêng tránh lỗi khóa file khi ứng dụng đang chạy từ thư mục bin. Tên file cố định, không thêm hậu tố version hoặc DEV. Build tiếp theo ghi đè file bộ cài cùng tên.

Script chuyển cả đường dẫn output của `BusinessLayer`, `DataAccessLayer` và `Logger` sang staging. Nếu project installer còn tham chiếu thư mục `bin/Debug`, script dừng để tránh đóng gói DLL Debug vào bộ cài Release. Kết nối DB hiện lấy từ `DataAccessLayer.SingleConnection`: bản Release dùng `.103`, Debug dùng `.153`; giá trị trong `App.config` không quyết định kết nối của context này.

Mặc định file nằm tại `E:\01. DEV\01.7System\04. Setup files\SetupDocumentSystem.exe`. Có thể đổi thư mục bằng `-OutputDirectory 'D:\Setup'`.

Version ứng dụng có dạng `yy.MM.dd.index`, tính theo ngày Việt Nam. Ngày mới bắt đầu index 1, build tiếp trong ngày tăng index. Có thể chỉ định `-BuildIndex 5` nếu index này lớn hơn lần trước. Trạng thái lưu trong `Installer/version.json` sau khi build thành công; version cũng được đồng bộ vào `AssemblyInfo.cs` và project installer.

Khi khởi động, `AppCopyRight.Version` đọc trực tiếp `AssemblyFileVersion`; không cần sửa version trong `Program.cs`. Version hiển thị, ngày phát hành và kiểm tra cập nhật dùng cùng giá trị này, kể cả khi Debug. Ngày phát hành lấy từ ba phần đầu, còn phần cuối phân biệt các build trong ngày.

Khi phát hành bộ cài, cập nhật trường `version` trong JSON tại `TPConfigs.UrlUpdate` theo đúng version ứng dụng, ví dụ `26.10.12.2`. Không dùng version MSI `26.10.1202` hoặc chỉ ngày `26.10.12`: ứng dụng đang yêu cầu chuỗi version khớp chính xác. Script build không tự cập nhật dữ liệu trên server; chỉ công bố version sau khi bộ cài tương ứng đã sẵn sàng tải về.

MSI dùng `yy.MM.(dd*100+index)` để các build cùng ngày nâng cấp đúng thứ tự. Ví dụ version ứng dụng `26.10.12.1` tương ứng MSI `26.10.1201`. Các cấu hình cài đặt và UpgradeCode được giữ từ project cũ.

Kiểm tra version dự kiến, công cụ và đường dẫn mà không build hay thay đổi file:

```powershell
.\Installer\Build-Installer.ps1 -Preview
```

Cần Visual Studio/Build Tools có MSBuild, .NET Framework 4.7.2, các dependency của ứng dụng và Advanced Installer có license phù hợp. Có thể đặt `ADVANCED_INSTALLER_PATH` tới `AdvancedInstaller.com`.

Thư mục `SetupDocumentSystem` hiện bị bỏ qua bởi Git. Máy build cần có sẵn project `.aip` đó; script không tạo project installer mới.
