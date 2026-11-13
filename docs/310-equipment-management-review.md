# Rà soát quản lý thiết bị 310 — 2026-11-07

## Các lỗi đã sửa trong đợt rà soát

1. Form sửa thay đổi trực tiếp bản ghi đã tải trước khi DB lưu thành công. Sau lỗi DB, lần thử lại có thể dùng sai người/bộ phận gốc hoặc ID vừa được cấp trong giao dịch đã hoàn tác. Form hiện tạo bản nhập riêng cho mỗi lần lưu và giữ nguyên bản gốc để đối chiếu.
2. Kiểm tra xung đột trước đây chỉ đối chiếu người quản lý và bộ phận. Bản cũ có thể ghi đè tên/mã/ghi chú vừa được người khác sửa. Sửa và xóa hiện đối chiếu toàn bộ các trường có thể chỉnh sửa; bản ghi bị thay đổi/xóa phải được tải lại.
3. Xóa từ form cũ có thể xóa thiết bị vừa được bàn giao. Xóa hiện kiểm tra bản gốc và quyền trong cùng giao dịch.
4. Khi DB hoặc lookup lỗi, một số thao tác lưu/xóa/in/tải dữ liệu có thể phát sinh exception không được xử lý ở sự kiện giao diện. Các đường thao tác này hiện xử lý lỗi và hiển thị thông báo; lỗi lưu không làm mất bản gốc.
5. Trả hồ sơ từ cửa sổ cũ có thể ghi đè trạng thái sau khi người khác đã xử lý. Trình lên cấp tiếp theo và trả hồ sơ hiện kiểm tra lại quyền, nội dung và trạng thái trong giao dịch, rồi lưu bước duyệt cùng trạng thái hồ sơ.
6. Tạo hồ sơ bàn giao và tạo các bước duyệt trước đây lưu riêng lẻ, không kiểm tra nhóm bị thiếu. Hiện dùng một giao dịch, kiểm tra đủ nhóm và chống tạo trùng hồ sơ còn mở. Nếu nhân sự đã được lưu nhưng không tạo được hồ sơ, ứng dụng thông báo rõ lỗi tạo hồ sơ để quản trị xử lý.
7. Tên thiết bị dài tối đa làm mẫu tem tăng chiều cao và tràn sang nhiều trang. Tem hiện giữ khổ cố định, xuống dòng và co chữ trong ô. Tên cực dài sẽ có chữ nhỏ hơn do giới hạn khổ tem.
8. Giữ tên người quản lý hiện tại khi mở chi tiết, kể cả người đó đã nghỉ/chuyển bộ phận; người nhận mới vẫn phải đang làm việc. Dấu bắt buộc không bị lặp khi chuyển chế độ form nhiều lần.

Lỗi mở lịch sử do cột chưa khởi tạo đã được giữ trong regression test của đợt trước: khai báo bảy cột rõ ràng trước khi bind dữ liệu.

## Kiểm tra đã thực thi

Chạy `powershell -NoProfile -ExecutionPolicy Bypass -File tools/Test310EquipmentManagement.ps1`.

- Debug build: thành công; không có lỗi biên dịch. Dự án vẫn có các cảnh báo có sẵn.
- 83 assertion của bộ kiểm tra sau khi đồng bộ giao diện: thành công.
- Giao diện được mở thật bằng WinForms event loop với dữ liệu mẫu; không chỉ tạo constructor. Kiểm tra form chi tiết ở chế độ xem/sửa, khóa quyền, dấu bắt buộc, tab bàn giao và việc post lựa chọn người nhận vào đối tượng dữ liệu.
- Grid chính: kiểm tra popup của người xem, người có quyền một bộ phận và quản trị EHS; chọn nhiều dòng; chuột phải dòng đã chọn/chưa chọn; bỏ qua dòng không phải dữ liệu; thanh công cụ và bật/tắt lọc chờ bàn giao.
- Lịch sử: mở cửa sổ thật với danh sách rỗng, có bản ghi và null; kiểm tra cột, số dòng, chế độ chỉ đọc, người nhận và định dạng ngày giờ.
- Quy tắc trước khi truy cập DB: thiếu trường, vượt chiều dài, người nhận rỗng/trùng người cũ, thiết bị trùng trong lô; đối chiếu bản gốc khi bản ghi thay đổi/bị xóa; dữ liệu bản gốc không bị thay đổi khi tạo bản lưu hoặc thử lại.
- JSON cũ và template hồ sơ có/không có thiết bị: đọc/render thành công.
- Tem hai thiết bị: hai trang, đủ tên Việt–Trung, mã và người phụ trách.
- Tem giới hạn dài: một trang, kiểm tra thêm bằng PyMuPDF xác nhận đủ 255 ký tự tên Việt, 255 ký tự tên Trung, 50 ký tự mã và 128 ký tự tên người phụ trách. Không chỉ kiểm tra số trang.
- Script triển khai được parse bằng SQL Server ScriptDom: không có lỗi cú pháp. Đây là kiểm tra cú pháp, không phải thực thi migration.

