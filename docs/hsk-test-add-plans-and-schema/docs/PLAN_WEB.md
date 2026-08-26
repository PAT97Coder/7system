# Plan Web FastAPI + Vuexy/Vue — Thi & Chấm điểm (HSK Reading)

Mục tiêu: thí sinh chọn template kỳ thi → hệ thống **random đề riêng cho từng lần làm** → làm bài → chấm điểm → xem lịch sử. Dùng chung DB SQL Server, **chỉ đọc** ngân hàng câu hỏi, ghi nhóm `attempt_*` và `users`. Giới hạn **tối đa 3 lần làm** mỗi user trên mỗi template.

## Nguyên tắc quan trọng

- **Đề KHÔNG dùng chung**: mỗi lần một user chọn kỳ thi thì random một bộ câu hỏi riêng (lưu trong `attempt_blocks`). 2 người (hoặc 2 lần) ra đề khác nhau.
- `exam_templates` chỉ lưu **quy tắc** (tổng câu, tỷ lệ 7/3, số lần làm tối đa), không lưu câu cụ thể.

## Giai đoạn 1 — Kết nối & model (Backend)

1. Setup FastAPI + SQLAlchemy, dialect `mssql+pyodbc` + driver ODBC tới SQL Server.
2. Map model: ngân hàng câu hỏi (read-only) + nhóm attempt (read/write).
3. Tạo DB login riêng cho API: `SELECT` ngân hàng, `INSERT/UPDATE` nhóm `attempt_*` + `users`.

## Giai đoạn 2 — Logic random đề (cốt lõi, tỷ lệ 7/3)

Random theo **block**, cộng dồn `question_count` đến khi đủ chỉ tiêu mỗi level (vd 42 HSK4 + 18 HSK5). Làm ở tầng Python cho dễ kiểm soát khít số câu.

```python
def pick_blocks(pool, target):
    random.shuffle(pool)
    picked, count = [], 0
    for b in pool:
        if count + b.question_count <= target:
            picked.append(b); count += b.question_count
        if count == target: break
    # lấp đầy bằng block 1 câu nếu còn thiếu
    if count < target:
        for b in pool:
            if b not in picked and b.question_count == 1:
                picked.append(b); count += 1
                if count == target: break
    return picked, count

hsk4, _ = pick_blocks(pool_hsk4, template.hsk4_count)  # 42
hsk5, _ = pick_blocks(pool_hsk5, template.hsk5_count)  # 18
```

Khuyến nghị: giữ phần lớn ngân hàng là `single_choice` (1 câu/block) để luôn ghép đủ số câu.

## Giai đoạn 3 — Tạo kỳ thi + giới hạn làm lại 3 lần

1. Đếm số attempt đã nộp của user trên template:
   ```sql
   SELECT COUNT(*) FROM exam_attempts
   WHERE user_id = @user_id AND template_id = @template_id
     AND submitted_at IS NOT NULL;
   ```
2. So với `exam_templates.max_attempts` (vd 3):
   - Nếu `>= max_attempts` → chặn, báo "Đã hết số lần làm (3/3)".
   - Nếu còn → cho làm, `attempt_no = count + 1`.
3. Attempt dở (`submitted_at` NULL) **không tính** vào số lần; mỗi user chỉ có tối đa 1 attempt dở/template — cho tiếp tục thay vì tạo mới (tránh lách luật).
4. Tạo `exam_attempts` → random block (Giai đoạn 2) → insert `attempt_blocks`.

## Giai đoạn 4 — API endpoints

| Method | Path | Mô tả |
|---|---|---|
| POST | `/attempts` | Chọn template, kiểm tra giới hạn 3 lần, sinh đề, trả câu hỏi (ẩn `answer_key`) |
| GET | `/attempts/{id}` | Lấy đề đang làm (block + questions + options theo `position`) |
| PUT | `/attempts/{id}/answers` | Lưu/cập nhật `attempt_answers` |
| POST | `/attempts/{id}/submit` | Chấm điểm, set `score` + `submitted_at` |
| GET | `/users/{id}/history` | Lịch sử attempt + điểm + còn mấy lần |

## Giai đoạn 5 — Chấm điểm (khi nộp)

```sql
UPDATE a
SET a.is_correct = CASE WHEN a.chosen_key = q.answer_key THEN 1 ELSE 0 END
FROM attempt_answers a
JOIN questions q ON q.id = a.question_id
WHERE a.attempt_id = @attempt_id;

UPDATE exam_attempts
SET score = (SELECT COUNT(*) FROM attempt_answers
             WHERE attempt_id = @attempt_id AND is_correct = 1),
    submitted_at = SYSUTCDATETIME()
WHERE id = @attempt_id;
```

- `single_choice`, `fill_blank`: so trực tiếp `chosen_key = answer_key`.
- `ordering`: so chuỗi thứ tự (`A,C,B`).

## Giai đoạn 6 — Frontend Vuexy/Vue

1. Trang chọn kỳ thi: danh sách template active + hiển thị số lần còn lại.
2. Trang làm bài: render **động theo `type`** (trắc nghiệm / điền từ kho / kéo-thả sắp xếp), đếm giờ nếu cần.
3. Trang kết quả: điểm, câu đúng/sai, đáp án đúng + giải thích.
4. Trang lịch sử làm bài của user.

## Giai đoạn 7 — Hoàn thiện

1. Bảo mật: không trả `answer_key` khi đang làm; chỉ lộ sau khi nộp.
2. Chống nộp trùng; xử lý attempt làm dở.
3. Kiểm thử end-to-end: random → làm → chấm → lịch sử → giới hạn 3 lần.

## Phụ thuộc

- Cần `sql/schema.sql` + `sql/seed.sql` đã chạy và ngân hàng câu hỏi đã có dữ liệu (do WinForm nhập).
