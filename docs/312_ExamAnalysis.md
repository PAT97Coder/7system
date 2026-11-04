# Phân tích nhóm câu hỏi sai — module 312

Chức năng mới: **錯題分析**, UserControl **uc312_ExamAnalysis**.
UC độc lập, mở trong vùng nội dung `f00_FluentFrame`; không ghép vào thi thật,
thi thử hoặc thống kê 317. Không có script schema/seed, không tự thêm menu/cấp quyền.

## Đăng ký element và cấp quyền

Sau khi build và mở lại ứng dụng:

1. Mở quản lý chức năng/quyền `uc402_Functions`, chọn **新增功能**.
2. Điền:

   | Trường | Giá trị |
   |---|---|
   | Id | Để hệ thống tạo theo cơ chế hiện có |
   | Root | Chọn đúng nhóm cha chứa `uc312_ExamMain`, `uc312_GroupMgmt`, `uc312_ExamMgmt`, `uc312ExamTest` (hiện giao diện của bạn đặt tên `thi can day`) |
   | Function | `錯題分析` |
   | Control | `uc312_ExamAnalysis` — chọn tên ngắn chính xác trong danh sách |
   | Priority | Giá trị lớn nhất của các chức năng con hiện có + 1; nếu `thi thu` đang là 4 và không có mục khác phía sau thì chọn 5 |
   | Image | `Statistics.svg` trong catalog SVG hiện có |

3. Nhấn **確認** để lưu chức năng.
4. Mở role cần cấp quyền trong `uc402_Functions` → sửa role → đánh dấu chức năng
   **錯題分析**, giữ nguyên các quyền khác. Bảo đảm role/nhân viên đã có quyền nhóm
   cha để khung Accordion hiển thị được nhánh này. Nhấn **確認**.
5. Gán role đó cho nhân viên bằng cơ chế quản lý role/người dùng hiện có nếu chưa gán.
6. Đăng xuất/đăng nhập lại để làm mới `AppPermission`; mở lại module 312.

Chưa đăng ký hoặc chưa có quyền: UC không tải kết quả, các bộ lọc bị vô hiệu hóa
và hiển thị hướng dẫn cấp quyền. Quyền thi thật/thi thử không tự cấp quyền phân tích.
BUS kiểm tra quyền lại với mỗi yêu cầu đọc danh sách/báo cáo. Không hardcode ID.

## Sử dụng

| Phạm vi | Nhân viên | Nội dung |
|---|---|---|
| 全部歷史 | 全部人員 | Mọi bài thi thật đã nộp của mọi người |
| 全部歷史 | Một nhân viên | Mọi bài thi thật đã nộp của người đó |
| 單次考試 + kỳ thi | 全部人員 | Mọi người trong kỳ đã chọn |
| 單次考試 + kỳ thi | Một nhân viên | Người đã chọn trong kỳ đó |

Danh sách nhân viên tìm bằng mã/tên; chọn kỳ thi sẽ giới hạn danh sách nhân viên
theo các bản ghi phân công của kỳ. Lựa chọn người không còn hợp lệ được trả về
**全部人員**. **刷新** tải lại danh mục và kết quả.
Danh sách kỳ thi dùng SearchLookUpEdit như bộ chọn nhân viên, có ô tìm kiếm theo
tên, chỉ hiển thị cột **考試名稱**. Hai popup tự căn chiều rộng theo nội dung, chiều
cao theo số dòng (tối đa 10 dòng rồi cuộn); tiêu đề căn giữa, dữ liệu và giá trị
đang chọn căn trái. Mã kỳ thi dùng nội bộ để chọn đúng dữ liệu, không hiển thị thành cột.

Bảng ở trên; biểu đồ cột đứng ở dưới; kéo đường chia để đổi chiều cao.
Giao diện theo `uc312_GroupMgmt`: thanh lệnh dock trên cùng, nút 刷新 trước các
bộ lọc, SVG 32px/caption 14.25pt; LayoutControl có padding gốc 10px và khoảng cách
item 2px. Grid có header căn giữa, dòng xen kẽ màu, chiều rộng cột cố định,
footer tổng lượt câu/sai và navigator chỉ đọc. Thông báo tải/lỗi nằm gọn ở cuối UC.
Tên nhóm nằm trên trục ngang; tỷ lệ sai trên trục dọc 0–100%, vạch chia 20%.
Nhãn phần trăm nằm ở đầu cột; nhóm sai nhiều nhất mặc định ở bên trái.
Biểu đồ dùng đúng các dòng đang hiển thị trong bảng, gồm lọc nhanh và thứ tự
sắp xếp. Mặc định sai nhiều nhất ở đầu, cùng tỷ lệ thì ưu tiên nhiều lượt câu hơn,
sau đó GroupId. Khi nhiều nhóm, biểu đồ hiển thị cửa sổ 8 nhóm và có thanh cuộn
để xem các nhóm còn lại, không cắt Top N. Tooltip giữ đầy đủ tên, tỷ lệ và sai/tổng.

