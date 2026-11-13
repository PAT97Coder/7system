# Quản lý thiết bị 310

Chức năng mở từ menu **310 → 設備管理**. Hồ sơ thiết bị gồm mã duy nhất, tên Việt/Trung, bộ phận quản lý, người phụ trách lấy từ danh sách nhân viên và ghi chú. Nhóm `安衛環7` quản lý toàn bộ; nhóm `安衛環{mã bộ phận}` quản lý đúng bộ phận tương ứng.

## Sử dụng

- Thêm, xem, sửa và xóa mềm thiết bị. Người phụ trách mới phải đang làm việc.
- Thanh công cụ giữ thêm mới, tải lại và lọc chờ bàn giao. Các thao tác theo thiết bị nằm trong menu chuột phải như phần 機邊庫. Bật **啟用多選** trong menu nếu cần chọn nhiều thiết bị.
- Chọn một hoặc nhiều thiết bị, chuột phải chọn **列印設備標籤** để xem trước/in mẫu `rpDeviceManagement` hiện có, khoảng 99 × 52 mm. Nội dung lấy lại từ dữ liệu hiện tại khi in.
- Menu **交接管理人** đổi người phụ trách cho các thiết bị được chọn sau khi xác nhận. Cả lô lưu trong một giao dịch; nếu một thiết bị bị đổi người/bộ phận trong lúc thao tác, cả lô không được lưu.
- Khi sửa/xóa, bản gốc được đối chiếu với dữ liệu hiện tại để chặn ghi đè thay đổi từ phiên khác. Lưu thất bại giữ nguyên bản gốc để có thể sửa thông tin hoặc tải lại.
- Menu **交接紀錄** xem lịch sử chỉ đọc của dòng đang bấm chuột phải: người cũ, người mới, bộ phận cũ/mới, người thực hiện, thời gian và số hồ sơ bàn giao (nếu có). Lịch sử bắt đầu được ghi từ bản này; không dựng lại các bàn giao trước đây.
- **待交接設備** bật/tắt lọc các trường hợp chưa có người hợp lệ, đã nghỉ, dự báo nghỉ hoặc quản lý khác bộ phận. Trường hợp khác bộ phận cần được kiểm tra, không tự động coi là sai.

## Nghỉ việc và chuyển bộ phận

Khi cập nhật nghỉ việc/dự báo nghỉ bằng chức năng nhân sự hiện có, hồ sơ 310 có thêm tab **設備交接**. Nhân viên An vệ hoàn chọn người tiếp nhận từng thiết bị rồi trình duyệt theo các cấp hiện có. Bản xem trước hồ sơ có bảng thiết bị, người cũ và người nhận. Thiết bị vẫn thuộc người cũ trong lúc chờ duyệt; chỉ cập nhật sau bước duyệt cuối.

Khi chuyển bộ phận một người đang giữ thiết bị trong màn hình nhân sự, hệ thống tạo hồ sơ bàn giao cho An vệ hoàn của bộ phận cũ. Nếu người đó đang có hồ sơ 310 chưa kết thúc thì dùng hồ sơ hiện có, tránh tạo trùng khi dự báo nghỉ chuyển thành nghỉ chính thức. Luồng tự động này gắn với cập nhật nhân sự trong ứng dụng; thay đổi trực tiếp ở DB không tạo hồ sơ.

Khi duyệt cuối, thiết bị, các trách nhiệm 310 trong cùng hồ sơ, lịch sử và trạng thái hoàn thành được lưu trong một giao dịch. Nếu người nhận đã nghỉ, thiết bị bị xóa/đổi chủ/đổi bộ phận, hoặc danh sách thiết bị thay đổi trong lúc chờ, hệ thống từ chối hoàn tất và yêu cầu tải lại hoặc trả hồ sơ để chọn lại. Hồ sơ JSON cũ vẫn đọc được; nếu người nghỉ vẫn có thiết bị nhưng hồ sơ cũ không chứa thiết bị, cần trả lại hồ sơ để bổ sung.

## Triển khai

1. Sao lưu DB, kiểm tra bản script `DBScripts/20261107_complete_310_equipment_management.sql`, rồi chạy trên DB ứng dụng trước khi dùng bản mới.
2. Script tạo bảng thiết bị nếu chưa có, bảng lịch sử, unique index cho mã thiết bị còn hiệu lực, view chỉ lấy thiết bị chưa xóa và menu thiết bị. Quyền xem menu được sao chép từ các chức năng 310 cùng cấp. Script có thể chạy lại; nếu dữ liệu cũ trùng mã, script dừng để xử lý thủ công, không xóa dữ liệu.
3. Build/triển khai ứng dụng cùng file tài nguyên `310_updateleaveruser.html` mới. Đăng nhập lại để tải menu.

## Kiểm tra

Chạy từ PowerShell: `powershell -NoProfile -ExecutionPolicy Bypass -File tools/Test310EquipmentManagement.ps1` (chỉ áp dụng execution policy cho tiến trình test). Cần Visual Studio, bộ tham chiếu DevExpress và các dependency hiện tại của dự án. Script build vào `tmp/310-equipment-check`, kiểm tra tab thiết bị/chọn người nhận, trạng thái/chọn nhiều thiết bị, tương thích JSON cũ, chặn mã thiết bị trùng trong lô và render hồ sơ có/không có thiết bị. Test xuất hai tem mẫu ra `tmp/310-equipment-label-smoke.pdf`, không kết nối hay sửa DB ứng dụng.

Test cũng mở cửa sổ lịch sử thật bằng vòng lặp sự kiện WinForms với ba trường hợp: danh sách rỗng, có bản ghi và dữ liệu null. Kiểm tra bảy cột, số dòng, chế độ chỉ đọc, người nhận và định dạng ngày giờ sau khi cửa sổ đã hiển thị. Đây là regression test cho lỗi truy cập cột trước khi DevExpress khởi tạo cột.

Trước khi phát hành cần kiểm tra trên DB thử nghiệm đã áp dụng script: CRUD và quyền theo bộ phận; bàn giao trực tiếp; nghỉ việc chỉ có thiết bị; hồ sơ trả lại; người nhận nghỉ trong lúc chờ; thiết bị thay đổi giữa lúc chọn và duyệt; bàn giao cuối thành công; lỗi DB phải hoàn tác cả thiết bị, lịch sử và trạng thái hồ sơ. Smoke test giao diện không thay thế kiểm tra giao dịch SQL thực tế.

Kết quả và giới hạn của lần rà soát 2026-11-07 được ghi trong `docs/310-equipment-management-review.md`.
