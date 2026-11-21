# Bộ dữ liệu SQL chạy tay cho phân tích 312

Hai file chạy bằng SSMS, tương thích cú pháp SQL Server 2014:

- Tạo: `Database/20261104_seed_312_analysis_demo.sql`.
- Xóa: `Database/20261104_cleanup_312_analysis_demo.sql`.

Đây là bộ **kết quả thi thật giả lập đã nộp** phục vụ thử màn hình phân tích,
không phải dữ liệu thi thử và không phải tổ chức một lượt thi trực tiếp bằng tài khoản mẫu.
Script dựng ngân hàng câu hỏi/đáp án, kỳ thi, phân công đã nộp và JSON cùng cấu trúc
`f312_DoExam.ExamResult`. Các lựa chọn trong snapshot được đánh số lại 1–4 như khi thi.
Điểm tính trên trắc nghiệm, loại tự luận; bài nhiều lựa chọn yêu cầu đúng cả tập đáp án.

## Chạy

1. Mở file tạo trong SSMS, kiểm tra dòng `USE [DBDocumentManagementSystem]`, chạy F5.
2. Script trả ra Id các kỳ thi/nhóm và mã người test để bạn tra cứu.
3. Vào **錯題分析**, bấm **刷新**.
4. Khi chọn **全部歷史 + 全部人員**, kết quả còn chứa dữ liệu thật sẵn có. Nhập
   **`[T312A_20261104]`** ở dòng lọc cột **題目群組** để chỉ xem ba nhóm mẫu;
   biểu đồ tự lọc theo bảng. Không cần xóa dữ liệu cũ để đối chiếu.
5. Chọn phạm vi, kỳ thi và người theo bảng đối chiếu bên dưới.
6. Thử xong mở file cleanup và chạy F5, rồi **刷新** màn hình.

Không cần sửa code, cập nhật model hoặc cấp quyền mới cho người test để xem thống kê.
Người thao tác màn hình vẫn cần quyền **錯題分析** như hiện tại.

## Thành phần được thêm

| Dữ liệu | Số lượng | Nhận diện |
|---|---:|---|
| Hồ sơ người mẫu | 3 | VNW9900311 / VNW9900312 / VNW9900313 |
| Nhóm | 3 | `[T312A_20261104] 安全 / 設備 / 流程` |
| Câu hỏi ngân hàng | 12 | Mỗi nhóm 2 đơn chọn, 1 nhiều lựa chọn, 1 tự luận |
| Đáp án ngân hàng | 36 | 4 lựa chọn cho mỗi câu trắc nghiệm |
| Kỳ thi | 3 | `[T312A_20261104] 2024期 / 2025期 / 2026期` |
| Bài đã nộp | 9 | Mỗi người một bài ở mỗi kỳ |

Người mẫu:

| Mã | Tên hiển thị |
|---|---|
| VNW9900311 | 測試甲 |
| VNW9900312 | 測試乙 |
| VNW9900313 | 測試丙 |

Kỳ 2024 có **2 câu trắc nghiệm + 1 tự luận mỗi nhóm**; kỳ 2025 có **3 + 1**;
kỳ 2026 có **1 + 1**. Vì vậy tổng lịch sử có số lượt khác nhau giữa các kỳ,
giúp kiểm chứng việc cộng lượt câu thay vì trung bình phần trăm.
Tổng cộng 54 lượt trắc nghiệm và 27 lượt tự luận trong 9 bài.
Tự luận có bài làm/đáp án tham khảo đầy đủ để mở xem kết quả, nhưng không vào tỷ lệ sai.
Có cả trường hợp bỏ trống trắc nghiệm, nhiều lựa chọn thiếu đáp án và chọn đúng đảo thứ tự.

Các nhóm mẫu được đánh dấu ngừng sử dụng ngay lúc tạo để không bị bốc vào bài thi
đang hoạt động. Phân tích vẫn đọc lịch sử của các nhóm này. Cấu hình thi, role và
dữ liệu có sẵn không bị sửa; không tạo tài khoản AD hay mật khẩu cho người mẫu.

## Bốn cách đối chiếu

Các số dưới đây chỉ xét ba nhóm có dấu `[T312A_20261104]`.

| Cách chọn | 安全: sai/tổng — tỷ lệ | 設備: sai/tổng — tỷ lệ | 流程: sai/tổng — tỷ lệ |
|---|---|---|---|
| 單次考試 → 2024期; 人員 → VNW9900311 | 2/2 — 100.00% | 1/2 — 50.00% | 0/2 — 0.00% |
| 單次考試 → 2025期; 人員 → 全部人員 | 6/9 — 66.67% | 4/9 — 44.44% | 3/9 — 33.33% |
| 全部歷史; 人員 → VNW9900311 | 3/6 — 50.00% | 5/6 — 83.33% | 3/6 — 50.00% |
| 全部歷史; 人員 → 全部人員 | 10/18 — 55.56% | 9/18 — 50.00% | 8/18 — 44.44% |

Số người / số bài đóng góp **cho từng nhóm** tương ứng bốn dòng:
**1/1**, **3/3**, **1/3**, **3/9**.

Mặc định sắp xếp theo tỷ lệ sai giảm dần nên thứ tự dòng/cột có thể khác thứ tự
ba nhóm trong bảng hướng dẫn. Tỷ lệ bằng nhau được phân tiếp theo lượt câu, rồi GroupId.

Ví dụ 測試甲 ở nhóm 設備: sai 1/2 trong 2024, 3/3 trong 2025, 1/1 trong 2026 →
**5/6 = 83.33%**, không phải trung bình của 50%, 100%, 100%.

## Xóa và chạy lại

Chạy nguyên file `20261104_cleanup_312_analysis_demo.sql`.
Kết quả bình thường: **9 bài, 36 đáp án, 12 câu, 3 nhóm, 3 kỳ, 3 hồ sơ** được xóa.
Không xóa chức năng phân tích/menu hoặc quyền đã đăng ký.

- Script tạo dừng nếu mã người hoặc bộ dữ liệu đã tồn tại; không ghi đè.
- Cleanup nhận diện bằng mã người riêng, marker, tên kỳ/nhóm và dấu thời gian.
- Không xóa theo khoảng Id hoặc xóa toàn bảng; không reset IDENTITY.
- Nếu người mẫu có role, bài thi ngoài bộ mẫu hoặc dữ liệu thi thử phát sinh,
  cleanup dừng trước khi xóa để bạn kiểm tra. Chỉ dùng các hồ sơ này cho bộ mẫu.
- Có thể chạy cleanup lần nữa: nếu đã xóa hết thì số bản ghi xóa là 0.
- Sau cleanup có thể chạy lại seed; Id tự tăng sẽ khác lần trước, kết quả kỳ vọng vẫn như trên.

## Kiểm chứng

Đã chạy seed và cleanup trên database LocalDB tạm qua bộ kiểm thử module 312:
**560 điều kiện đạt**. Kiểm tra JSON mở được bằng form review, điểm đúng logic chấm,
số lượt/tỷ lệ của đủ bốn phạm vi, chạy seed trùng bị từ chối, cleanup dừng khi có
role ngoài bộ mẫu, cleanup xong giữ nguyên dữ liệu gốc và chạy cleanup lần hai an toàn.
Chưa chạy seed/cleanup vào database ứng dụng của bạn; bạn tự chạy bằng SSMS.
