# Module 209 — Lỗi kế thừa và đề xuất để xem xét

Module 209 bắt đầu từ code 201. Các sửa lỗi dưới đây chỉ áp dụng cho 209;
module 201 giữ nguyên. Đợt 1 đã xử lý các mục 1, 2, 5 và 8.

## Lỗi đã tái hiện và sửa trong đợt 1

1. **Đã sửa kết quả xóa mềm.** `dt209_FormsBUS.Remove` chỉ gọi `SaveChanges()` một lần.
   Trả `true` sau khi lưu thành công; trả `false` khi bản ghi không tồn tại hoặc đã xóa.
   Test xác nhận kết quả, thời gian xóa và việc loại khỏi danh sách đang xử lý.
2. **Đã sửa tên danh mục thiếu dòng.** Dialog thêm node chấp nhận tên một dòng,
   chuỗi rỗng hoặc null. Tên hai dòng vẫn theo thứ tự Việt/Trung; tên một dòng hiển thị
   ở cột Trung, thiếu tên Trung thì dùng tên Việt, thiếu cả hai thì dùng mã hồ sơ.
   Chưa thay đổi schema danh mục.

## Phát hiện qua đọc code, cần xác nhận kịch bản trước khi sửa

3. **Copy nhánh không giữ đủ cây nhiều cấp.** `uc209_AuditISOMain.ItemClone_Click` chỉ
   thêm node cha đầu tiên và các node `IsFinalNode=true`, gán tất cả node cuối về cha mới;
   bỏ qua node trung gian. Đề xuất ánh xạ Id cũ/mới và copy theo cấp trong transaction.
4. **Lưu metadata và copy file chưa có cơ chế nhất quán.** `HandleCreateEvent`/
   `HandleUpdateEvent` ở `f209_AddAttachment` lưu attachment, form, workflow bằng các
   lần lưu riêng; nhánh file thường đặt `result=true` sau copy mà không ràng buộc kết quả
   tạo form. Có thể để lại metadata/file mồ côi nếu một bước lỗi. Đề xuất transaction EF6
   cho metadata, xác minh Id/kết quả từng bước, xử lý bù khi copy file lỗi.
5. **Đã sửa trong đợt 1: kiểm tra trạng thái màn hình ký.** Sắp xếp bước theo Id,
   đối chiếu lịch sử tuần tự trước khi lấy bước tiếp theo, hỗ trợ một người ký nhiều bước.
   Khóa thao tác và nêu nguyên nhân trên tiêu đề khi tài liệu không tồn tại, đã xóa,
   đã trả lại/kết thúc, hết bước, thiếu lịch sử/người ký/vai trò/đính kèm hoặc sai lượt ký.
   Kiểm tra lại trạng thái khi bắt đầu thao tác để chặn dialog đã cũ.
   Tính nguyên tử của cập nhật quy trình vẫn thuộc mục 4, chưa xử lý trong đợt này.
6. **Kiểm tra quyền thay đổi tập trung ở UI.** BUS sao chép từ 201 chưa nhận danh tính
   người thao tác để tự kiểm tra quyền; một số dialog chỉ kiểm tra helper sửa khi ở chế
   độ Update. Đề xuất rà soát toàn bộ Create/Update/Delete và kiểm tra lại trong service
   nghiệp vụ, bao gồm phạm vi bộ phận.
7. **Trigger cây kế thừa các giả định về dữ liệu.** So sánh `deleted.Flag <> inserted.Flag`
   không bắt được chuyển từ NULL; một số nhánh UPDATE dựa trên `EXISTS` toàn batch và
   áp dụng cho toàn bộ `inserted`. CTE đệ quy chưa xử lý chu trình và giới hạn độ sâu.
   Đề xuất kiểm thử cập nhật nhiều dòng và chuẩn hóa NULL/điều kiện riêng từng dòng.
8. **Đã sửa trong đợt 1: ID bản ghi dùng Int32.** Rà soát đọc ID trong danh sách
   hồ sơ, tiến độ ký, ký số và quá hạn. Có test thao tác mở lịch sử với ID 40000.
   Trường chu kỳ thông báo không phải ID, không thuộc thay đổi này.

## Các lựa chọn nghiệp vụ nên xem xét

- Phần hiệu chuẩn, watermark riêng bộ phận `7820` và gửi Note hiệu chuẩn đã được gỡ
  khỏi 209. Font/bố cục và cách phân quyền `二級{IdDept}` được giữ theo 201. Cần quyết định các
  quy tắc này có phù hợp ISO14001 không trước khi thay đổi.
- Danh mục hồ sơ/điều khoản ISO14001 cần bộ cấu hình đã duyệt.
  Không gắn nhãn các điều khoản ISO17025 thành ISO14001 tự động. Có script copy danh
  mục tùy chọn nếu cần bản nháp, tách khỏi migration chính.
- Chuyển bộ phận/nghỉ việc/đổi việc: đã xóa toàn bộ màn hình, BUS, entity, EDMX và các
  điểm phát sinh tự động của nhân sự 209. DB `.153` đã dọn ba bảng cùng trigger nhân sự;
  kiểm tra trước khi dọn xác nhận không có hồ sơ/chi tiết nhân sự, chỉ có 47 cấu hình mẫu
  từ 201 (đã sao lưu vào thư mục tạm). Các script cài mới không tạo lại phần này.
  Nhân sự 201 giữ nguyên.
- Hạ tầng mẫu quy trình cố định dùng chung Id vai trò giữa 201/209/306; hiện giữ nguyên
  cơ chế gộp theo Id của dự án. Nếu cần vai trò ISO14001 khác nghĩa, cần thiết kế phạm vi
  mẫu quy trình thay vì thay nhãn cùng Id trong một bảng riêng lẻ.
