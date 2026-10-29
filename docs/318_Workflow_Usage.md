# Module 318: xác nhận tổ và lưu trình

Phần nghiệp vụ dùng C#/LINQ và các bảng hiện tại, không bổ sung FK hoặc xử lý
nghiệp vụ trong SQL Server. Chức năng Notes được hoãn để tích hợp service riêng.

## Cách sử dụng

Nút `設定` ngay sau `刷新` mở form nhỏ có ba ô: `7810下載路徑`,
`7820下載路徑`, `建立檔案路徑`. Nhập đường dẫn hoặc bấm `…` để chọn thư mục,
rồi bấm `確認` để lưu. Đóng form khi chưa xác nhận sẽ không lưu thay đổi.
Đường dẫn mới được dùng từ lần nhập Án tiếp theo và làm thư mục đích mặc định
khi thêm Án bằng tay.

1. Mở module hoặc bấm `刷新`: nhập Án mới từ các thư mục XPS/OXPS đã cấu hình.
   Phần thêm tay cho phép chọn người nhận 7810/7820 và đính kèm; các file được
   sao chép vào thư mục chung và lưu cùng thông tin Án.
2. Người nhận Án được hiển thị ở `原收件人`. Tổ người nhận mặc định **是**;
   tổ còn lại có cả hai ô trống, nghĩa là chưa xác nhận. Mỗi tổ có hai ô 是/否
   và một cột tiến độ/người đang xử lý. Chỉ người phụ trách của tổ đó được chọn.
3. Nhấp đôi dòng hoặc chọn menu `7810 流程／確認` / `7820 流程／確認`.
   Form một trang chỉ mở lưu trình của tổ được chọn, không chia tab. Nhấp đôi
   cột tiến độ mở đúng tổ của cột; nhấp đôi thông tin chung ưu tiên tổ đang đến
   lượt xử lý của người đăng nhập, sau đó là tổ người đó phụ trách.
4. Tổ đã chọn 是 có thể thiết lập lưu trình như phần điện tử ký: bảng chỉ có
   `職務 / 工號 / 名稱 / 權限 / ×`. Dòng đầu điền sẵn người phụ trách.
   Bấm dòng `按一下此處新增流程`, chọn mã nhân viên; chức vụ và tên tự điền.
   Các dòng là thứ tự xử lý, dòng đầu xử lý Án và các dòng sau xác nhận.
5. Nút `固定流程` dùng lại danh sách lưu trình cố định của điện tử ký. Chọn mẫu
   để nạp các nhân viên theo thứ tự, hoặc chọn từng người và bấm × để bỏ dòng
   chưa xác nhận. Quyền trong module này là `確認`. Bấm `確認` ở trên để lưu.
   Mỗi Án–tổ giữ bản lưu trình riêng; sửa mẫu không tự đổi các Án đã lưu.
   Khi đã có bước xác nhận, mẫu được chọn phải có phần đầu khớp người và thứ tự
   đã xác nhận; hệ thống chỉ thay phần còn lại. Các nhóm đã lưu từ giao diện
   trước vẫn được giữ và hiển thị; chọn một nhân viên sẽ thay nhóm ở dòng đó.
6. Người được giao bước hiện tại, hoặc thành viên nhóm được giao, bấm
   `已處理完成` / `確認本步驟`. Hệ thống ghi người, thời gian, ý kiến và chuyển
   tới bước tiếp theo. Hai tổ có tiến độ độc lập.
7. Lịch sử người, thời gian và thay đổi trước/sau vẫn tự động lưu vào CSDL.

## Quy tắc đang áp dụng

- `CanXuLy7810` / `CanXuLy7820` là nguồn duy nhất của lựa chọn 是/否:
  `null` chưa xác nhận, `true` có tham gia, `false` không tham gia.
- Án cũ được bổ sung thông tin tổ khi mở/làm mới module. Không ghi đè 是/否 đã
  lưu. Chỉ suy người nhận khi có đúng một liên kết người còn hiệu lực; không
  dùng người tạo bản ghi để thay thế người nhận. Trường hợp mơ hồ hiển thị
  `待確認收件人` để rà dữ liệu nguồn, thay vì tự chọn một người.
- Tổ chọn 否 không có bước đang chờ. Có thể chọn lại 是 và lưu lại phần lưu trình
  chưa xử lý. Khi đã có bước xác nhận, không cho rút tổ đó khỏi Án.
- Kết quả, người và thứ tự của các bước đã xác nhận được giữ nguyên. Lưu trình
  tổ đã hoàn thành được khóa; phần chưa xử lý của tổ đang chạy có thể chỉnh sửa.
- Án hoàn tất khi cả hai tổ đã xác nhận tham gia/không tham gia và mọi tổ tham gia
  đã hoàn thành. Nếu một tổ chưa xác nhận, trạng thái toàn Án vẫn đang chờ.
- Chỉ backend kiểm tra quyền mới quyết định thao tác hợp lệ. Giao diện chỉ hỗ trợ
  thể hiện quyền đó; gọi trực tiếp hàm sai người/sai bước vẫn bị từ chối.
- Án, xác nhận tổ, bước và lịch sử được ghi trong cùng transaction Serializable.
  Các thao tác dùng token rowversion để từ chối dữ liệu cũ. Không dựa vào FK hay
  UNIQUE mới; tranh chấp deadlock có thử lại giới hạn bằng context mới.
- Giai đoạn này không gửi Notes và không tạo bản ghi hàng đợi Notes.

## Cấu hình và kiểm chứng

Dùng lại các khóa `Module318:UserTuan`, `Module318:UserHung` cho người phụ trách;
hai người phải khác nhau và có trong danh sách người dùng. Mỗi Án giữ người phụ
trách và lưu trình riêng đã lưu.

Ba đường dẫn được lưu riêng theo tài khoản Windows trên máy hiện tại tại
`%LOCALAPPDATA%\7system\Module318\paths.config`. Nếu chưa lưu qua form `設定`,
phần mềm dùng cấu hình cũ `Module318:CommonRoot`, `Module318:SourceTuan`,
`Module318:SourceHung`. Không sửa App.config và không cần thay đổi CSDL.
Hai thư mục tải xuống phải tồn tại và khác nhau. Thư mục đích không được nằm
trong thư mục tải xuống; có thể nhập đích chưa tồn tại để tạo lúc nhập Án.

Bộ kiểm thử tích hợp: `tools/318_WorkflowTests/run.py`, chạy trên LocalDB riêng,
không đọc thông tin kết nối CSDL thật của ứng dụng. Xem README trong thư mục test.

Đã kiểm chứng ngày 2026-10-29: build `KnowledgeSystem` Debug/AnyCPU thành công;
81 kiểm tra tích hợp đạt, bao gồm quyền, chuyển bước, sửa lưu trình, hai tổ,
thao tác đồng thời, nhập file, rollback, khởi tạo grid thực tế, chọn nhân viên
và áp dụng lưu trình cố định, đối chiếu appearance với form ký tên,
mở riêng từng tổ và xác nhận bước trên form một trang, lưu/mở lại đường dẫn,
kiểm tra đường dẫn không hợp lệ và giữ cấu hình cũ khi hủy. Hàng đợi Notes giữ nguyên trong toàn bộ
kiểm thử. Chưa chạy các thao tác nghiệp vụ này trên CSDL thật.
