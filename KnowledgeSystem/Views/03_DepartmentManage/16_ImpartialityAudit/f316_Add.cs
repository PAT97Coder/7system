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
        private readonly Font fontUI14 =
            new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);

        private readonly BindingSource source7810 = new BindingSource();
        private readonly BindingSource source7820 = new BindingSource();
        private readonly List<PlanUserItem> users7810 = new List<PlanUserItem>();
        private readonly List<PlanUserItem> users7820 = new List<PlanUserItem>();

        private List<dm_User> users;
        private List<dm_JobTitle> jobTitles;

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

        private void InitializeIcon()
        {
            btnEdit.ImageOptions.SvgImage = TPSvgimages.Edit;
            btnDelete.ImageOptions.SvgImage = TPSvgimages.Remove;
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f316_Add_Load(object sender, EventArgs e)
        {
            btnEdit.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            btnDelete.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            Text = "新增檢驗公正性查核計劃";
            tabbedControlGroup1.SelectedTabPageIndex = 0;

            // CSDL: lấy trực tiếp nhân viên khối 78 và chức danh giống các form khác.
            users = dm_UserBUS.Instance.GetListByDept("78")
                .Where(r => r.Status == 0)
                .ToList();
            jobTitles = dm_JobTitleBUS.Instance.GetList();

            ConfigureUserLookup(lookupUser, repositoryItemGridLookUpEdit1View, "7820");
            ConfigureUserLookup(lookupUser7810, lookupUser7810View, "7810");

            source7820.DataSource = users7820;
            source7810.DataSource = users7810;
            gcProgress.DataSource = source7820;
            gcFiles.DataSource = source7810;
        }

        private void ConfigureUserLookup(
            RepositoryItemGridLookUpEdit lookup,
            GridView popupView,
            string idDept)
        {
            lookup.ValueMember = "Id";
            lookup.DisplayMember = "Id";
            lookup.DataSource = users
                .Where(r => r.IdDepartment.StartsWith(idDept))
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
            if (string.IsNullOrWhiteSpace(displayName))
            {
                MsgTP.MsgShowInfomation(
                    "<font='Microsoft JhengHei UI' size=14>請輸入計劃名稱。</font>");
                return false;
            }

            // CSDL/LINQ: kiểm tra trùng DisplayName trong các kế hoạch chưa xóa mềm.
            if (dt316_PlanBUS.Instance.IsDisplayNameExists(displayName))
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

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                // CSDL: thêm kế hoạch cha và nhận khóa chính Id.
                int idPlan = dt316_PlanBUS.Instance.Add(new dt316_Plan
                {
                    DisplayName = txbTitle.Text.Trim(),
                    CreateAt = DateTime.Now,
                    CreateBy = TPConfigs.LoginUser.Id
                });

                if (idPlan < 0)
                {
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
                    // Hoàn tác kế hoạch bằng xóa mềm nếu lưu danh sách người thất bại.
                    dt316_PlanBUS.Instance.RemoveById(idPlan, TPConfigs.LoginUser.Id);
                    MsgTP.MsgError("儲存計劃人員失敗！");
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            }
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
