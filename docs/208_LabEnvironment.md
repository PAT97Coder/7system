# Module 208 — Quản lý nhiệt độ và độ ẩm phòng thí nghiệm

Module 208 nằm trong khu vực menu ISO, control `uc208_LabEnvironment`, nằm trong
`Views/02_StandardsAndTechs/08_LabEnvironment`. Dữ liệu do app riêng gửi vào SQL Server;
7system không nhập hay sửa số đo gốc.

## Cài đặt

1. Build `KnowledgeSystem` bằng cấu hình tương ứng DB cần cài.
2. Chạy `DBScripts/20261013_add_208_lab_environment.sql` trên DB đã chọn.
   Script hỗ trợ SQL Server 2012 trở lên và có thể chạy lại. Không đổi cấu trúc bảng cũ.
3. Vào màn hình quản lý nhóm hiện có, thêm thành viên vào nhóm module 208 đúng bộ phận.
   Script cài đặt tạo ba nhóm theo các bộ phận đang có nhóm ISO và một nhóm quản trị `*`.
   Có thể dùng script `DBScripts/20261013_grant_208_lab_environment_permissions.sql`
   để cấp nhóm bằng SQL nếu cần; không bắt buộc.
   Người dùng cần có ít nhất một role hệ thống để mở menu. Nếu menu cha chưa được cấp,
   quản trị viên cấp quyền menu cha theo quy trình hiện có.
4. Đăng xuất/đăng nhập lại, mở khu vực **ISO → 溫濕度管理 → 溫濕度監控**.
5. Đăng ký thiết bị và ngưỡng trước khi app bắt đầu gửi số đo để có cấu hình hiệu lực.

Không tự chuyển người dùng ISO thành người có quyền sửa module mới.
Trong màn hình quản lý nhóm hiện có có thể thêm/bỏ thành viên các nhóm sau:

| Nhóm | IdDept | Quyền |
|---|---|---|
| 溫濕度組 | Mã bộ phận | Xem thiết bị, số đo, lịch sử, xuất báo cáo |
| 溫濕度【編輯】 | Mã bộ phận | Quản lý thiết bị/ngưỡng và xem |
| 溫濕度【處理】 | Mã bộ phận | Ghi nhận xử lý số đo vượt giới hạn và xem |
| 溫濕度【管理】 | * | Tất cả bộ phận và danh sách SN chưa đăng ký |

Khi chuyển thiết bị, người sửa cần quyền EDIT ở cả bộ phận cũ và mới.
Mỗi lần lưu cấu hình có kiểm tra phiên bản để tránh ghi đè thay đổi của người khác.

## Bảng dữ liệu

| Bảng | Nội dung |
|---|---|
| dt208_Devices | SN duy nhất, tên và model hiện tại |
| dt208_DeviceConfigs | Lịch sử tên/model, bộ phận, vị trí, người quản lý, ngưỡng, chu kỳ, trạng thái, người và lý do thay đổi |
| dt208_Readings | Số đo gốc; duy nhất theo SN + MeasuredAt |
| dt208_ReadingConflicts | Giá trị khác được gửi lại tại cùng SN/thời điểm |
| dt208_ReadingActions | Nhật ký nguyên nhân, biện pháp, kết quả và trạng thái xử lý |

BUS nằm trong `BusinessLayer/dt208`, dùng truy vấn có tham số và cùng cấu hình kết nối
của DataAccessLayer. Các bảng mới chưa thuộc EDMX; không chỉnh thủ công các file EF sinh tự động.
`dt208_DeviceEdit` là DTO dùng cho form lưu thiết bị.

## Hợp đồng cho app thu thập số đo

Tất cả thời gian của module là **giờ Việt Nam, UTC+07:00**, lưu `datetime2(3)`.
App phải chuyển thời gian UTC hoặc múi giờ khác về UTC+07 trước khi gửi; không gửi chuỗi
ngày theo locale. Dùng tham số SQL `datetime2`, `decimal`, `nvarchar`.

App gọi duy nhất:

```sql
EXEC dbo.usp208_IngestReading
    @SN = N'LAB-001',
    @MeasuredAt = '2026-10-13T08:00:00.000', -- giờ Việt Nam
    @Temperature = 20.25,
    @Humidity = 61.50;
```

| Tham số | Kiểu | Quy tắc |
|---|---|---|
| SN | nvarchar(100) | Bắt buộc; trim và chuyển chữ hoa; giống SN đã đăng ký |
| MeasuredAt | datetime2(3) | Thời điểm thực tế đo; giữ nguyên khi gửi lại/gửi bù |
| Temperature | decimal(8,2) | °C, tối đa hai chữ số thập phân |
| Humidity | decimal(5,2) | %RH, 0–100, tối đa hai chữ số thập phân |

`ReceivedAt` được DB tự ghi. Thời điểm đo quá hơn giờ DB + 5 phút bị từ chối để phát hiện lệch đồng hồ.
App cần giữ độ chính xác thời gian đến millisecond nếu đo nhiều lần mỗi giây.

