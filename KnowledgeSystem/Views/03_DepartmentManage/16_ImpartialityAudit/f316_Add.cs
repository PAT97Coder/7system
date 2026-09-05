using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._16_ImpartialityAudit
{
    /// <summary>
    /// Form thêm kế hoạch 316. Hai tab 7810 và 7820 dùng cùng chức năng chọn người.
    /// Dữ liệu được lưu vào dt316_Plan và dt316_PlanUser.
    /// </summary>
    public partial class f316_Add : XtraForm
    {
        private const string PlanFileDept = "PLAN";

        private readonly Font fontUI14 =
            new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private readonly BindingSource source7810 = new BindingSource();
        private readonly BindingSource source7820 = new BindingSource();
        private readonly List<PlanUserItem> users7810 = new List<PlanUserItem>();
        private readonly List<PlanUserItem> users7820 = new List<PlanUserItem>();
        private readonly int? planId;

        private List<dm_User> users;
        private List<dm_JobTitle> jobTitles;
        private Attachment attachment;

        private class Attachment : dm_Attachment
        {
            public string FullPath { get; set; }
        }

        private class PlanUserItem
        {
            public string IdUsr { get; set; }
            public string UserName { get; set; }
            public string JobName { get; set; }

        }

        public f316_Add()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public f316_Add(int idPlan) : this()
        {
            planId = idPlan;
        }

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
            txbAtt.Properties.Buttons[0].ImageOptions.SvgImage = TPSvgimages.Search;
            txbAtt.Properties.Buttons[1].ImageOptions.SvgImage = TPSvgimages.Copy;
        }

        private void f316_Add_Load(object sender, EventArgs e)
        {
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            Text = planId.HasValue
                ? "編輯檢驗公正性查核計劃"
                : "新增檢驗公正性查核計劃";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            // CSDL: lấy trực tiếp nhân viên khối 78 và chức danh giống các form khác.
            // Giữ cả người đã có trong kế hoạch để chế độ sửa luôn hiển thị đủ dữ liệu;
            // danh sách lookup bên dưới vẫn chỉ cho chọn nhân viên đang hoạt động.
            users = dm_UserBUS.Instance.GetListByDept("78").ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();

            ConfigureUserLookup(lookupUser, repositoryItemGridLookUpEdit1View, "7820");
            ConfigureUserLookup(lookupUser7810, lookupUser7810View, "7810");

            source7820.DataSource = users7820;
            source7810.DataSource = users7810;
            gcProgress.DataSource = source7820;
            gcFiles.DataSource = source7810;

            if (planId.HasValue) LoadExistingPlan(planId.Value);
        }

        private void LoadExistingPlan(int idPlan)
        {
            // CSDL: nạp Plan, PlanUser và PDF kế hoạch để dùng lại chính form Add khi sửa.
            var plan = dt316_PlanBUS.Instance.GetItemById(idPlan);
            if (plan == null || plan.RemoveAt != null)
            {
                MsgTP.MsgError("找不到計劃！");
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            txbTitle.Text = plan.DisplayName;

            var planReport = dt316_ReportBUS.Instance
                .GetItemByPlanAndDept(idPlan, PlanFileDept);
            if (planReport?.IdAdt != null)
            {
                var currentAttachment = dm_AttachmentBUS.Instance
                    .GetItemById(planReport.IdAdt.Value);
                if (currentAttachment != null)
                {
                    attachment = new Attachment
                    {
                        Id = currentAttachment.Id,
                        Thread = currentAttachment.Thread,
                        ActualName = currentAttachment.ActualName,
                        EncryptionName = currentAttachment.EncryptionName
                    };
                    txbAtt.Text = currentAttachment.ActualName;
                }
            }

            foreach (var mapping in dt316_PlanUserBUS.Instance.GetListByPlan(idPlan))
            {
                var user = users.FirstOrDefault(r => r.Id == mapping.IdUser);
                if (user == null) continue;

                var item = new PlanUserItem
                {
                    IdUsr = user.Id,
                    UserName = user.DisplayName,
                    JobName = jobTitles.FirstOrDefault(r => r.Id == user.JobCode)?.DisplayName
                };

                if (mapping.IdDept?.StartsWith("7810") == true)
                    users7810.Add(item);
                else if (mapping.IdDept?.StartsWith("7820") == true)
                    users7820.Add(item);
            }

            source7810.ResetBindings(false);
            source7820.ResetBindings(false);
        }

        private void ConfigureUserLookup(
            RepositoryItemGridLookUpEdit lookup,
            GridView popupView,
            string idDept)
        {
            lookup.ValueMember = "Id";
            lookup.DisplayMember = "Id";
            lookup.DataSource = users
                .Where(r => r.Status == 0 && r.IdDepartment.StartsWith(idDept))
                .ToList();
            lookup.NullText = string.Empty;

            popupView.Columns.Clear();
            popupView.Columns.AddRange(new[]
            {
                new GridColumn
                {
                    FieldName = "IdDepartment",
                    Caption = "單位",
                    Visible = true,
                    VisibleIndex = 0,
                    Width = 60
                },
                new GridColumn
                {
                    FieldName = "Id",
                    Caption = "工號",
                    Visible = true,
                    VisibleIndex = 1,
                    Width = 100
                },
                new GridColumn
                {
                    FieldName = "DisplayName",
                    Caption = "名稱",
                    Visible = true,
                    VisibleIndex = 2,
                    Width = 140
                }
            });
            popupView.FocusRectStyle = DrawFocusRectStyle.RowFocus;
            popupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.OptionsView.ShowAutoFilterRow = true;
        }

        private bool ValidateData()
        {
            gvProgress.CloseEditor();
            gvProgress.UpdateCurrentRow();
            gvFiles.CloseEditor();
            gvFiles.UpdateCurrentRow();

            string displayName = txbTitle.Text.Trim();
            bool hasPlanFile = attachment != null &&
                (attachment.Id > 0 || File.Exists(attachment.FullPath));
            if (string.IsNullOrWhiteSpace(displayName) || !hasPlanFile)
            {
                MsgTP.MsgShowInfomation(
                    "<font='Microsoft JhengHei UI' size=14>請填寫年度並選擇PDF檔案。</font>");
                return false;
            }

            // CSDL/LINQ: kiểm tra trùng DisplayName trong các kế hoạch chưa xóa mềm.
            if (dt316_PlanBUS.Instance.IsDisplayNameExists(displayName, planId))
            {
                MsgTP.MsgShowInfomation(
                    "<font='Microsoft JhengHei UI' size=14>計劃名稱已存在。</font>");
                return false;
            }

            // Cả 7810 và 7820 đều là dữ liệu bắt buộc theo dấu * trên hai tab.
            if (users7810.Count == 0 || users7820.Count == 0)
            {
                MsgTP.MsgShowInfomation(
                    "<font='Microsoft JhengHei UI' size=14>請為7810和7820選擇人員。</font>");
                return false;
            }

            var allRows = users7810.Concat(users7820).ToList();
            if (allRows.Any(r => string.IsNullOrWhiteSpace(r.IdUsr) ||
                                 !users.Any(user => user.Id == r.IdUsr)))
            {
                MsgTP.MsgShowInfomation(
                    "<font='Microsoft JhengHei UI' size=14>請選擇正確的工號。</font>");
                return false;
            }

            // LINQ: một nhân viên chỉ được chọn một lần trong toàn bộ kế hoạch.
            if (allRows.GroupBy(r => r.IdUsr).Any(group => group.Count() > 1))
            {
                MsgTP.MsgError("人員重複！");
                return false;
            }

            return true;
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!ValidateData()) return;

            if (planId.HasValue)
            {
                UpdatePlan(planId.Value);
                return;
            }

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var attachmentData = new dm_Attachment
                {
                    Thread = attachment.Thread,
                    ActualName = attachment.ActualName,
                    EncryptionName = attachment.EncryptionName
                };

                // CSDL: tạo attachment trước để lấy IdAdt cho dt316_Report.
                int idAttachment = dm_AttachmentBUS.Instance.Add(attachmentData);
                if (idAttachment < 0)
                {
                    MsgTP.MsgError("儲存附件失敗！");
                    return;
                }

                string destinationPath = Path.Combine(TPConfigs.Folder316, attachment.EncryptionName);
                try
                {
                    if (!Directory.Exists(TPConfigs.Folder316))
                        Directory.CreateDirectory(TPConfigs.Folder316);

                    File.Copy(attachment.FullPath, destinationPath, true);
                }
                catch
                {
                    dm_AttachmentBUS.Instance.RemoveById(idAttachment);
                    MsgTP.MsgError("複製附件失敗！");
                    return;
                }

                // CSDL: thêm kế hoạch cha và nhận khóa chính Id.
                int idPlan = dt316_PlanBUS.Instance.Add(new dt316_Plan
                {
                    DisplayName = txbTitle.Text.Trim(),
                    CreateAt = DateTime.Now,
                    CreateBy = TPConfigs.LoginUser.Id
                });

                if (idPlan < 0)
                {
                    RemoveAttachment(idAttachment, destinationPath);
                    MsgTP.MsgError("新增計劃失敗！");
                    return;
                }

                // LINQ/CSDL: IdPlan lấy từ Plan vừa thêm; IdUser và IdDept lấy từ dm_User.
                var mappings = users7810.Concat(users7820)
                    .Select(r =>
                    {
                        var user = users.First(u => u.Id == r.IdUsr);
                        return new dt316_PlanUser
                        {
                            IdPlan = idPlan,
                            IdUser = user.Id,
                            IdDept = user.IdDepartment
                        };
                    })
                    .ToList();

                // CSDL: ghi một lần toàn bộ người của hai tab.
                if (!dt316_PlanUserBUS.Instance.AddRange(mappings))
                {
                    dt316_PlanBUS.Instance.RemoveById(idPlan, TPConfigs.LoginUser.Id);
                    RemoveAttachment(idAttachment, destinationPath);
                    MsgTP.MsgError("儲存計劃人員失敗！");
                    return;
                }

                // CSDL/LINQ: PDF chọn tại form Add là file kế hoạch, lưu ở dòng PLAN.
                // Mỗi report phòng ban dùng đúng IdDept đã ghi trong dt316_PlanUser,
                // không tự rút gọn hoặc gán cứng mã 7810/7820.
                var reports = mappings
                    .Where(r => !string.IsNullOrWhiteSpace(r.IdDept))
                    .Select(r => r.IdDept)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select(idDept => CreateReport(idPlan, idDept, null))
                    .ToList();
                reports.Insert(0, CreateReport(idPlan, PlanFileDept, idAttachment));

                if (!dt316_ReportBUS.Instance.AddRange(reports))
                {
                    dt316_PlanUserBUS.Instance.RemoveByPlan(idPlan);
                    dt316_PlanBUS.Instance.RemoveById(idPlan, TPConfigs.LoginUser.Id);
                    RemoveAttachment(idAttachment, destinationPath);
                    MsgTP.MsgError("儲存報告失敗！");
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void UpdatePlan(int idPlan)
        {
            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                var mappings = BuildPlanUsers(idPlan);

                // CSDL: tìm theo Id và ghi đè 年度 vào đúng Plan cũ, không tạo Plan mới.
                if (!dt316_PlanBUS.Instance.UpdateDisplayName(idPlan, txbTitle.Text.Trim()))
                {
                    MsgTP.MsgError("更新計劃失敗！");
                    return;
                }

                // CSDL: thay danh sách người tham gia bằng transaction tại BUS.
                if (!dt316_PlanUserBUS.Instance.ReplaceByPlan(idPlan, mappings))
                {
                    MsgTP.MsgError("更新計劃人員失敗！");
                    return;
                }

                // LINQ/CSDL: bổ sung report trống cho IdDept mới; report cũ và file
                // đã tải vẫn được giữ nguyên để không làm mất dữ liệu lịch sử.
                var currentReports = dt316_ReportBUS.Instance.GetListByPlan(idPlan);
                var missingReports = mappings
                    .Where(r => !string.IsNullOrWhiteSpace(r.IdDept))
                    .Select(r => r.IdDept)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Where(idDept => !currentReports.Any(r => r.IdDept == idDept))
                    .Select(idDept => CreateReport(idPlan, idDept, null))
                    .ToList();
                if (missingReports.Count > 0 &&
                    !dt316_ReportBUS.Instance.AddRange(missingReports))
                {
                    MsgTP.MsgError("更新報告單位失敗！");
                    return;
                }

                // Chỉ tạo attachment mới khi người dùng thực sự chọn PDF kế hoạch khác.
                if (attachment.Id <= 0 && File.Exists(attachment.FullPath) &&
                    !UpdatePlanAttachment(idPlan))
                    return;

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private List<dt316_PlanUser> BuildPlanUsers(int idPlan)
        {
            // LINQ/CSDL: IdDept luôn lấy từ dm_User tại thời điểm lưu.
            return users7810.Concat(users7820)
                .Select(r => users.First(user => user.Id == r.IdUsr))
                .Select(user => new dt316_PlanUser
                {
                    IdPlan = idPlan,
                    IdUser = user.Id,
                    IdDept = user.IdDepartment
                })
                .ToList();
        }

        private bool UpdatePlanAttachment(int idPlan)
        {
            var attachmentData = new dm_Attachment
            {
                Thread = attachment.Thread,
                ActualName = attachment.ActualName,
                EncryptionName = attachment.EncryptionName
            };
            int idAttachment = dm_AttachmentBUS.Instance.Add(attachmentData);
            if (idAttachment < 0)
            {
                MsgTP.MsgError("儲存附件失敗！");
                return false;
            }

            string destinationPath = Path.Combine(TPConfigs.Folder316, attachment.EncryptionName);
            try
            {
                if (!Directory.Exists(TPConfigs.Folder316))
                    Directory.CreateDirectory(TPConfigs.Folder316);
                File.Copy(attachment.FullPath, destinationPath, true);
            }
            catch
            {
                dm_AttachmentBUS.Instance.RemoveById(idAttachment);
                MsgTP.MsgError("複製附件失敗！");
                return false;
            }

            // CSDL: PDF 計劃 vẫn dùng dòng kỹ thuật IdDept = PLAN, tách khỏi report.
            var planReport = dt316_ReportBUS.Instance
                .GetItemByPlanAndDept(idPlan, PlanFileDept);
            bool saved;
            if (planReport == null)
            {
                saved = dt316_ReportBUS.Instance.Add(
                    CreateReport(idPlan, PlanFileDept, idAttachment)) > 0;
            }
            else
            {
                planReport.IdAdt = idAttachment;
                saved = dt316_ReportBUS.Instance.AddOrUpdate(planReport);
            }

            if (saved) return true;

            RemoveAttachment(idAttachment, destinationPath);
            MsgTP.MsgError("更新計劃檔案失敗！");
            return false;
        }

        private dt316_Report CreateReport(int idPlan, string idDept, int? idAttachment)
        {
            return new dt316_Report
            {
                IdPlan = idPlan,
                IdDept = idDept,
                IdAdt = idAttachment,
                CreateAt = DateTime.Now,
                CreateBy = TPConfigs.LoginUser.Id
            };
        }

        private void RemoveAttachment(int idAttachment, string filePath)
        {
            dm_AttachmentBUS.Instance.RemoveById(idAttachment);
            if (File.Exists(filePath)) File.Delete(filePath);
        }

        private void txbAtt_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            string filePath = null;

            if (e.Button == txbAtt.Properties.Buttons[1])
            {
                if (Clipboard.ContainsFileDropList())
                {
                    var pdfFiles = Clipboard.GetFileDropList()
                        .Cast<string>()
                        .Where(path => File.Exists(path) &&
                                       string.Equals(Path.GetExtension(path), ".pdf",
                                           StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (pdfFiles.Count == 1) filePath = pdfFiles[0];
                }
            }
            else
            {
                using (var dialog = new OpenFileDialog { Filter = "Pdf|*.pdf" })
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        filePath = dialog.FileName;
                }
            }

            if (string.IsNullOrWhiteSpace(filePath))
            {
                XtraMessageBox.Show("請選擇一個PDF檔案", TPConfigs.SoftNameTW,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            attachment = new Attachment
            {
                Thread = "316",
                ActualName = Path.GetFileName(filePath),
                EncryptionName = EncryptionHelper.EncryptionFileName(filePath),
                FullPath = filePath
            };
            txbAtt.Text = attachment.ActualName;
        }

        private void gvProgress_CellValueChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            UpdateSelectedUser(sender as GridView, e);
        }

        private void gv7810_CellValueChanged(
            object sender,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            UpdateSelectedUser(sender as GridView, e);
        }

        private void UpdateSelectedUser(
            GridView view,
            DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (view == null || e.Column.FieldName != "IdUsr") return;

            var user = users.FirstOrDefault(r => r.Id == e.Value?.ToString());
            view.SetRowCellValue(e.RowHandle, "UserName", user?.DisplayName ?? string.Empty);
            view.SetRowCellValue(e.RowHandle, "JobName",
                jobTitles.FirstOrDefault(r => r.Id == user?.JobCode)?.DisplayName ?? string.Empty);
        }

        private void btnDelProg_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            PlanUserItem item = gvProgress.GetRow(gvProgress.FocusedRowHandle) as PlanUserItem;
            if (item == null) return;

            users7820.Remove(item);
            source7820.ResetBindings(false);
        }

        private void btnDelFile_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            PlanUserItem item = gvFiles.GetRow(gvFiles.FocusedRowHandle) as PlanUserItem;
            if (item == null) return;

            users7810.Remove(item);
            source7810.ResetBindings(false);
        }

        private void gridView_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            GridView view = sender as GridView;
            if (view == null || !e.Info.IsRowIndicator || e.RowHandle < 0) return;

            e.Info.ImageIndex = -1;
            e.Info.DisplayText = (e.RowHandle + 1).ToString();
            e.Info.Appearance.Font = fontUI14;
            e.Info.Appearance.ForeColor = view.Appearance.HeaderPanel.ForeColor;

            int width = Convert.ToInt32(e.Graphics.MeasureString(e.Info.DisplayText, fontUI14).Width) + 20;
            if (view.IndicatorWidth < width) view.IndicatorWidth = width;
        }
    }
}
