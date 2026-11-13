using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraReports.UI;
using DevExpress.XtraSplashScreen;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._00_Generals;
using KnowledgeSystem.Views._03_DepartmentManage._05_PrintLabel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class uc310_EquipmentInfo : XtraUserControl
    {
        private readonly BindingSource sourceData = new BindingSource();

        private List<vw310_EquipmentInfo> equipmentInfos = new List<vw310_EquipmentInfo>();
        private List<dm_GroupUser> userGroups = new List<dm_GroupUser>();
        private List<string> editableDeptIds = new List<string>();
        private bool isEHSAdmin = false;
        private bool canManage = false;

        private DXMenuItem itemViewInfo;
        private DXMenuItem itemBatchManager;
        private DXMenuItem itemMultiselect;
        private DXMenuItem itemPrintStamp;
        private DXMenuItem itemHistory;

        public uc310_EquipmentInfo()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        private void InitializeIcon()
        {
            btnAdd.ImageOptions.SvgImage = TPSvgimages.Add;
            btnReload.ImageOptions.SvgImage = TPSvgimages.Reload;
            AddToolbarButton("待交接設備", TPSvgimages.View, (s, e) =>
            {
                var column = gvData.Columns["ManagerStatus"];
                column.FilterInfo = string.IsNullOrEmpty(column.FilterInfo.FilterString)
                    ? new DevExpress.XtraGrid.Columns.ColumnFilterInfo("Contains([ManagerStatus], '待')")
                    : new DevExpress.XtraGrid.Columns.ColumnFilterInfo();
            });
            gvData.Columns.AddVisible("ManagerStatus", "管理狀態");
        }

        private DevExpress.XtraBars.BarButtonItem AddToolbarButton(string caption, SvgImage icon, EventHandler action)
        {
            var button = new DevExpress.XtraBars.BarButtonItem(barManagerTP, caption);
            button.ImageOptions.SvgImage = icon;
            button.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
            button.ItemClick += (s, e) => action(s, e);
            bar2.AddItem(button);
            return button;
        }

        private void ShowHistory(object sender, EventArgs e)
        {
            var item = equipmentInfos.FirstOrDefault(r => r.Id == Convert.ToInt32(gvData.GetFocusedRowCellValue(gColId) ?? -1));
            if (item == null) return;
            try
            {
                using (var form = CreateHistoryDialog(item.Code, dt310_EquipmentInfoBUS.Instance.GetHistory(item.Id)))
                {
                    form.ShowDialog(this);
                }
            }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private static XtraForm CreateHistoryDialog(string equipmentCode, List<dt310_EquipmentInfoBUS.EquipmentHistory> history)
        {
            return new f310_EquipmentHistory(equipmentCode, history);
        }

        private DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            DXMenuItem menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
            return menuItem;
        }

        private void InitializeMenuItems()
        {
            itemViewInfo = CreateMenuItem("查看信息", ItemViewInfo_Click, TPSvgimages.View);
            itemBatchManager = CreateMenuItem("交接管理人", ItemBatchManager_Click, TPSvgimages.Edit);
            itemMultiselect = CreateMenuItem("啟用多選", ItemMultiselect_Click, TPSvgimages.CheckedRadio);
            itemPrintStamp = CreateMenuItem("列印設備標籤", ItemPrintStamp_Click, TPSvgimages.Print);
            itemHistory = CreateMenuItem("交接紀錄", ShowHistory, TPSvgimages.View);
        }

        private void LoadData()
        {
            try
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(gcData))
                {
                    var activeIds = new HashSet<int>(dt310_EquipmentInfoBUS.Instance.GetList().Select(r => r.Id));
                    var items = vw310_EquipmentInfoBUS.Instance.GetList().Where(r => activeIds.Contains(r.Id)).ToList();
                    BindEquipmentData(items, dm_UserBUS.Instance.GetList());
                }
            }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private void BindEquipmentData(List<vw310_EquipmentInfo> items, List<dm_User> allUsers)
        {
            var users = allUsers.ToDictionary(r => r.Id);
            equipmentInfos = items;

            var displayData = equipmentInfos
                .Select(r => new
                {
                    r.Id,
                    r.Code,
                    r.DisplayNameVN,
                    r.DisplayNameTW,
                    DeptDisplay = string.Join(" ", new[] { r.DeptId, r.DeptNameTW }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    ManagerDisplay = string.Join(" ", new[] { r.ManagerId, r.ManagerNameTW, r.ManagerNameVN }.Where(x => !string.IsNullOrWhiteSpace(x))),
                    ManagerStatus = !users.ContainsKey(r.ManagerId ?? "") ? "待指定管理人"
                        : users[r.ManagerId].Status != 0 ? "待交接：已離職"
                        : users[r.ManagerId].ResignPlan.HasValue ? "待交接：預報離職"
                        : users[r.ManagerId].IdDepartment != r.DeptId ? "待確認：跨部門管理" : "正常",
                    r.Note
                })
                .ToList();

            sourceData.DataSource = displayData;
            gcData.DataSource = sourceData;

            gvData.BestFitColumns();
        }

        private void ResolvePermission()
        {
            userGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);

            List<dm_Group> ehsGroups = dm_GroupBUS.Instance.GetListContainName("安衛環");
            dm_Group adminGroup = ehsGroups.FirstOrDefault(r => string.Equals(r.DisplayName?.Trim(), "安衛環7", StringComparison.Ordinal));
            isEHSAdmin = adminGroup != null && userGroups.Any(r => r.IdGroup == adminGroup.Id);

            editableDeptIds = ehsGroups
                .Where(r => userGroups.Any(gu => gu.IdGroup == r.Id))
                .Select(r => (r.DisplayName ?? string.Empty).Replace("安衛環", string.Empty).Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r) && r != "7" && r.All(char.IsDigit))
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            canManage = isEHSAdmin || editableDeptIds.Count > 0;
        }

        private bool CanManageDept(string deptId)
        {
            if (isEHSAdmin)
            {
                return true;
            }

            return !string.IsNullOrWhiteSpace(deptId) && editableDeptIds.Contains(deptId);
        }

        private List<vw310_EquipmentInfo> GetSelectedEquipmentInfos()
        {
            List<int> selectedIds = gvData.GetSelectedRows()
                .Where(r => r >= 0)
                .Select(r => Convert.ToInt32(gvData.GetRowCellValue(r, gColId) ?? -1))
                .Where(r => r > 0)
                .Distinct()
                .ToList();

            if (selectedIds.Count == 0)
            {
                int focusedId = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId) ?? -1);
                if (focusedId > 0)
                {
                    selectedIds.Add(focusedId);
                }
            }

            return equipmentInfos
                .Where(r => selectedIds.Contains(r.Id))
                .OrderBy(r => selectedIds.IndexOf(r.Id))
                .ToList();
        }

        private List<dm_User> GetAvailableManagers(IEnumerable<vw310_EquipmentInfo> items)
        {
            HashSet<string> visibleDeptIds = new HashSet<string>();

            if (isEHSAdmin)
            {
                foreach (var item in items.Where(r => !string.IsNullOrWhiteSpace(r.DeptId)))
                {
                    visibleDeptIds.Add(item.DeptId);
                }

                if (visibleDeptIds.Count == 0)
                {
                    return dm_UserBUS.Instance.GetList()
                        .Where(r => r.Status == 0)
                        .OrderBy(r => r.IdDepartment)
                        .ThenBy(r => r.DisplayName)
                        .ToList();
                }
            }
            else
            {
                foreach (string deptId in editableDeptIds)
                {
                    visibleDeptIds.Add(deptId);
                }
            }

            return dm_UserBUS.Instance.GetList()
                .Where(r => r.Status == 0 && visibleDeptIds.Contains(r.IdDepartment))
                .OrderBy(r => r.IdDepartment)
                .ThenBy(r => r.DisplayName)
                .ToList();
        }

        private static SearchLookUpEdit CreateManagerEditor(List<dm_User> managers)
        {
            var editor = new SearchLookUpEdit();
            var popupView = new GridView();
            editor.Properties.PopupView = popupView;
            editor.Properties.DataSource = managers.Select(r => new
            {
                r.Id,
                DisplayName = $"LG{r.IdDepartment}/{r.DisplayName} {r.DisplayNameVN}",
                r.IdDepartment
            }).ToList();
            editor.Properties.DisplayMember = "DisplayName";
            editor.Properties.ValueMember = "Id";
            editor.Properties.NullText = "";
            editor.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            editor.Properties.Appearance.Font = new Font("Microsoft JhengHei UI", 14.25F);
            editor.Properties.AppearanceDropDown.Font = new Font("Microsoft JhengHei UI", 12F);

            popupView.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F);
            popupView.Appearance.HeaderPanel.ForeColor = Color.Black;
            popupView.Appearance.HeaderPanel.Options.UseFont = true;
            popupView.Appearance.HeaderPanel.Options.UseForeColor = true;
            popupView.Appearance.HeaderPanel.Options.UseTextOptions = true;
            popupView.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            popupView.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F);
            popupView.Appearance.Row.Options.UseFont = true;
            popupView.OptionsSelection.EnableAppearanceFocusedCell = false;
            popupView.OptionsView.EnableAppearanceOddRow = true;
            popupView.OptionsView.ShowAutoFilterRow = true;
            popupView.OptionsView.ShowGroupPanel = false;
            popupView.Columns.AddVisible("IdDepartment", "部門");
            popupView.Columns.AddVisible("Id", "工號");
            popupView.Columns.AddVisible("DisplayName", "姓名");

            popupView.OptionsView.ColumnAutoWidth = false;
            popupView.Columns["IdDepartment"].Width = 100;
            popupView.Columns["Id"].Width = 160;
            popupView.Columns["DisplayName"].Width = 460;
            editor.Properties.PopupFormSize = new Size(760, 400);
            editor.Properties.Appearance.Options.UseFont = true;
            return editor;
        }

        private string SelectManagerId(List<dm_User> managers)
        {
            using (SearchLookUpEdit editor = CreateManagerEditor(managers))
            {
                XtraInputBoxArgs args = new XtraInputBoxArgs
                {
                    Caption = TPConfigs.SoftNameTW,
                    Prompt = "\u8acb\u9078\u64c7\u65b0\u7684\u7ba1\u7406\u4eba",
                    DefaultButtonIndex = 0,
                    Editor = editor
                };
                return XtraInputBox.Show(args)?.ToString();
            }
        }

        private void ShowPrintPreview(IEnumerable<vw310_EquipmentInfo> items)
        {
            var selectedIds = new HashSet<int>(items.Select(r => r.Id));
            if (selectedIds.Count == 0)
            {
                XtraMessageBox.Show("請先選擇要列印的設備資料。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var activeIds = new HashSet<int>(dt310_EquipmentInfoBUS.Instance.GetList().Select(r => r.Id));
            List<object> labels = vw310_EquipmentInfoBUS.Instance.GetList()
                .Where(r => selectedIds.Contains(r.Id) && activeIds.Contains(r.Id))
                .Select(r => new
                {
                    NameVN = r.DisplayNameVN ?? string.Empty,
                    NameTW = r.DisplayNameTW ?? string.Empty,
                    Code = r.Code ?? string.Empty,
                    Dept = r.DeptId ?? string.Empty,
                    UserVN = r.ManagerNameVN ?? string.Empty,
                    UserTW = r.ManagerNameTW ?? string.Empty
                })
                .Cast<object>()
                .ToList();

            if (labels.Count == 0)
            {
                XtraMessageBox.Show("請先選擇要列印的設備資料。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (XtraReport report = new rpDeviceManagement())
            {
                report.DataSource = labels;
                report.CreateDocument();
                report.PrintingSystem.ShowMarginsWarning = false;

                using (f00_PrintReport printReport = new f00_PrintReport())
                {
                    printReport.ViewerReport.DocumentSource = report;
                    printReport.ShowDialog(this);
                }
            }
        }

        private void ItemMultiselect_Click(object sender, EventArgs e)
        {
            bool multiSelect = gvData.OptionsSelection.MultiSelect;
            gvData.OptionsSelection.MultiSelect = !multiSelect;
            gvData.OptionsSelection.MultiSelectMode = !multiSelect
                ? GridMultiSelectMode.CheckBoxRowSelect
                : GridMultiSelectMode.RowSelect;

            if (multiSelect)
            {
                gvData.ClearSelection();
            }
        }

        private void ItemPrintStamp_Click(object sender, EventArgs e)
        {
            try { ShowPrintPreview(GetSelectedEquipmentInfos()); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private void ItemViewInfo_Click(object sender, EventArgs e)
        {
            int idBase = Convert.ToInt32(gvData.GetRowCellValue(gvData.FocusedRowHandle, gColId) ?? -1);
            if (idBase <= 0)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.View,
                formName = "設備",
                idBase = idBase
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }

        private void uc310_EquipmentInfo_Load(object sender, EventArgs e)
        {
            try
            {
                ResolvePermission();

                if (!canManage)
                {
                    btnAdd.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }

                gvData.ReadOnlyGridView();
                gvData.OptionsBehavior.Editable = false;
                gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
                gvData.OptionsView.ShowGroupPanel = false;

                LoadData();
            }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private void btnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!canManage)
            {
                return;
            }

            using (var finfo = new f310_EquipmentInfo_Info
            {
                eventInfo = EventFormInfo.Create,
                formName = "設備"
            })
            {
                finfo.ShowDialog(this);
            }

            LoadData();
        }

        private void btnReload_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadData();
        }

        private void ExecuteBatchManagerUpdate()
        {
            if (!canManage)
            {
                MsgTP.MsgNoPermission();
                return;
            }

            List<vw310_EquipmentInfo> selectedItems = GetSelectedEquipmentInfos();
            if (selectedItems.Count == 0)
            {
                XtraMessageBox.Show("請先勾選要修改管理人的設備。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<vw310_EquipmentInfo> invalidItems = selectedItems
                .Where(r => !CanManageDept(r.DeptId))
                .ToList();

            if (invalidItems.Count > 0)
            {
                XtraMessageBox.Show("所選設備中包含非您權限範圍的部門資料，請重新確認。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<dm_User> managers = GetAvailableManagers(selectedItems);
            if (managers.Count == 0)
            {
                XtraMessageBox.Show("目前沒有可選擇的管理人資料。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedManagerId = SelectManagerId(managers);
            if (string.IsNullOrWhiteSpace(selectedManagerId))
            {
                return;
            }

            var selectedManager = managers.FirstOrDefault(r => r.Id == selectedManagerId);
            if (selectedManager == null)
            {
                MsgTP.MsgError("請從清單選擇有效的接任人。");
                return;
            }
            string managerName = selectedManager.DisplayName;
            if (XtraMessageBox.Show($"確認將 {selectedItems.Count} 台設備交接給 {managerName}？", TPConfigs.SoftNameTW,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                using (var handle = SplashScreenManager.ShowOverlayForm(this))
                {
                    dt310_EquipmentInfoBUS.Instance.TransferManagers(selectedItems
                        .Where(r => r.ManagerId != selectedManagerId)
                        .Select(r => new dt310_EquipmentInfoBUS.ManagerChange
                        {
                            EquipmentId = r.Id, PreviousManagerId = r.ManagerId,
                            DeptId = r.DeptId, NewManagerId = selectedManagerId
                        }).ToList(), TPConfigs.LoginUser.Id);
                }
            }
            catch (Exception ex)
            {
                MsgTP.MsgError(ex.Message);
            }

            LoadData();
        }

        private void ItemBatchManager_Click(object sender, EventArgs e)
        {
            try { ExecuteBatchManagerUpdate(); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private void gvData_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.Menu != null && e.HitInfo.InRowCell && e.HitInfo.InDataRow)
            {
                PopulateEquipmentMenu(e.Menu, e.HitInfo.RowHandle);
            }
        }

        private void PopulateEquipmentMenu(DXPopupMenu menu, int rowHandle)
        {
            if (!gvData.IsDataRow(rowHandle)) return;
            if (gvData.OptionsSelection.MultiSelect && !gvData.IsRowSelected(rowHandle))
            {
                gvData.ClearSelection();
                gvData.SelectRow(rowHandle);
            }
            gvData.FocusedRowHandle = rowHandle;

            bool multiSelect = gvData.OptionsSelection.MultiSelect;
            itemMultiselect.Caption = multiSelect ? "啟用單選" : "啟用多選";
            itemMultiselect.BeginGroup = true;

            menu.Items.Add(itemViewInfo);
            menu.Items.Add(itemHistory);
            if (canManage)
            {
                itemBatchManager.BeginGroup = true;
                itemBatchManager.Enabled = GetSelectedEquipmentInfos().All(r => CanManageDept(r.DeptId));
                menu.Items.Add(itemBatchManager);
            }
            menu.Items.Add(itemMultiselect);
            menu.Items.Add(itemPrintStamp);
        }
    }
}