Procedure trả một dòng gồm `ReadingId`, `Result`:

- `Accepted`: đã lưu số đo mới.
- `Duplicate`: đã có cùng số đo; coi là gửi thành công, không cần gửi lại.
- `Conflict`: SN/thời điểm đã có giá trị khác; lưu bản mâu thuẫn, không ghi đè bản gốc.

SN chưa đăng ký vẫn được nhận và xuất hiện trong danh sách **未登記SN** của quản trị viên.
Khi đăng ký sau, các số đo trước thời điểm cấu hình được hiển thị là **chưa có cấu hình hiệu lực**;
không tự đánh giá chúng bằng ngưỡng mới. Việc đánh giá lại/quay lùi cấu hình không nằm trong bản này.

Tạo login riêng cho app bằng quy trình quản trị SQL hiện có rồi cấp role:

```sql
-- Chạy bởi DBA; thay tên bằng tài khoản app thực tế.
-- CREATE USER [AppThuThapNhietAm] FOR LOGIN [AppThuThapNhietAm];
-- ALTER ROLE lab_environment_ingest ADD MEMBER [AppThuThapNhietAm];
```

Role chỉ được EXECUTE procedure nhận số đo, không được ghi trực tiếp bảng.
Không dùng tài khoản SQL quản trị hoặc tài khoản của ứng dụng desktop cho app thu thập.

## Sử dụng

- Thanh bar chọn bộ phận và trạng thái; cây **vị trí → thiết bị**. Làm mới tự động mỗi 60 giây
  khi màn hình đang hiển thị hoặc bấm **刷新**. **資料逾時** được tính theo thời điểm đo, không phải lúc nhận.
- **新增設備** trên thanh lệnh; **編輯設備** trong menu chuột phải của thiết bị: nhập SN, người quản lý, vị trí, khoảng dưới/trên hoặc trung tâm ± dung sai,
  chu kỳ và trạng thái. SN không sửa sau khi tạo. Lý do thay đổi bắt buộc.
- **設定紀錄** trong menu chuột phải: lịch sử cấu hình. Cấu hình mới có hiệu lực khi lưu theo giờ DB.
  Màn hình chính đợi số đo thuộc cấu hình hiện tại để không trộn ngưỡng cũ và mới.
- **歷史與圖表** trong menu chuột phải: chọn ngày, xem bảng và biểu đồ có giới hạn áp dụng tại từng lần đo.
  Tối đa 10.000 bản ghi/lần truy vấn; vượt mức cần giảm khoảng ngày. Mặc định bật **僅月表取樣**,
  chỉ hiện hai số đo/ngày được chọn cho biểu tháng (gần 08:00/16:00 nhất trong ±30 phút).
  Bỏ chọn để xem mọi số đo. Bộ lọc này và **僅超限** chỉ lọc bảng;
  biểu đồ vẫn hiển thị toàn bộ số đo trong khoảng đã chọn.
- **處理超限** chỉ xuất hiện trên số đo vượt giới hạn được chọn cho biểu tháng: ghi nguyên nhân,
  biện pháp và kết quả; kết quả bắt buộc và có xác nhận khi kết án.
  Nhật ký xử lý được thêm mới, không ghi đè các lần xử lý trước.
  **取樣超限處理** trong menu chuột phải thiết bị mở danh sách lấy mẫu để chọn đúng số đo,
  không mở thẳng phiếu xử lý của số đo mới nhất (có thể nằm ngoài giờ lấy mẫu).
  Mục này chỉ hiện cho người có quyền HANDLE trong bộ phận của số đo.
  Cột **超限原因填寫** hiển thị **待填原因** hoặc **已填原因** cho mẫu vượt giới hạn;
  mọi điểm khác hiển thị **無須填寫**, kể cả điểm bất thường gần mốc lấy mẫu nhưng không được chọn lên biểu.
  Lịch sử xử lý đã có vẫn giữ nguyên. Không bắt nhập lý do cho các điểm ngoài hai mẫu của biểu tháng.