Các cột: nhóm; số người khác nhau; số bài khác nhau đóng góp câu hợp lệ;
tổng lượt câu; số lượt sai; tỷ lệ sai. Trục biểu đồ 0–100%, nhãn hai chữ số thập phân.

## Quy tắc và giới hạn

- Chỉ đọc `dt312_ExamUser` có `SubmitAt`; không đọc kết quả thi thử hoặc module khác.
- Không giới hạn năm, không chọn riêng bài mới nhất/cao nhất; mỗi lần xuất hiện
  của câu hỏi trong một bài thi được tính một lượt.
- Tỷ lệ = tổng lượt sai / tổng lượt trắc nghiệm hợp lệ × 100. Ví dụ 2/5 + 3/10 +
  1/5 cho kết quả 6/20 = 30%; không trung bình các tỷ lệ bài/nhân viên.
- Bỏ tự luận khỏi cả tử và mẫu; trắc nghiệm bỏ trống là sai theo kết quả chấm.
- Dùng `GroupId` và `IsCorrect` từ snapshot bài thi, không đọc lại ngân hàng câu hỏi.
- JSON cũ thiếu `IsEssay` được hiểu là trắc nghiệm; thiếu hẳn `IsCorrect` thì chỉ
  tính lại bằng `dt312_ExamScoring.IsCorrect` khi đủ khóa đáp án và trường bài làm.
  `false` đã lưu được giữ nguyên; null/kiểu dữ liệu không hợp lệ không được coi là thiếu.
- Bài JSON hỏng/rỗng và câu thiếu thông tin bị bỏ qua, có bộ đếm trên màn hình và
  `TraceWarning` theo Id bài; không ghi lại JSON/điểm. Bài toàn tự luận hợp lệ không
  tính là bài lỗi và không tạo tỷ lệ giả.
- Tên nhóm lấy theo danh mục hiện tại, giữ cả nhóm ngừng sử dụng; không có danh mục
  thì hiển thị **未知群組（ID：...）**. Đổi tên nhóm sẽ đổi nhãn lịch sử, mã nhóm và số liệu
  vẫn lấy từ snapshot. Nhóm trùng tên vẫn tách theo Id.
- Người không còn hồ sơ vẫn giữ theo UserId. Tên ưu tiên DisplayName → DisplayNameVN → UserId.
- DTO dùng tỷ lệ dạng 0–100; Grid và Chart không nhân thêm 100.
- Đọc EF6/LINQ, AsNoTracking, lọc SQL trước khi tải JSON; không sửa GetList cũ.
  BUS không phụ thuộc class UI. Tải nền có số phiên yêu cầu để bỏ kết quả cũ khi
  thay đổi bộ lọc/đóng UC. Mỗi tác vụ dùng DbContext riêng.

## Kiểm chứng

Build Debug/AnyCPU vào `%TEMP%/7system-practice-history-build`.
Bản triển khai này đã build thành công và đạt **560 điều kiện kiểm tra** của bộ
module 312 (bao gồm chức năng mới). Kiểm tra truy cập control khác luồng được bật;
kết quả tải nền chỉ cập nhật UI qua `BeginInvoke`, bỏ yêu cầu cũ theo số phiên.
Bộ kiểm thử chung module 312 có thêm `Tests/Exam312/ExamAnalysisTests.cs`;
chạy trên LocalDB tạm theo `Tests/Exam312/Run-IntegrationTests.ps1`, tự xóa database.
Các bản ghi chức năng/role trong kiểm thử chỉ được tạo trong database tạm.

Kiểm tra bốn phạm vi, tổng lượt/không chọn bài mới nhất, nhiều lần gặp cùng câu,
tự luận/bỏ trống, snapshot không còn câu trong ngân hàng, nhóm ngừng sử dụng/không
có danh mục, người không có hồ sơ, JSON lỗi/cũ, quyền riêng, lọc/sắp xếp bảng và
biểu đồ, nhiều nhóm, đổi bộ lọc nhanh và không sửa dữ liệu thi gốc.
Ảnh giao diện: `%TEMP%/7system-Exam312-TestArtifacts/Exam312-Analysis*.png`.

Chưa kiểm chứng hiệu năng trên toàn bộ lịch sử database thật hoặc mọi mức DPI.
Việc tạo element và cấp quyền trong database thật do người quản trị thực hiện
bằng màn hình 402 theo hướng dẫn trên. Không commit/push tự động.
