# Plan WinForm C# — Quản lý câu hỏi & kỳ thi (HSK Reading)

Mục tiêu: WinForm (.NET Framework, EF6 Database-First) dùng để **quản lý** ngân hàng câu hỏi (3 loại + đoạn văn) và cấu hình template kỳ thi. Không xử lý thi/chấm điểm (phần đó do Web FastAPI đảm nhận). Dùng chung 1 DB SQL Server.

## Phân vùng trách nhiệm trên DB

- **WinForm ghi/sửa**: `reading_blocks`, `questions`, `question_options`, `shared_options`, `ordering_items`, `exam_templates`.
- **Web ghi**: `users`, `exam_attempts`, `attempt_blocks`, `attempt_answers` (WinForm không đụng tới).

## Các loại câu hỏi (đơn vị lưu = block)

| type | Mô tả | HSK4 | HSK5 | Bảng con dùng |
|---|---|---|---|---|
| `single_choice` | Đọc đoạn + trắc nghiệm A/B/C/D | Phần 3 | Phần 2, 3 | `questions`, `question_options` |
| `fill_blank` | Điền từ vào chỗ trống | Phần 1 (kho A-F) | Phần 1 (4 đáp án) | `questions` (+ `shared_options` cho HSK4) |
| `ordering` | Sắp xếp câu thành đoạn | Phần 2 | – | `ordering_items` |

Đoạn văn (`passage`) lưu ở cấp block; 1 block có thể chứa nhiều câu dùng chung 1 đoạn (HSK5 phần 3).

## Giai đoạn 1 — Script SQL Server (nền tảng, dùng chung)

1. Chạy `sql/schema.sql` tạo toàn bộ bảng (2 nhóm).
2. Chạy `sql/seed.sql` chèn dữ liệu mẫu mỗi loại + template 7/3 (`max_attempts = 3`).
3. Kiểm tra: CHECK constraint `level`/`type`, FK cascade, index `(level, type)`, NVARCHAR cho tiếng Trung.

## Giai đoạn 2 — EF6 Database-First

1. Tạo project WinForm (.NET Framework) + cài EF6 qua NuGet.
2. Add → ADO.NET Entity Data Model → EF Designer from database → trỏ tới DB → chọn toàn bộ bảng.
3. Kiểm tra entity + navigation property: block ↔ questions ↔ options, block ↔ shared_options, block ↔ ordering_items.

## Giai đoạn 3 — Tầng BUS (nghiệp vụ)

1. `BlockService`: CRUD block + câu hỏi con theo `type`. Validate `question_count` khớp số câu thực tế.
2. `OptionService`: quản lý `question_options` / `shared_options` / `ordering_items` theo loại block.
3. `TemplateService`: CRUD `exam_templates`, validate `hsk4_count + hsk5_count = total_count`, `max_attempts >= 1`.
4. Validate nghiệp vụ:
   - `single_choice`: mỗi câu đủ 4 options + đúng 1 `answer_key`.
   - `fill_blank` HSK4: có kho `shared_options` A-F; `answer_key` thuộc kho.
   - `ordering`: đủ `correct_pos` (1..n) không trùng; `answer_key` là chuỗi thứ tự (vd `A,C,B`).
5. Dùng transaction khi lưu block + câu con (đảm bảo nguyên tử).

## Giai đoạn 4 — UI WinForm

1. Form danh sách block: lọc theo `level`, `type`, tìm kiếm theo nội dung.
2. Form thêm/sửa block: UI **động theo `type`** (single_choice / fill_blank / ordering).
3. Form quản lý template kỳ thi: `name`, `total_count`, `hsk4_count`, `hsk5_count`, `max_attempts`, `is_active`.
4. Preview block đúng như hiển thị đề thi thật.

## Giai đoạn 5 — Hoàn thiện

1. Xử lý lỗi, transaction, rollback khi lưu thất bại.
2. Kiểm thử nhập tiếng Trung (NVARCHAR), xóa cascade (xóa block → tự xóa câu/đáp án con).
3. Kiểm tra số liệu ngân hàng đủ để Web random 7/3 (đủ ≥42 câu HSK4, ≥18 câu HSK5).

## Phụ thuộc

- Giai đoạn 1 (script SQL) là **nền chung** cho cả WinForm và Web — làm trước tiên.
- Web chỉ random ra nội dung khi ngân hàng đã có dữ liệu do WinForm nhập.