- Rê chuột trên biểu đồ nhiệt độ/độ ẩm để xem thời gian đo đầy đủ `yyyy/MM/dd HH:mm:ss` (UTC+07), số đo và giới hạn.
- **資料衝突**: xem các giá trị app gửi khác số đo gốc tại cùng thời điểm.
- Xuất Excel theo các hàng đang lọc; màn hình lịch sử/nhật ký hỗ trợ xem trước khi in.
- **月檢查表** trên thanh lệnh chính: chọn tháng, phòng (kèm bộ phận) và SN thiết bị để xuất
  biểu `GA-700-B007-03實驗室溫濕度檢查紀錄表-R1.xls`. Danh sách phòng/thiết bị lấy từ
  các bộ phận đang chọn mà người dùng có quyền xem. Thiết bị được lọc lại khi đổi phòng.
  Mỗi ngày một hàng, hai lượt **08:00** và **16:00** theo giờ Việt Nam; lấy số đo gần nhất
  trong **±30 phút**, bao gồm hai đầu. Nếu khoảng cách bằng nhau lấy số đo sớm hơn;
  thiếu dữ liệu thì để trống, ngày không tồn tại trong tháng không điền.
  Chỉ lấy số đo có cấu hình hiệu lực thuộc đúng bộ phận, phòng và SN đã chọn tại thời điểm đo.
  File giữ bố cục, nội dung song ngữ và font của mẫu gốc, chỉnh vùng in sheet đầu vừa một trang;
  không xuất sheet ví dụ cũ.
  Cột giờ ghi giờ đo thực tế. Ô người kiểm tra và ký duyệt để trống cho người thực hiện điền.
  Số đo vượt giới hạn tô đỏ, đánh dấu lượt 1/2 trong cột ghi chú; sheet **量測明細**
  ghi đầy đủ giờ dự kiến/thực tế, ngưỡng hiệu lực, nguyên nhân, biện pháp, kết quả và người xử lý.
  Nếu ngưỡng đổi trong tháng, tiêu đề dẫn chiếu sheet chi tiết thay vì dùng ngưỡng hiện tại cho cả tháng.
  Mẫu được nhúng vào ứng dụng, không phụ thuộc đường dẫn máy phát triển hay cài thêm Excel.
  Truy vấn tháng vượt 10.000 bản ghi được chia theo ngày; nếu một ngày vẫn vượt giới hạn
  thì báo lỗi để tránh xuất dữ liệu bị cắt.

Giao diện 208 dùng BarManager và LayoutControl theo kiểu ISO. Thanh lệnh dùng SVG 32×32,
font Microsoft JhengHei UI 14.25; dòng dữ liệu và menu dùng cỡ 12. Menu chuột phải giữ
các mục mặc định, dùng localizer zh-Hant chung và chọn đúng dòng dưới con trỏ trước
khi hiển thị thao tác. Quyền sửa và xử lý được kiểm tra trước khi bật nút/menu;
dialog không có quyền bị khóa thao tác lưu.

Thanh lệnh dùng cùng cấu hình với `uc201_AuditISOMain`: nút thêm → **刷新** → **出表**
→ chọn **實驗室**, phân nhóm bằng vạch ngăn, chiếm toàn hàng, không có viền kéo,
chữ hover màu xanh. Font disabled kế thừa Segoe UI 12 của bar ISO theo yêu cầu
đồng nhất giao diện. TreeList dùng tiêu đề đen căn giữa 14.25, dữ liệu 12,
chiều cao dòng mặc định, dòng xen kẽ và hiệu ứng hover như ISO.
UI test đối chiếu các thuộc tính này trực tiếp với control ISO.

Dialog 208 dùng mẫu `f201_AddNode`: nền RGB(244,244,244), bar xác nhận/hủy,
font bar/editor/caption/dropdown 14.25, disabled 12, caption đen kể cả khi disabled,
padding ngoài 10 và layout item 2. Dialog nhập liệu dùng FixedToolWindow;
lịch sử và danh sách phụ vẫn cho phép co giãn. Các trường ngắn của dialog thiết bị
được ghép theo hai cột; ô lý do giới hạn chiều cao 80–120 để không chiếm phần lớn
cửa sổ. UI test mở dialog bằng dữ liệu giả, kiểm tra lề và các editor không bị cắt,
đồng thời lưu ảnh `Lab208-DeviceDialog.png` và `Lab208-ActionDialog.png` trong thư mục tạm.

Giới hạn là khoảng đóng: bằng min/max vẫn đạt. Độ ẩm và nhiệt độ được đánh giá riêng.
Thiết bị ngừng sử dụng vẫn giữ lịch sử. Khi chuyển bộ phận, dữ liệu và cấu hình cũ chỉ hiện
cho bộ phận có quyền tại thời điểm đo, không lộ lịch sử cho bộ phận mới.

## Kiểm tra

Build Debug rồi chạy:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/LabEnvironment/Run-IntegrationTests.ps1
```

Runner dùng kết nối Debug hoặc biến môi trường `LAB208_TEST_SERVER_CONNECTION`, tạo một DB
riêng tên `7system_Lab208_Test_<guid>`, chạy migration hai lần và kiểm thử, rồi xóa đúng DB tạm
trong `finally`. Tài khoản cần quyền tạo/xóa DB test; không chạy migration vào DB ứng dụng.

Kiểm tra bao gồm quyền bộ phận, hai đầu giới hạn, thiếu/quá hạn dữ liệu, dữ liệu gửi bù,
gửi trùng/mâu thuẫn/đồng thời, lịch sử đổi cấu hình/bộ phận, kiểm tra phiên bản sửa,
nhật ký xử lý, SN chưa đăng ký và quyền SQL của app nhận số đo.

Kiểm tra giao diện bằng dữ liệu giả, không ghi vào DB:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Tests/LabEnvironment/Run-UiTests.ps1
```

Thông báo ngoài ứng dụng, quản lý hiệu chuẩn/chứng thư và xuất biểu đồ vào báo cáo là phần bổ sung sau.
