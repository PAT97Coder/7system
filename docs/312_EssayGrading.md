# Chấm tự luận — yêu cầu và triển khai

Menu kết quả dùng trạng thái bài thi, tương tự các module307/315/316:
chưa nộp không có hành động; đã nộp còn tự luận chưa có điểm chỉ hiện
進行評分 cho người có quyền; đã chấm hết chỉ hiện 查看詳情. Bài chỉ có trắc
nghiệm xem như đã chấm tự động. Dựa vào điểm từng câu trong JSON, không
dùng riêng Score (bài cũ có thể đã có Score nhưng tự luận chưa chấm).
Màn hình kết quả của người thi cũng chỉ hiện 查看詳情 sau khi đã chấm xong.
Khi nhấp phải phải lấy trạng thái từ đúng dòng được nhấp và dữ liệu mới nhất.

Sửa tương thích bài cũ chưa chấm: nếu JSON chưa có MaxScore ở tất cả câu,
khi mở chấm sẽ chia đều100 điểm theo số câu thực tế của bài đã lưu. Form và
BUS cùng dùng PreparePendingGradingData; không lấy số câu hiện tại trong
cấu hình để tính lại bài cũ. Không ghi CSDL khi chỉ mở xem/chấm; MaxScore
được lưu cùng điểm khi kết thúc chấm. Bài đã chấm và bài đã có MaxScore
giữ nguyên dữ liệu đã lưu.

Yêu cầu cuối cùng ngày 2026-11-16: điểm tự chia đều theo số câu trên thang100,
không nhập trọng số riêng. Hai ô 配分 (%) chỉ đọc và tính từ số câu thực tế
sau làm tròn. Ví dụ10 câu, dù7/3 hay6/4, mọi câu đều tối đa10 điểm.
Nếu100 không chia hết cho số câu, phân bổ phần dư0,01 điểm để tổng vẫn100.
Bài chính thức mới luôn lưu MaxScore và SnapshotPassingScore lúc tạo đề,
không dùng ChoiceScorePercent cũ; trường này được giữ để tương thích model
và được đặt NULL khi lưu cấu hình. Không cần thay đổi CSDL cho điều chỉnh này.
Bài đã nộp giữ điểm tối đa trong snapshot cũ. Những mô tả về trọng số nhập tay
bên dưới chỉ ghi lại phiên bản trước, đã được thay thế bởi yêu cầu này.

Yêu cầu đã thống nhất: dùng chung `f312_ExamReview` cho xem bài và chấm bài,
phân quyền xem/chấm riêng; không tạo `f312_ExamGrading`.
Tại kết quả kỳ thi thêm `進行評分`, chấm tự luận rồi bấm `結束評分`.
Bài chính thức có tự luận chưa chấm để trống điểm tổng và kết quả đạt.

Triển khai: chạy `Database/20261114_add_312_grading_permission.sql`, sau đó
cấp quyền `f312_ExamReview_Grading` cho vai trò chấm bằng quản lý phân quyền.
Người chấm cần quyền vào `uc312_ExamMgmt` để mở danh sách kết quả.
Quyền xem hiện tại giữ nguyên. Kiểm tra quyền chấm lúc mở và lúc lưu.

Quy tắc cho bài cũ/chưa cấu hình trọng số: mỗi câu có trọng số bằng nhau. Trắc nghiệm đúng được 100,
sai được 0; tự luận nhập 0–100. Tổng là trung bình các câu, làm tròn về số nguyên.
Điểm tự luận, người chấm, thời gian chấm được lưu trong JSON `ExamData`.
Không thay đổi schema bảng kết quả. Hoàn tất chấm chỉ được lưu một lần;
bài thay đổi trong lúc đang mở phải được mở lại trước khi lưu.
Điểm đạt của bài cũ sử dụng cấu hình hiện tại, giống luồng nộp bài hiện có.
Thi thử giữ nguyên cách tính điểm trắc nghiệm, tự luận không tính điểm.
Bài cũ có tự luận nhưng chưa có điểm từng câu hiển thị `待評分` trong chi tiết;
điểm tổng cũ trong bảng kết quả chỉ cập nhật khi hoàn tất chấm.

