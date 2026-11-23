using BusinessLayer;
using DataAccessLayer;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using KnowledgeSystem.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._11_ExpenseReimbursement
{
    public partial class uc311_ContractorAcceptance : XtraUserControl
    {
        private List<ContractorAcceptance> records = new List<ContractorAcceptance>();
        private readonly ContractorAcceptanceService service = new ContractorAcceptanceService();
        private int? permissionId;

        public uc311_ContractorAcceptance()
        {
            InitializeComponent();
            TraditionalGridLocalizer.Install();
        }

        private bool HasAccess { get { return TPConfigs.LoginUser != null && permissionId.HasValue && AppPermission.lsPermissions.Contains(permissionId.Value); } }
        private string UserId { get { return TPConfigs.LoginUser?.Id; } }

        private void Screen_Load(object sender, EventArgs e)
        {
            if (DesignMode || TPConfigs.LoginUser == null) return;
            try
            {
                permissionId = service.GetPermissionId(UserId);
                UpdateState();
                if (HasAccess) ReloadRecords();
            }
            catch (Exception ex) { ShowError(ex); }
        }

        private void UpdateState()
        {
            import.Enabled = HasAccess;
            reload.Enabled = HasAccess;
            export.Enabled = HasAccess && records.Count > 0;
        }

        private void BindRecords(List<ContractorAcceptance> data, string selectedNumber = null)
        {
            var previouslySelected = view.GetFocusedRow() as ContractorAcceptance;
            bool wasExpanded = previouslySelected != null && previouslySelected.DocumentNumber == selectedNumber
                && view.GetMasterRowExpanded(view.FocusedRowHandle);
            records = data;
            grid.DataSource = records;
            if (selectedNumber != null)
            {
                int row = view.LocateByValue(nameof(ContractorAcceptance.DocumentNumber), selectedNumber);
                if (row >= 0)
                {
                    view.FocusedRowHandle = row;
                    if (wasExpanded) view.SetMasterRowExpanded(row, true);
                }
            }
            UpdateState();
        }

        private void Import_Click(object sender, ItemClickEventArgs e)
        {
            if (!HasAccess) return;
            using (var dialog = new OpenFileDialog { Title = "匯入外包工作驗收單", Filter = "Excel 活頁簿 (*.xlsx)|*.xlsx", Multiselect = true })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try { ImportFiles(dialog.FileNames, ConfirmImport); }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        private void ImportFiles(string[] files, Func<int, IReadOnlyDictionary<string, byte[]>, bool> confirm)
        {
            var added = files.SelectMany(ContractorAcceptanceWorkbook.Read).ToList();
            var duplicates = service.FindDuplicates(UserId, added);
            if (!confirm(added.Count, duplicates)) return;
            service.Save(UserId, added, duplicates);
            ReloadRecords(added[0].DocumentNumber);
        }

        private bool ConfirmImport(int count, IReadOnlyDictionary<string, byte[]> duplicates)
        {
            string message = $"本次新增 {count - duplicates.Count} 張驗收單。";
            if (duplicates.Count > 0)
            {
                string keys = string.Join("、", duplicates.Keys.OrderBy(key => key).Take(12));
                if (duplicates.Count > 12) keys += "…";
                message += $"\n本單編號已存在（{duplicates.Count} 張）：{keys}\n確認後將覆寫這些驗收單、工作明細及來源檔案。";
            }
            return XtraMessageBox.Show(this, message + "\n要儲存本次匯入資料嗎？", "確認匯入",
                MessageBoxButtons.YesNo, duplicates.Count == 0 ? MessageBoxIcon.Question : MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        private void Reload_Click(object sender, ItemClickEventArgs e)
        {
            if (!HasAccess) return;
            try { ReloadRecords(); }
            catch (Exception ex) { ShowError(ex); }
        }

        private void ReloadRecords(string number = null)
        {
            number = number ?? (view.GetFocusedRow() as ContractorAcceptance)?.DocumentNumber;
            BindRecords(service.GetList(UserId), number);
        }

        private List<ContractorAcceptance> GetVisibleRecords()
        {
            return Enumerable.Range(0, view.RowCount).Select(index => view.GetRow(view.GetVisibleRowHandle(index)))
                .OfType<ContractorAcceptance>().ToList();
        }

        private void Export_Click(object sender, ItemClickEventArgs e)
        {
            if (!HasAccess || records.Count == 0) return;
            // Xuất đúng các phiếu còn hiển thị sau lọc/sắp xếp, cùng toàn bộ chi tiết của chúng.
            var visible = GetVisibleRecords();
            if (visible.Count == 0) { XtraMessageBox.Show(this, "沒有可匯出的資料。", "提示"); return; }
            using (var dialog = new SaveFileDialog { Title = "匯出驗收資料", Filter = "Excel 活頁簿 (*.xlsx)|*.xlsx", FileName = "外包工作驗收資料.xlsx" })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    ContractorAcceptanceWorkbook.Export(dialog.FileName, visible);
                    XtraMessageBox.Show(this, "匯出完成。", "提示");
                }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        private void MasterRowExpanded(object sender, CustomMasterRowEventArgs e)
        {
            view.FocusedRowHandle = e.RowHandle;
            if (!(view.GetDetailView(e.RowHandle, e.RelationIndex) is GridView expanded)) return;
            expanded.BestFitColumns();
            foreach (GridColumn column in expanded.VisibleColumns)
            {
                int designedWidth = detailView.Columns[column.FieldName].Width;
                column.Width = Math.Max(designedWidth, Math.Min(column.Width, designedWidth + 100));
            }
        }

        private void PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (!HasAccess || e.Menu == null || !e.HitInfo.InDataRow) return;
            view.FocusedRowHandle = e.HitInfo.RowHandle;
            view.ClearSelection();
            view.SelectRow(e.HitInfo.RowHandle);
            var selected = view.GetRow(e.HitInfo.RowHandle) as ContractorAcceptance;
            var action = new DXMenuItem("ERP：外包工作驗收輸入", Erp_Click, TPSvgimages.Robot, DXMenuItemPriority.Normal)
            {
                BeginGroup = true, Enabled = selected != null && selected.Validation == "檢查通過"
            };
            action.ImageOptions.SvgImageSize = new System.Drawing.Size(24, 24);
            e.Menu.Items.Add(action);
            var download = new DXMenuItem("下載來源 Excel", (menu, args) => DownloadSource(selected),
                TPSvgimages.Excel, DXMenuItemPriority.Normal) { Enabled = selected != null };
            e.Menu.Items.Add(download);
            TraditionalGridLocalizer.StyleMenu(e.Menu);
        }

        private void DownloadSource(ContractorAcceptance selected)
        {
            if (!HasAccess || selected == null) return;
            using (var dialog = new SaveFileDialog { Title = "下載來源 Excel", Filter = "Excel 活頁簿 (*.xlsx)|*.xlsx", FileName = selected.SourceFileName })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    byte[] content = service.GetSource(UserId, selected.DocumentNumber);
                    if (content == null) throw new FileNotFoundException("找不到來源檔案。");
                    File.WriteAllBytes(dialog.FileName, content);
                }
                catch (Exception ex) { ShowError(ex); }
            }
        }

        private void Erp_Click(object sender, EventArgs e)
        {
            if (!HasAccess || !(view.GetFocusedRow() is ContractorAcceptance selected) || selected.Validation != "檢查通過") return;
            // Điểm nối ERP; thứ tự trường và phím sẽ được cấu hình khi có màn hình đích.
            XtraMessageBox.Show(this, "ERP 輸入順序尚未設定。請先設定目標畫面的欄位順序與按鍵流程。", "ERP 外包工作驗收");
        }

        private void ShowError(Exception ex) { XtraMessageBox.Show(this, ex.Message, "處理失敗", MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
