# Module 209 — Lỗi kế thừa và đề xuất để xem xét

Giữ nguyên code và giao diện 201 sau khi đổi định danh module. Các mục dưới đây chưa
được sửa nghiệp vụ trong 209, theo yêu cầu ghi chú để xem xét trước. Cũng tồn tại ở 201.

## Lỗi đã tái hiện khi kiểm thử

1. **Xóa file báo thất bại dù đã xóa mềm.** `dt209_FormsBUS.Remove` gọi `SaveChanges()`
   hai lần; lần thứ hai không còn thay đổi nên trả `false`. Test tái hiện xác nhận bản ghi
   có `IsDel=true` nhưng kết quả hàm là `false`. Đề xuất chỉ lưu một lần và trả kết quả đó.
2. **Tên danh mục hồ sơ một dòng có thể làm dialog thêm node lỗi.** Trong
   `f209_AddNode_Load`, `DisplayName.Split(...)` được truy cập `[1]` mà không kiểm tra
   số dòng. Khi dùng tên chỉ có một dòng trong fixture ban đầu, dialog không tải được.
   Regression test đã tái hiện `IndexOutOfRangeException`. Dữ liệu hợp lệ hiện phải là
   tên Việt + xuống dòng + tên Trung. Đề xuất fallback khi
   thiếu một ngôn ngữ hoặc tách hai cột tên rõ ràng.

## Phát hiện qua đọc code, cần xác nhận kịch bản trước khi sửa

3. **Copy nhánh không giữ đủ cây nhiều cấp.** `uc209_AuditISOMain.ItemClone_Click` chỉ
   thêm node cha đầu tiên và các node `IsFinalNode=true`, gán tất cả node cuối về cha mới;
   bỏ qua node trung gian. Đề xuất ánh xạ Id cũ/mới và copy theo cấp trong transaction.
4. **Lưu metadata và copy file chưa có cơ chế nhất quán.** `HandleCreateEvent`/
   `HandleUpdateEvent` ở `f209_AddAttachment` lưu attachment, form, workflow bằng các
   lần lưu riêng; nhánh file thường đặt `result=true` sau copy mà không ràng buộc kết quả
   tạo form. Có thể để lại metadata/file mồ côi nếu một bước lỗi. Đề xuất transaction EF6
   cho metadata, xác minh Id/kết quả từng bước, xử lý bù khi copy file lỗi.
5. **Màn hình ký giả định luôn tồn tại bước tiếp theo.** `f209_SignDoc_Info_Load` truy
   cập `progress[stepNow + 1]` và `First(...)` trên danh sách bước. Workflow thiếu/hỏng
   dữ liệu hoặc đã hoàn tất có thể gây lỗi chỉ số/không tìm thấy. Đề xuất kiểm tra trạng
   thái trước khi truy cập, hiển thị thông báo cụ thể và khóa thao tác không phù hợp.
6. **Kiểm tra quyền thay đổi tập trung ở UI.** BUS sao chép từ 201 chưa nhận danh tính
   người thao tác để tự kiểm tra quyền; một số dialog chỉ kiểm tra helper sửa khi ở chế
   độ Update. Đề xuất rà soát toàn bộ Create/Update/Delete và kiểm tra lại trong service
   nghiệp vụ, bao gồm phạm vi bộ phận.
7. **Trigger cây kế thừa các giả định về dữ liệu.** So sánh `deleted.Flag <> inserted.Flag`
   không bắt được chuyển từ NULL; một số nhánh UPDATE dựa trên `EXISTS` toàn batch và
   áp dụng cho toàn bộ `inserted`. CTE đệ quy chưa xử lý chu trình và giới hạn độ sâu.
   Đề xuất kiểm thử cập nhật nhiều dòng và chuẩn hóa NULL/điều kiện riêng từng dòng.
8. **Id bản ghi được đọc bằng Int16 ở một số màn hình nguồn.** `Convert.ToInt16`
   không phù hợp khi khóa DB là `int`. Bản ghi vượt 32767 có thể gây overflow.
   Đề xuất dùng `Int32` nhất quán.

## Các lựa chọn nghiệp vụ nên xem xét

- Phần hiệu chuẩn, watermark riêng bộ phận `7820` và gửi Note hiệu chuẩn đã được gỡ
  khỏi 209. Font/bố cục và cách phân quyền `二級{IdDept}` được giữ theo 201. Cần quyết định các
  quy tắc này có phù hợp ISO14001 không trước khi thay đổi.
- Danh mục hồ sơ/điều khoản và checklist nhân sự ISO14001 cần bộ cấu hình đã duyệt.
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
