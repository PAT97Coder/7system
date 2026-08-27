# 7System — Hệ thống quản lý tài liệu và nghiệp vụ nội bộ

Ứng dụng Windows desktop phục vụ quản lý tài liệu, nhân sự, quy trình ký duyệt và các nghiệp vụ nội bộ. Giao diện chính sử dụng DevExpress WinForms; dữ liệu được truy cập qua Entity Framework 6 và SQL Server.

## Công nghệ chính

- C# / Windows Forms
- .NET Framework 4.7.2
- DevExpress 22.1.3
- Entity Framework 6.5.1, Database First (`ModelDocManager.edmx`)
- SQL Server
- Visual Studio 2022
- Advanced Installer cho các project đóng gói cài đặt

Ứng dụng có nội dung giao diện bằng tiếng Hoa phồn thể và tiếng Việt. Khi sửa file Designer hoặc resource, cần giữ đúng encoding để tránh biến nội dung tiếng Hoa thành ký tự `?`.

## Cấu trúc solution

| Thư mục/project | Vai trò |
|---|---|
| `KnowledgeSystem` | Ứng dụng WinForms chính và các màn hình nghiệp vụ |
| `BusinessLayer` | Business logic và lớp truy cập dữ liệu theo từng module |
| `DataAccessLayer` | Entity Framework model, entity và database context |
| `Logger` | Ghi log dùng chung |
| `SVC207Knowledge` | Service cho module kho tri thức |
| `SVC301SafetyCert` | Service cho module chứng chỉ an toàn |
| `NotesMail` | Xử lý/gửi thông báo Notes Mail |
| `ConsoleTestService` | Công cụ console hỗ trợ kiểm tra service |
| `DBScripts` | Migration, seed data và tài liệu thiết kế database |
| `SetupDocumentSystem`, `SetupDEVProject` | Project đóng gói bằng Advanced Installer |

Luồng phụ thuộc chính:

```text
KnowledgeSystem
├── BusinessLayer
│   └── DataAccessLayer
└── Logger
```

## Yêu cầu môi trường

Trước khi build, máy phát triển cần có:

1. Windows và Visual Studio 2022 với workload **.NET desktop development**.
2. .NET Framework 4.7.2 Developer Pack.
3. DevExpress 22.1.3 WinForms được cài và các assembly có thể được MSBuild resolve.
4. Quyền truy cập SQL Server của môi trường phát triển.
5. Advanced Installer nếu cần build hai project `Setup*` hoặc build toàn bộ solution.

Nếu không có Advanced Installer, hãy build trực tiếp project ứng dụng thay vì build toàn solution.

## Thiết lập lần đầu

### 1. Lấy source và restore package

```powershell
git clone <repository-url>
cd 7system
nuget restore KnowledgeSystem.sln
```

Có thể dùng chức năng **Restore NuGet Packages** của Visual Studio nếu `nuget.exe` không nằm trong `PATH`.

### 2. Cấu hình database

Connection string Entity Framework có tên:

```text
DBDocumentManagementSystemEntities
```

Cấu hình giá trị phù hợp với môi trường local trong `KnowledgeSystem/App.config` và các service liên quan. Không commit địa chỉ máy chủ, tài khoản hoặc mật khẩu thật. Nếu thông tin đăng nhập đã từng được đưa vào Git, cần thay/rotate credential thay vì chỉ xóa ở commit mới.

### 3. Chạy migration

Các script nằm trong `DBScripts` và được đặt tên theo ngày. Trước khi chạy:

- sao lưu database;
- xác nhận đúng database đích;
- đọc script để kiểm tra dependency và dữ liệu cập nhật;
- chạy theo thứ tự thời gian đối với các script phụ thuộc nhau;
- ghi nhận script đã áp dụng theo quy trình triển khai của đội.

Project hiện không có migration runner tự động. Không giả định rằng việc chạy ứng dụng sẽ tự nâng cấp schema.

### 4. Build ứng dụng

Trong **Developer PowerShell for Visual Studio**:

```powershell
msbuild KnowledgeSystem\KnowledgeSystem.csproj /t:Build /p:Configuration=Debug /m
```

Hoặc mở `KnowledgeSystem.sln`, đặt `KnowledgeSystem` làm Startup Project và build cấu hình `Debug | Any CPU`.

Build `Release` của một số project sử dụng target `x64`; cần kiểm tra lại platform trước khi đóng gói.

## Quy ước phát triển

- Tạo feature branch từ `main`; không phát triển trực tiếp trên `main`.
- Giữ thay đổi theo đúng module và tránh ghi đè file Designer ngoài phạm vi cần sửa.
- Thêm file `.cs`, `.Designer.cs`, `.resx` vào project `.csproj` vì solution đang dùng định dạng project cũ.
- Thay đổi schema phải có script idempotent trong `DBScripts` khi có thể.
- Không xóa cứng dữ liệu danh mục đã được dữ liệu nghiệp vụ tham chiếu; ưu tiên trạng thái hoạt động/ngừng dùng.
- Trước khi commit, chạy build phù hợp và `git diff --check`.

## Quy ước danh mục bộ phận

Danh mục bộ phận dùng entity `dm_Departments` và cờ `IsActive`:

- `dm_DeptBUS.GetList()` trả về toàn bộ bộ phận. Dùng cho màn hình quản trị, báo cáo, dữ liệu lịch sử và lookup phục vụ join.
- `dm_DeptBUS.GetActiveList()` chỉ trả về bộ phận đang hoạt động. Dùng cho combobox hoặc thao tác tạo nghiệp vụ mới.
- Khi sửa dữ liệu cũ, nếu bộ phận hiện tại đã ngừng dùng thì form cần bổ sung lại đúng bộ phận đó vào datasource và ghi rõ `（停用）`. Không đưa các bộ phận inactive khác vào danh sách chọn.
- Ngừng dùng bộ phận không được làm mất tên bộ phận trên báo cáo hoặc bản ghi lịch sử.

Các script liên quan:

```text
DBScripts/20260514_add_department_active_flag.sql
DBScripts/20260515_add_department_manage_menu.sql
```

## Kiểm tra trước khi tạo pull request

- [ ] Working tree không chứa file tạm, output build hoặc credential.
- [ ] Project chính build được trên máy có đủ DevExpress.
- [ ] Script SQL chạy đúng trên database thử nghiệm và có thể chạy lại an toàn nếu được thiết kế idempotent.
- [ ] Form tạo mới chỉ hiển thị danh mục active.
- [ ] Form xem/sửa và báo cáo vẫn hiển thị được dữ liệu inactive trong lịch sử.
- [ ] Nội dung tiếng Hoa/tiếng Việt không bị lỗi encoding.
- [ ] Pull request mô tả migration, ảnh hưởng dữ liệu và cách rollback.

## Giới hạn kiểm thử hiện tại

Solution chưa có project unit test tự động rõ ràng. Với thay đổi nghiệp vụ, tối thiểu cần kiểm tra thủ công:

1. mở màn hình và tải dữ liệu;
2. tạo, xem và sửa bản ghi;
3. kiểm tra quyền truy cập theo nhóm/bộ phận;
4. kiểm tra dữ liệu lịch sử sau khi danh mục bị ngừng dùng;
5. kiểm tra export/report nếu thay đổi lookup hoặc phép join.

Các thay đổi lớn ở BusinessLayer nên bổ sung test tự động hoặc một test harness có thể chạy lặp lại trong các lần phát triển sau.
