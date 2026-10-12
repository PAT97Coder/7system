-- Run this script in the application's database before using report 308.
-- Each department has its own row; 7400 and 7410 start with the same values.
-- Rerunning preserves any information already edited in the database.
-- Example: UPDATE dbo.dt308_ReportContact SET Email = N'new@email.com' WHERE DepartmentId = '7400';
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.dt308_ReportContact', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.dt308_ReportContact
    (
        DepartmentId varchar(4) NOT NULL CONSTRAINT PK_dt308_ReportContact PRIMARY KEY,
        CompanyVN nvarchar(512) NOT NULL,
        CompanyTW nvarchar(512) NOT NULL,
        IndustryVN nvarchar(1024) NOT NULL,
        IndustryTW nvarchar(1024) NOT NULL,
        AddressVN nvarchar(512) NOT NULL,
        AddressTW nvarchar(512) NOT NULL,
        Email nvarchar(256) NOT NULL,
        ContactVN nvarchar(256) NOT NULL,
        ContactTW nvarchar(256) NOT NULL
    );
END;

DECLARE @Contacts TABLE
(
    DepartmentId varchar(4) PRIMARY KEY,
    CompanyVN nvarchar(512), CompanyTW nvarchar(512),
    IndustryVN nvarchar(1024), IndustryTW nvarchar(1024),
    AddressVN nvarchar(512), AddressTW nvarchar(512),
    Email nvarchar(256), ContactVN nvarchar(256), ContactTW nvarchar(256)
);
INSERT INTO @Contacts (DepartmentId, CompanyVN, CompanyTW, IndustryVN, IndustryTW, AddressVN, AddressTW, Email, ContactVN, ContactTW)
VALUES
    ('7010', N'Phòng Giám đốc - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-經理室管理組', N'Phụ trách kiểm tra quản lí hiệu suất, nhân sự, Vật tư, ATVS&MT, hỗ trợ kĩ thuật.', N'負責部內績效、人事、人事、資材、安衛環及技術等', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'nguyenbichthao261292@gmail.com', N'Nguyễn Thị Bích Thảo', N'阮氏碧草'),
    ('7100', N'Phòng Tiêu chuẩn Luyện kim và Hỗ trợ kỹ thuật - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-冶金規範與技術服務處', N'Phụ trách kiểm tra quản lí hiệu suất, nhân sự, Vật tư, ATVS&MT, hỗ trợ kĩ thuật.', N'負責部內績效、人事、資材、安衛環等', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'tangthiquyen1995@gmail.com', N'Tăng Thị Quyên', N'曾氏娟'),
    ('7300', N'Phòng Quản Lý Chất Lượng Luyện Thép - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-煉鋼品管處', N'Phụ trách kiểm tra quản lí hiệu suất, nhân sự, Vật tư', N'負責部內績效、人事、人事、資材', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'binhhle97@gmail.com', N'Lê Thị Bình', N'黎氏平'),
    ('7400', N'Tổ QC cán dây cây, Phòng Quản Lý Chất Lượng cán Thép - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-軋鋼品管處-線棒品管技術課', N'Phụ trách kiểm tra quản lí hiệu suất, nhân sự, Vật tư', N'負責部內績效、人事、人事、資材等', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'vuongthihoa051998@gmail.com', N'Vương Thị Hòa', N'王氏和'),
    ('7410', N'Tổ QC cán dây cây, Phòng Quản Lý Chất Lượng cán Thép - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-軋鋼品管處-線棒品管技術課', N'Phụ trách kiểm tra quản lí hiệu suất, nhân sự, Vật tư', N'負責部內績效、人事、人事、資材等', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'vuongthihoa051998@gmail.com', N'Vương Thị Hòa', N'王氏和'),
    ('7420', N'Phòng quản lý chất lượng cán thép - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-軋鋼品管處', N'Quản lý chất lượng cán nóng', N'軋鋼品管', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'nguyenthiha3105@gmail.com', N'Nguyễn Thị Hà', N'阮氏河'),
    ('7700', N'Phòng Phân tích hóa học nguyên liệu - Bộ phận Kĩ thuật luyện kim - Công ty TNHH Gang Thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-原料化學分析處', N'Phụ trách thử nghiệm tính chất hóa lý của nguyên vật liệu hoặc thử nghiệm hóa học của thành phẩm, và hiệu chuẩn các thiết bị liệu quan.', N'負責原物料物理、化學試驗或產品之化學試驗及相關設備校正', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'Phanhoangha93@gmail.com', N'Phan Thị Hà', N'阮氏娥'),
    ('7800', N'Phòng thử nghiệm Vật lý - Bộ phận kỹ thuật Luyện Kim - Công ty TNHH Gang thép Hưng Nghiệp Formosa Hà Tĩnh', N'台塑河靜鋼鐵興業責任有限公司冶金部-物理試驗處', N'Nghiên cứu phát triển quy trình sản xuất và sản phẩm, thử nghiệm kim tương và vật lý của sản phẩm, kiểm nghiệm không phá hủy và hiệu chuẩn các thiết bị liên quan', N'負責產品，製製程之研發及產品之金相及物理實驗，非破損檢驗及相關設備校正等', N'KKT Vũng Áng, Phường Vũng Áng, Tỉnh Hà Tĩnh', N'河靜省永安坊永安經濟區', N'nguyenhanh.hatinh1997@gmail.com', N'Nguyễn Thị Hạnh', N'阮氏幸');

INSERT INTO dbo.dt308_ReportContact (DepartmentId, CompanyVN, CompanyTW, IndustryVN, IndustryTW, AddressVN, AddressTW, Email, ContactVN, ContactTW)
SELECT source.DepartmentId, source.CompanyVN, source.CompanyTW, source.IndustryVN, source.IndustryTW, source.AddressVN, source.AddressTW, source.Email, source.ContactVN, source.ContactTW
FROM @Contacts AS source
WHERE NOT EXISTS (
    SELECT 1 FROM dbo.dt308_ReportContact AS target WITH (UPDLOCK, HOLDLOCK)
    WHERE target.DepartmentId = source.DepartmentId
);

COMMIT TRANSACTION;