# 7System

7System là hệ thống quản lý tài liệu và nghiệp vụ nội bộ được xây dựng dưới dạng ứng dụng Windows desktop. Hệ thống tập trung các chức năng quản lý tài liệu, nhân sự, quy trình phê duyệt và nhiều nghiệp vụ vận hành vào một nền tảng thống nhất.

Giao diện hệ thống sử dụng tiếng Hoa phồn thể và tiếng Việt, phù hợp với môi trường làm việc đa ngôn ngữ.

## Chức năng chính

- Quản lý người dùng, bộ phận, chức vụ, nhóm và phân quyền.
- Quản lý tài liệu ISO, tiêu chuẩn kỹ thuật và kho tri thức.
- Quản lý quy trình ký duyệt và thông báo.
- Quản lý đào tạo, chứng chỉ an toàn và kiểm tra kiến thức.
- Quản lý nhân sự mới, lịch làm việc và sức khỏe nhân viên.
- Quản lý vật tư, phụ tùng, máy móc và tài sản cố định.
- Hỗ trợ các nghiệp vụ EHS, đánh giá nhân viên và thống kê báo cáo.
- Tích hợp các service xử lý dữ liệu và gửi thông báo.

## Công nghệ sử dụng

- C# và Windows Forms
- .NET Framework 4.7.2
- DevExpress WinForms 22.1
- Entity Framework 6
- SQL Server
- Visual Studio 2022

## Kiến trúc tổng quan

Solution được tổ chức theo mô hình phân lớp:

```text
KnowledgeSystem    Giao diện và các module nghiệp vụ
       │
BusinessLayer      Xử lý nghiệp vụ và truy cập dữ liệu
       │
DataAccessLayer    Entity Framework model và database context
```

Các project service và tiện ích sử dụng chung `BusinessLayer`, `DataAccessLayer` và `Logger` tùy theo chức năng.

## Cấu trúc project

| Project/thư mục | Mô tả |
|---|---|
| `KnowledgeSystem` | Ứng dụng WinForms chính |
| `BusinessLayer` | Business logic của hệ thống |
| `DataAccessLayer` | Entity Framework model và các entity |
| `Logger` | Thành phần ghi log dùng chung |
| `SVC207Knowledge` | Service hỗ trợ kho tri thức |
| `SVC301SafetyCert` | Service hỗ trợ chứng chỉ an toàn |
| `NotesMail` | Xử lý thông báo qua Notes Mail |
| `ConsoleTestService` | Công cụ console hỗ trợ kiểm tra service |
| `DBScripts` | Script database và tài liệu thiết kế dữ liệu |
| `SetupDocumentSystem` | Project đóng gói bộ cài chính thức |
| `SetupDEVProject` | Project đóng gói cho môi trường phát triển |

## Bắt đầu phát triển

### Yêu cầu

- Windows
- Visual Studio 2022 với workload .NET desktop development
- .NET Framework 4.7.2 Developer Pack
- DevExpress WinForms 22.1
- Kết nối đến SQL Server của môi trường phát triển

### Mở và chạy project

1. Clone repository.
2. Mở `KnowledgeSystem.sln` bằng Visual Studio.
3. Restore các NuGet package.
4. Cấu hình connection string cho môi trường phát triển.
5. Đặt `KnowledgeSystem` làm Startup Project.
6. Build và chạy cấu hình `Debug | Any CPU`.

Các project `Setup*` yêu cầu Advanced Installer. Nếu chỉ phát triển ứng dụng, có thể build trực tiếp project `KnowledgeSystem` mà không cần build project đóng gói.

## Database

Hệ thống sử dụng SQL Server và Entity Framework theo hướng Database First. Các thay đổi database được lưu trong thư mục `DBScripts`.

Trước khi chạy script cần kiểm tra đúng môi trường, sao lưu dữ liệu và đọc nội dung script để xác định thứ tự cũng như phạm vi ảnh hưởng.

## Phát triển và đóng góp

- Tạo feature branch từ `main` cho mỗi thay đổi.
- Giữ code theo đúng module và kiến trúc hiện tại.
- Đi kèm script trong `DBScripts` khi thay đổi database.
- Kiểm tra build và các luồng nghiệp vụ liên quan trước khi tạo pull request.
- Không đưa credential, file build hoặc dữ liệu môi trường cá nhân vào commit.