Tỷ lệ số câu trắc nghiệm/tự luận đã được bổ sung trong `考試資訊設定`:
hai ô phần trăm tự bù nhau để tổng là 100%, hiển thị trước số câu thực tế.
Lưu `ChoicePercent` (số nguyên 0–100) trong `dt312_Setting`; phần tự luận là
100 trừ tỷ lệ trắc nghiệm. Script `Database/20261114_add_312_question_ratio.sql`
thêm cột, giá trị mặc định 70%, và ràng buộc khoảng hợp lệ; có thể chạy lại.
Cần chạy script trước khi sử dụng bản ứng dụng có cấu hình tỷ lệ.

Trắc nghiệm = làm tròn tổng câu × ChoicePercent / 100, phần .5 làm tròn lên;
tự luận = tổng câu trừ số câu trắc nghiệm. Ví dụ 10 câu với 70%/30% thành 7/3;
3 câu với 50%/50% thành 2/1. Đây là tỷ lệ số câu, không phải tỷ trọng điểm.
Đề chính thức và thi thử đều áp dụng tỷ lệ, lấy câu ngẫu nhiên và phân bổ
giữa các nhóm còn hoạt động trong từng loại câu. Khi thiếu một loại,
hệ thống báo số câu cần/có và yêu cầu bổ sung câu hỏi hoặc đổi cấu hình.
Các bài đã nộp tiếp tục dùng nội dung đề đã lưu.

Kiểm chứng: build Debug thành công; bộ kiểm thử tích hợp/UI trên database tạm
đạt 10752 assertions (gồm tỷ lệ câu hỏi). Đã kiểm tra script quyền chạy hai lần, từ chối người chỉ
có quyền xem, thiếu điểm/ngoài khoảng, bài thay đổi, lưu qua form thật,
tổng điểm và thông tin người chấm, từ chối chấm lại bài đã hoàn tất.
Database tạm được xóa sau kiểm thử. Đã áp dụng script quyền chấm và cấp full
quyền 312 cho VNW0018616 theo yêu cầu ở database Debug 10.198.138.153.
Ngày 2026-11-14 đã áp dụng script tỷ lệ tại database này: cấu hình hiện tại
10 câu, 70% trắc nghiệm / 30% tự luận; ngân hàng hoạt động có 18 câu trắc
nghiệm và 7 câu tự luận. Database Release 10.198.138.103 chưa được cập nhật.

Ngày 2026-11-16 bổ sung trọng số điểm trong `考試資訊設定`, tách riêng với
tỷ lệ số câu: `選擇題配分（%）` và `申論題配分（%）` tự bù thành 100%.
Script `Database/20261116_add_312_score_weight.sql` chỉ thêm cột lưu
`ChoiceScorePercent int NULL`; kiểm tra và chia điểm bằng C#/LINQ.
Đã áp dụng script tại database Debug 10.198.138.153. NULL giữ chế độ cũ
đến khi người quản lý lưu cấu hình trọng số. Release chưa áp dụng script.

Bài chính thức mới lưu `MaxScore` từng câu và `SnapshotPassingScore` trong
JSON khi tạo đề. Chấm bài dựa trên dữ liệu này, không lấy trọng số/điểm đạt
mới thay đổi trong cấu hình. Ví dụ 7 trắc nghiệm/3 tự luận, trọng số40/60:
trắc nghiệm chia tổng40 điểm, tự luận mỗi câu20 điểm. Điểm tối đa có tối đa
hai chữ số thập phân; phần dư từng0,01 điểm được phân bổ cho các câu trong
cùng loại để tổng đúng100. Chỉ có một loại câu thì cả loại nhận100 điểm.
Form chấm hiển thị điểm tối đa thực tế; cho nhập dấu phẩy hoặc dấu chấm,
kiểm tra0..MaxScore và tối đa hai chữ số thập phân cả tại form và BUS.
Tổng cộng trực tiếp các điểm rồi làm tròn về số nguyên ở bước cuối theo
kiểu cột Score hiện có. Thi thử tiếp tục dùng cách tính trước đây.

Build thành công và 10764 kiểm tra tích hợp/UI đạt, bao gồm chấm câu tối đa
12,3 điểm bằng dữ liệu10,5; kiểm tra giới hạn, chữ số thập phân, phân bổ40/60
và tổng100, dữ liệu cấu hình thay đổi không làm đổi cách chấm bài đã nộp.