PDF mẫu và log nằm trong `tmp`, không được đưa vào Git.

## Đồng bộ giao diện

Cửa sổ lịch sử hiện là form riêng `f310_EquipmentHistory` cùng file Designer, lấy mẫu từ `f309_MaterialReplacementChain` của 機邊庫: `LayoutControl`, thanh lệnh đóng có icon/chữ, dòng thông tin thiết bị, cửa sổ `FixedToolWindow`, không có nút minimize/maximize. Grid dùng Microsoft JhengHei UI, tiêu đề 14.25 căn giữa màu đen, dữ liệu 12, hàng xen màu, bộ lọc và thanh điều hướng chỉ đọc. Các cột mặc định vừa cửa sổ ở môi trường test; dữ liệu dài vẫn có thể dùng thanh cuộn ngang như các grid hiện có.

Tab bàn giao thiết bị được bổ sung `MenuManager`, navigator chỉ đọc, tắt co ép chiều rộng cột và bật kiểu đánh dấu dòng giống các tab 310 cùng form. Hộp chọn người quản lý tiếp tục dùng `XtraInputBox` như các trang 機邊庫 hiện có, với font ô chọn và popup đã thiết lập.

Form chi tiết mở giữa cửa sổ cha, tắt minimize/maximize. Popup chọn người quản lý có độ rộng cột và kích thước đủ cho mã nhân viên cùng tên Việt–Trung. Test mở popup thật với danh sách rỗng/có dữ liệu và kiểm tra tên hiển thị sau khi chọn; tab bàn giao được kiểm tra font và navigator cùng các tab bên cạnh.

Test mở cửa sổ lịch sử thật cho dữ liệu rỗng/có dữ liệu/null, kiểm tra font, màu, khung cửa sổ, toolbar và độ rộng cột. Ảnh đã được xem trực tiếp tại `tmp/310-history-ui-empty.png` và `tmp/310-history-ui-populated.png`.

## Giới hạn và kiểm tra còn cần DB thử nghiệm

Máy hiện tại không có SQL Server, LocalDB hay Docker để tạo DB thử nghiệm. Bộ kiểm tra giao diện chủ động không mở DB ứng dụng; không có migration hoặc thay đổi dữ liệu nào được thực hiện trên DB thật trong đợt rà soát.

Các kiểm tra trước DB và đối chiếu bản gốc không xác nhận được giao dịch SQL thực tế. Cần kiểm tra trên DB riêng trước khi phát hành:

1. Chạy migration hai lần, kiểm tra menu/quyền và dữ liệu cũ; mã thiết bị trùng phải làm migration dừng mà không mất dữ liệu.
2. Thêm/sửa/xóa với nhóm đúng bộ phận, sai bộ phận, `安衛環7` và trường hợp quyền bị thu hồi giữa lúc mở và lưu form.
3. Gây lỗi ghi lịch sử để xác nhận thiết bị và lịch sử cùng hoàn tác; thử lưu lại sau lỗi.
4. Hai phiên cùng sửa/xóa/bàn giao một thiết bị; phiên cũ phải bị chặn và không ghi đè dữ liệu mới.
5. Hồ sơ nghỉ/chuyển bộ phận: trước duyệt cuối phải giữ người cũ; sau duyệt cuối thiết bị, trách nhiệm khác, lịch sử, chữ ký và trạng thái cùng lưu. Lỗi ở một phần phải hoàn tác toàn bộ.
6. Người nhận nghỉ hoặc thiết bị đổi chủ/đổi bộ phận/bị xóa trong lúc chờ; hồ sơ cũ chưa có danh sách thiết bị; hồ sơ bị trả lại; hai phiên duyệt/trả cùng hồ sơ.
7. Tạo hồ sơ nhân sự khi thiếu nhóm duyệt hoặc có lỗi tạo bước duyệt: không để lại hồ sơ thiếu bước; ứng dụng báo đúng lỗi.
