using BusinessLayer;
using DataAccessLayer;
using DevExpress.Data.Browsing;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Svg;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Items.ViewInfo;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraSpreadsheet.Model;
using ExcelDataReader;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator;
using KnowledgeSystem.Views._04_SystemAdministrator._02_SystemAdmin;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using OfficeOpenXml.Table.PivotTable;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Util;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._04_SystemAdministrator._01_Moderator
{
    public partial class uc401_UserManage : DevExpress.XtraEditors.XtraUserControl
    {
        RefreshHelper helper;
        public uc401_UserManage()
        {
            InitializeComponent();
            InitializeIcon();
            InitializeMenuItems();

            helper = new RefreshHelper(gvData, "Id");
            Font fontUI12 = new Font("Microsoft JhengHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            DevExpress.Utils.AppearanceObject.DefaultMenuFont = fontUI12;
        }

        #region parameters

        bool IsSysAdmin = false;

        BindingSource sourceUsers = new BindingSource();
        string sheetName = "DataUser";
        List<dm_User> users = new List<dm_User>();
        readonly HashSet<string> universityEducationCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "公大學一",
            "公大學工",
            "私大學一",
            "重大學一",
            "重大學工"
        };

        DXMenuItem itemEditRole;
        DXMenuItem itemEditSign;
        DXMenuItem itemEditGroup;

        DXMenuItem CreateMenuItem(string caption, EventHandler clickEvent, SvgImage svgImage)
        {
            var menuItem = new DXMenuItem(caption, clickEvent, svgImage, DXMenuItemPriority.Normal);
            SetMenuItemProperties(menuItem);
            return menuItem;
        }

        void SetMenuItemProperties(DXMenuItem menuItem)
        {
            menuItem.ImageOptions.SvgImageSize = new Size(24, 24);
            menuItem.AppearanceHovered.ForeColor = Color.Blue;
        }

        private void InitializeMenuItems()
        {
            itemEditRole = CreateMenuItem("設定用色", ItemEditRole_Click, TPSvgimages.Num1);
            itemEditSign = CreateMenuItem("設定簽名", ItemEditSign_Click, TPSvgimages.Num2);
            itemEditGroup = CreateMenuItem("設定群組", ItemEditGroup_Click, TPSvgimages.Num3);
        }

        private void ItemEditGroup_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "group", idUsr = idUser };
            form.ShowDialog();
        }

        private void ItemEditSign_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "sign", idUsr = idUser };
            form.ShowDialog();

            //string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            //f402_UserSigns fSetting = new f402_UserSigns();
            //fSetting.eventInfo = EventFormInfo.View;
            //fSetting.idUsr = idUser;
            //fSetting.ShowDialog();
        }

        private void ItemEditRole_Click(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            var form = new f402_UserMapping() { mapData = "role", idUsr = idUser };
            form.ShowDialog();

            //string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();

            //f402_UserRoles fSetting = new f402_UserRoles();
            //fSetting.eventInfo = EventFormInfo.View;
            //fSetting.idUsr = idUser;
            //fSetting.ShowDialog();
        }

        #endregion

        #region methods

        private void InitializeIcon()
        {
            btnCreate.ImageOptions.SvgImage = TPSvgimages.Add;
            btnRefresh.ImageOptions.SvgImage = TPSvgimages.Reload;
            btnExportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnExportReportExcel.ImageOptions.SvgImage = TPSvgimages.Excel;
            btnManageDeptHeadcount.ImageOptions.SvgImage = TPSvgimages.Dept;
        }

        private void InitializeControl()
        {
            gColPCName.Visible = IsSysAdmin;
            gColIP.Visible = IsSysAdmin;
            gColLastUpdate.Visible = IsSysAdmin;
        }

        private void CreateRuleGV()
        {
            // Quy tắc định dạng khi TransactionType = 'C'
            var ruleResignPlan = new GridFormatRule
            {
                ApplyToRow = true,
                Column = gColStatus,
                Name = "RuleResignPlan",
                Rule = new FormatConditionRuleExpression
                {
                    Expression = "[StatusName] = '預報離職'",
                    Appearance = { ForeColor = DevExpress.LookAndFeel.DXSkinColors.ForeColors.Critical, }
                }
            };
            gvData.FormatRules.Add(ruleResignPlan);
        }

        private void LoadUser()
        {
            helper.SaveViewInfo();

            string idDept2Word = TPConfigs.LoginUser.IdDepartment.Substring(0, 2);
            string idDept1Word = TPConfigs.LoginUser.IdDepartment.Substring(0, 1);

            if (TPConfigs.IdParentControl == AppPermission.SysAdmin)
            {
                users = dm_UserBUS.Instance.GetList();
            }
            else if (TPConfigs.IdParentControl == AppPermission.Mod)
            {
                users = dm_UserBUS.Instance.GetListByDept(idDept1Word);
            }
            else if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            {
                users = dm_UserBUS.Instance.GetListByDept(idDept2Word);
            }

            List<dm_Departments> lsDepts = dm_DeptBUS.Instance.GetList();
            List<dm_Role> lsRoles = dm_RoleBUS.Instance.GetList();
            List<dm_JobTitle> lsJobTitles = dm_JobTitleBUS.Instance.GetList();

            var lsUserManage = (from data in users
                                join depts in lsDepts on data.IdDepartment equals depts.Id
                                join job in lsJobTitles on data.JobCode equals job.Id into dtg
                                from g in dtg.DefaultIfEmpty()
                                join actualJob in lsJobTitles on data.ActualJobCode equals actualJob.Id into atg
                                from a in atg.DefaultIfEmpty()
                                let displayName = $"{data.DisplayName}{(!string.IsNullOrEmpty(data.DisplayNameVN) ? $"\r\n{data.DisplayNameVN}" : "")}"
                                let deptName = $"{data.IdDepartment}\r\n{depts.DisplayName}"
                                let sexName = data.Sex == null ? "" : data.Sex.Value ? "男" : "女"
                                let statusName = (data.ResignPlan != null && data.Status == 0) ? "預報離職" : (data.Status == null ? "" : TPConfigs.lsUserStatus[data.Status.Value])
                                select new
                                {
                                    Data = data,
                                    Depts = depts,
                                    DisplayName = displayName,
                                    DeptName = deptName,
                                    JobName = g != null ? g.DisplayName : "",
                                    ActualJobName = a != null ? a.DisplayName : "",
                                    SexName = sexName,
                                    StatusName = statusName,
                                }).ToList();

            sourceUsers.DataSource = lsUserManage;

            helper.LoadViewInfo();
            gvData.BestFitColumns();
            gcData.RefreshDataSource();
        }

        #endregion

        private void f401_UserManager_Load(object sender, EventArgs e)
        {
            IsSysAdmin = AppPermission.Instance.CheckAppPermission(AppPermission.SysAdmin);
            InitializeControl();

            gvData.ReadOnlyGridView();
            gvData.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;

            gcData.DataSource = sourceUsers;

            LoadUser();
            CreateRuleGV();

            string filterString = "[StatusName] In ('在職','留職停薪','預報離職')";
            gvData.Columns["StatusName"].FilterInfo = new ColumnFilterInfo(filterString);
        }

        private void btnCreate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.Create;
            fInfo.formName = "用戶";
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            LoadUser();
        }

        private void gvData_DoubleClick(object sender, EventArgs e)
        {
            string idUser = gvData.GetRowCellValue(gvData.FocusedRowHandle, gColIdUser).ToString();
            dm_User _userSelect = users.FirstOrDefault(r => r.Id == idUser);

            f401_UserInfo fInfo = new f401_UserInfo();
            fInfo.eventInfo = EventFormInfo.View;
            fInfo.formName = "用戶";
            fInfo.userInfo = _userSelect;
            fInfo.ShowDialog();

            LoadUser();
        }

        private void btnExportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
                Directory.CreateDirectory(documentsPath);

            string filePath = Path.Combine(documentsPath, $"{Text} - {DateTime.Now:yyyyMMddHHmm}.xlsx");

            gcData.ExportToXlsx(filePath);
            Process.Start(filePath);
        }

        private void btnExportReportExcel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!TryPromptMonthRange(out DateTime startMonth, out DateTime endMonth))
            {
                return;
            }

            string documentsPath = TPConfigs.DocumentPath();
            if (!Directory.Exists(documentsPath))
            {
                Directory.CreateDirectory(documentsPath);
            }

            string filePath = Path.Combine(documentsPath, $"人員統計_{startMonth:yyyyMM}_{endMonth:yyyyMM}_{DateTime.Now:yyyyMMddHHmm}.xlsx");

            using (var handle = SplashScreenManager.ShowOverlayForm(this))
            {
                ExportSummaryWorkbook(filePath, startMonth, endMonth);
            }

            Process.Start(filePath);
        }

        private void btnManageDeptHeadcount_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (var form = new DepartmentHeadcountManageForm())
            {
                form.ShowDialog(this);
            }
        }

        private bool TryPromptMonthRange(out DateTime startMonth, out DateTime endMonth)
        {
            startMonth = default(DateTime);
            endMonth = default(DateTime);

            var editor = new TextEdit
            {
                Font = new Font("Microsoft JhengHei UI", 14F),
                EditValue = $"{DateTime.Today.AddMonths(-2):yyyy/MM}-{DateTime.Today:yyyy/MM}"
            };

            editor.Properties.MaskSettings.Set("MaskManagerType", typeof(DevExpress.Data.Mask.RegExpMaskManager));
            editor.Properties.MaskSettings.Set("mask", @"\d{4}/\d{2}\s*-\s*\d{4}/\d{2}");
            editor.Properties.MaskSettings.Set("isAutoComplete", true);
            editor.Properties.Mask.UseMaskAsDisplayFormat = true;
            editor.Properties.NullValuePrompt = "yyyy/MM-yyyy/MM";
            editor.Properties.NullValuePromptShowForEmptyValue = true;

            var args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "請輸入年月區間 (格式 yyyy/MM-yyyy/MM)",
                Editor = editor,
                DefaultButtonIndex = 0,
                DefaultResponse = editor.EditValue.ToString()
            };

            object result = XtraInputBox.Show(args);
            if (result == null)
            {
                return false;
            }

            string rangeText = result.ToString().Trim();
            string pattern = @"^\d{4}/\d{2}\s*-\s*\d{4}/\d{2}$";
            if (!Regex.IsMatch(rangeText, pattern))
            {
                XtraMessageBox.Show("格式錯誤！請輸入正確格式：yyyy/MM-yyyy/MM", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var parts = rangeText.Split('-');
            startMonth = DateTime.ParseExact(parts[0].Trim(), "yyyy/MM", CultureInfo.InvariantCulture);
            endMonth = DateTime.ParseExact(parts[1].Trim(), "yyyy/MM", CultureInfo.InvariantCulture);

            if (startMonth > endMonth)
            {
                XtraMessageBox.Show("開始年月不可大於結束年月！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void ExportSummaryWorkbook(string filePath, DateTime startMonth, DateTime endMonth)
        {
            var allUsers = GetUsersForExport();
            var allDepts = dm_DeptBUS.Instance.GetList();
            var allJobs = dm_JobTitleBUS.Instance.GetList().ToDictionary(r => r.Id, r => r.DisplayName ?? "", StringComparer.OrdinalIgnoreCase);

            var scopedUsers = allUsers.ToList();

            var summaryDepartments = GetDepartmentsForExport(allDepts)
                .Where(r => !string.IsNullOrWhiteSpace(r.Id) && r.Id.Length == 2)
                .OrderBy(r => r.Id)
                .ToList();

            if (summaryDepartments.Count == 0)
            {
                summaryDepartments = GetDepartmentsForExport(allDepts).Where(r => !string.IsNullOrWhiteSpace(r.Id)).OrderBy(r => r.Id).ToList();
            }

            string divisionPrefix = GetDivisionPrefix();
            string divisionName = allDepts.FirstOrDefault(r => string.Equals(r.Id, divisionPrefix, StringComparison.OrdinalIgnoreCase))?.DisplayName
                                  ?? TPConfigs.LoginUser.IdDepartment
                                  ?? "公司";

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                BuildSheet1(package.Workbook.Worksheets.Add("個人資料明細表"), divisionName, scopedUsers, allJobs, allDepts);
                BuildSheet2(package.Workbook.Worksheets.Add("人數彙總表"), divisionName, summaryDepartments, scopedUsers);
                BuildSheet3(package.Workbook.Worksheets.Add("每月人數統計表"), divisionName, summaryDepartments, scopedUsers, startMonth, endMonth);

                package.SaveAs(new FileInfo(filePath));
            }
        }

        private void BuildSheet1(ExcelWorksheet ws, string divisionName, List<dm_User> scopedUsers, Dictionary<string, string> allJobs, List<dm_Departments> allDepts)
        {
            DateTime today = DateTime.Today;

            var activeUsers = scopedUsers
                .Where(IsIncludedInCurrentSheets)
                .OrderBy(r => r.IdDepartment)
                .ThenBy(r => r.Id)
                .ToList();

            string[] headers =
            {
                "公司", "人員代號", "中文姓名", "身分證號碼", "越文姓名", "部門代號", "部門名稱",
                "出生日期", "進公司日期", "離職日期", "年資", "認定學歷", "職務代號", "職務名稱", "職務生效日", "性別", "國籍"
            };

            ApplyDefaultSheetStyle(ws);
            WriteHeaderRow(ws, 1, headers);

            for (int i = 0; i < activeUsers.Count; i++)
            {
                var user = activeUsers[i];
                int row = i + 2;
                string deptName = allDepts.FirstOrDefault(r => r.Id == user.IdDepartment)?.DisplayName ?? "";
                string jobName = allJobs.ContainsKey(user.JobCode ?? "") ? allJobs[user.JobCode ?? ""] : "";

                ws.Cells[row, 1].Value = divisionName;
                ws.Cells[row, 2].Value = user.Id;
                ws.Cells[row, 3].Value = user.DisplayName;
                ws.Cells[row, 4].Value = user.CitizenID;
                ws.Cells[row, 5].Value = user.DisplayNameVN;
                ws.Cells[row, 6].Value = user.IdDepartment;
                ws.Cells[row, 7].Value = deptName;
                ws.Cells[row, 8].Value = user.DOB?.ToString("yyyy/MM/dd");
                ws.Cells[row, 9].Value = user.DateCreate.ToString("yyyy/MM/dd");
                ws.Cells[row, 10].Value = "";
                ws.Cells[row, 11].Value = Math.Round((today - user.DateCreate).TotalDays / 365.25, 1);
                ws.Cells[row, 12].Value = user.RecognizedEducation;
                ws.Cells[row, 13].Value = user.JobCode;
                ws.Cells[row, 14].Value = jobName;
                ws.Cells[row, 15].Value = user.JobEffectiveDate?.ToString("yyyy/MM/dd");
                ws.Cells[row, 16].Value = user.Sex == null ? "" : user.Sex.Value ? "男" : "女";
                ws.Cells[row, 17].Value = user.Nationality;
            }

            ApplyTableBorder(ws, 1, 1, Math.Max(2, activeUsers.Count + 1), headers.Length);
            SetColumnWidths(ws, new[] { 14d, 12d, 14d, 14d, 14d, 12d, 14d, 12d, 14d, 14d, 10d, 12d, 12d, 14d, 14d, 10d, 10d });
            ws.View.FreezePanes(2, 1);
        }

        private void BuildSheet2(ExcelWorksheet ws, string divisionName, List<dm_Departments> departments, List<dm_User> scopedUsers)
        {
            DateTime today = DateTime.Today;

            ApplyDefaultSheetStyle(ws);

            ws.Cells["A1:J1"].Merge = true;
            ws.Cells["A1"].Value = "編制缺額統計";
            ws.Cells["A1"].Style.Font.Bold = true;
            ws.Cells["A1"].Style.Font.Size = 18;

            ws.Cells["H2:J2"].Merge = true;
            ws.Cells["H2"].Value = $"製作日期：{today:yyyy/M/d}";
            ws.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            ws.Cells["A3:A4"].Merge = true;
            ws.Cells["B3:B4"].Merge = true;
            ws.Cells["C3:C4"].Merge = true;
            ws.Cells["D3:F3"].Merge = true;
            ws.Cells["G3:G4"].Merge = true;
            ws.Cells["H3:I3"].Merge = true;
            ws.Cells["J3:J4"].Merge = true;

            ws.Cells["A3"].Value = "項次";
            ws.Cells["B3"].Value = "處別";
            ws.Cells["C3"].Value = "編制";
            ws.Cells["D3"].Value = "實際";
            ws.Cells["D4"].Value = "TW";
            ws.Cells["E4"].Value = "VN";
            ws.Cells["F4"].Value = "合計";
            ws.Cells["G3"].Value = "缺額";
            ws.Cells["H3"].Value = "預告離職";
            ws.Cells["H4"].Value = "大學";
            ws.Cells["I4"].Value = "中高專";
            ws.Cells["J3"].Value = "說明";

            int startRow = 5;
            int totalAuthorized = 0;
            int totalTw = 0;
            int totalVn = 0;
            int totalResignUniversity = 0;
            int totalResignOther = 0;

            for (int i = 0; i < departments.Count; i++)
            {
                var dept = departments[i];
                int row = startRow + i;
                string deptPrefix = dept.Id ?? "";
                int authorized = dept.AuthorizedHeadcount ?? 0;
                int actualTw = scopedUsers.Count(r => (r.IdDepartment ?? "").StartsWith(deptPrefix, StringComparison.OrdinalIgnoreCase)
                                                  && IsIncludedInCurrentSheets(r)
                                                  && string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
                int actualVn = scopedUsers.Count(r => (r.IdDepartment ?? "").StartsWith(deptPrefix, StringComparison.OrdinalIgnoreCase)
                                                  && IsIncludedInCurrentSheets(r)
                                                  && !string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
                int resignUniversity = scopedUsers.Count(r => (r.IdDepartment ?? "").StartsWith(deptPrefix, StringComparison.OrdinalIgnoreCase)
                                                           && r.ResignPlan.HasValue
                                                           && IsIncludedInCurrentSheets(r)
                                                            && universityEducationCodes.Contains((r.RecognizedEducation ?? "").Trim()));
                int resignOther = scopedUsers.Count(r => (r.IdDepartment ?? "").StartsWith(deptPrefix, StringComparison.OrdinalIgnoreCase)
                                                     && r.ResignPlan.HasValue
                                                     && IsIncludedInCurrentSheets(r)
                                                     && !universityEducationCodes.Contains((r.RecognizedEducation ?? "").Trim()));
                int actualTotal = actualTw + actualVn;
                int shortage = authorized - actualTotal;

                ws.Cells[row, 1].Value = i + 1;
                ws.Cells[row, 2].Value = dept.DisplayName;
                ws.Cells[row, 3].Value = authorized;
                ws.Cells[row, 4].Value = actualTw;
                ws.Cells[row, 5].Value = actualVn;
                ws.Cells[row, 6].Value = actualTotal;
                ws.Cells[row, 7].Value = shortage;
                ws.Cells[row, 8].Value = resignUniversity;
                ws.Cells[row, 9].Value = resignOther;
                ws.Cells[row, 10].Value = "";

                totalAuthorized += authorized;
                totalTw += actualTw;
                totalVn += actualVn;
                totalResignUniversity += resignUniversity;
                totalResignOther += resignOther;
            }

            int totalRow = startRow + departments.Count;
            int totalActual = totalTw + totalVn;
            int totalShortage = totalAuthorized - totalActual;
            int suspendCount = scopedUsers.Count(r => IsIncludedInCurrentSheets(r) && r.Status == 2);

            ws.Cells[totalRow, 1].Value = "合計";
            ws.Cells[totalRow, 1, totalRow, 2].Merge = true;
            ws.Cells[totalRow, 3].Value = totalAuthorized;
            ws.Cells[totalRow, 4].Value = totalTw;
            ws.Cells[totalRow, 5].Value = totalVn;
            ws.Cells[totalRow, 6].Value = totalActual;
            ws.Cells[totalRow, 7].Value = totalShortage;
            ws.Cells[totalRow, 8].Value = totalResignUniversity;
            ws.Cells[totalRow, 9].Value = totalResignOther;

            int noteRow = totalRow + 2;
            ws.Cells[noteRow, 1, noteRow, 10].Merge = true;
            ws.Cells[noteRow, 1].Value = $"本部編制人數共{totalAuthorized}人，實際人數共{totalActual}人(含停薪留職{suspendCount}人)，缺額{totalShortage}人({(totalAuthorized == 0 ? 0 : Math.Round(totalShortage * 100d / totalAuthorized, 1))}%)。";
            ws.Cells[noteRow, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            ApplyTableBorder(ws, 3, 1, totalRow, 10);
            ws.Cells[3, 1, 4, 10].Style.Font.Bold = true;
            ws.Cells[totalRow, 1, totalRow, 10].Style.Font.Bold = true;
            SetColumnWidths(ws, new[] { 8d, 20d, 10d, 10d, 10d, 10d, 10d, 12d, 12d, 18d });
        }

        private void BuildSheet3(ExcelWorksheet ws, string divisionName, List<dm_Departments> departments, List<dm_User> scopedUsers, DateTime startMonth, DateTime endMonth)
        {
            var months = GetMonthSequence(startMonth, endMonth).ToList();
            ApplyDefaultSheetStyle(ws);

            ws.Cells[1, 1].Value = "單位";
            ws.Cells[1, 2].Value = "編制";
            for (int i = 0; i < months.Count; i++)
            {
                ws.Cells[1, i + 3].Value = $"{months[i].Month}月";
            }

            int startRow = 2;
            int totalAuthorized = 0;
            for (int i = 0; i < departments.Count; i++)
            {
                var dept = departments[i];
                int row = startRow + i;
                string deptPrefix = dept.Id ?? "";
                int authorized = dept.AuthorizedHeadcount ?? 0;
                totalAuthorized += authorized;

                ws.Cells[row, 1].Value = dept.DisplayName;
                ws.Cells[row, 2].Value = authorized;

                for (int m = 0; m < months.Count; m++)
                {
                    ws.Cells[row, m + 3].Value = scopedUsers.Count(r => (r.IdDepartment ?? "").StartsWith(deptPrefix, StringComparison.OrdinalIgnoreCase)
                                                                     && IsEmployedOnDate(r, GetMonthEnd(months[m])));
                }
            }

            int totalRow = startRow + departments.Count;
            ws.Cells[totalRow, 1].Value = divisionName;
            ws.Cells[totalRow, 2].Value = totalAuthorized;
            for (int m = 0; m < months.Count; m++)
            {
                ws.Cells[totalRow, m + 3].Value = scopedUsers.Count(r => IsEmployedOnDate(r, GetMonthEnd(months[m])));
            }

            ApplyTableBorder(ws, 1, 1, totalRow, months.Count + 2);
            ws.Cells[1, 1, totalRow, months.Count + 2].Style.Font.Bold = true;

            int summaryStartRow = totalRow + 3;
            ws.Cells[summaryStartRow, 1, summaryStartRow, months.Count + 3].Merge = true;
            ws.Cells[summaryStartRow, 1].Value = $"{divisionName}離職率統計(越籍)";
            ws.Cells[summaryStartRow, 1].Style.Font.Bold = true;
            ws.Cells[summaryStartRow, 1].Style.Font.Size = 16;

            ws.Cells[summaryStartRow + 1, 1].Value = "項目";
            for (int i = 0; i < months.Count; i++)
            {
                ws.Cells[summaryStartRow + 1, i + 2].Value = $"{months[i].Month}月";
            }
            ws.Cells[summaryStartRow + 1, months.Count + 2].Value = "平均";

            string[] metricNames = { "編制人數", "在職人數", "缺額", "離職人數", "月離職率" };
            var monthlyRates = new List<double>();

            for (int i = 0; i < metricNames.Length; i++)
            {
                ws.Cells[summaryStartRow + 2 + i, 1].Value = metricNames[i];
            }

            for (int i = 0; i < months.Count; i++)
            {
                DateTime month = months[i];
                DateTime monthEnd = GetMonthEnd(month);
                DateTime monthStart = month;

                int activeVietnamese = scopedUsers.Count(r => IsEmployedOnDate(r, monthEnd)
                                                           && !string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
                int resignedVietnamese = scopedUsers.Count(r => r.ResignDate.HasValue
                                                             && r.ResignDate.Value.Date >= monthStart
                                                             && r.ResignDate.Value.Date <= monthEnd
                                                             && !string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
                int shortage = totalAuthorized - activeVietnamese;
                double rate = activeVietnamese == 0 ? 0 : resignedVietnamese * 100d / activeVietnamese;
                monthlyRates.Add(rate);

                ws.Cells[summaryStartRow + 2, i + 2].Value = totalAuthorized;
                ws.Cells[summaryStartRow + 3, i + 2].Value = activeVietnamese;
                ws.Cells[summaryStartRow + 4, i + 2].Value = shortage;
                ws.Cells[summaryStartRow + 5, i + 2].Value = resignedVietnamese;
                ws.Cells[summaryStartRow + 6, i + 2].Value = $"{Math.Round(rate, 2)}%";
            }

            ws.Cells[summaryStartRow + 2, months.Count + 2].Value = totalAuthorized;
            ws.Cells[summaryStartRow + 3, months.Count + 2].Value = months.Count == 0 ? 0 : scopedUsers.Count(r => IsEmployedOnDate(r, GetMonthEnd(months.Last()))
                                                                                                                   && !string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
            ws.Cells[summaryStartRow + 4, months.Count + 2].Value = totalAuthorized - Convert.ToInt32(ws.Cells[summaryStartRow + 3, months.Count + 2].Value ?? 0);
            ws.Cells[summaryStartRow + 5, months.Count + 2].Value = scopedUsers.Count(r => r.ResignDate.HasValue
                                                                                         && r.ResignDate.Value.Date >= startMonth
                                                                                         && r.ResignDate.Value.Date <= GetMonthEnd(endMonth)
                                                                                         && !string.Equals(r.Nationality, "TW", StringComparison.OrdinalIgnoreCase));
            ws.Cells[summaryStartRow + 6, months.Count + 2].Value = $"{Math.Round(monthlyRates.DefaultIfEmpty(0).Average(), 2)}%";

            ApplyTableBorder(ws, summaryStartRow + 1, 1, summaryStartRow + 6, months.Count + 2);
            SetColumnWidthsDynamic(ws, months.Count + 2, 12d);
            ws.Column(1).Width = 16;
            ws.Column(2).Width = 10;
        }

        private string GetDivisionPrefix()
        {
            string deptId = TPConfigs.LoginUser?.IdDepartment ?? "";
            return deptId.Length >= 2 ? deptId.Substring(0, 2) : deptId;
        }

        private List<dm_User> GetUsersForExport()
        {
            string idDept2Word = GetDivisionPrefix();

            if (TPConfigs.IdParentControl == AppPermission.SysAdmin)
            {
                return dm_UserBUS.Instance.GetList();
            }

            if (TPConfigs.IdParentControl == AppPermission.Mod)
            {
                string idDept1Word = (TPConfigs.LoginUser?.IdDepartment ?? "").Length >= 1 ? TPConfigs.LoginUser.IdDepartment.Substring(0, 1) : "";
                return dm_UserBUS.Instance.GetListByDept(idDept1Word);
            }

            if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            {
                return dm_UserBUS.Instance.GetListByDept(idDept2Word);
            }

            return dm_UserBUS.Instance.GetList();
        }

        private IEnumerable<dm_Departments> GetDepartmentsForExport(List<dm_Departments> allDepts)
        {
            string idDept2Word = GetDivisionPrefix();
            string idDept1Word = (TPConfigs.LoginUser?.IdDepartment ?? "").Length >= 1 ? TPConfigs.LoginUser.IdDepartment.Substring(0, 1) : "";

            if (TPConfigs.IdParentControl == AppPermission.SysAdmin)
            {
                return allDepts;
            }

            if (TPConfigs.IdParentControl == AppPermission.Mod)
            {
                return allDepts.Where(r => (r.Id ?? "").StartsWith(idDept1Word, StringComparison.OrdinalIgnoreCase));
            }

            if (TPConfigs.IdParentControl == AppPermission.SafetyCertMain || TPConfigs.IdParentControl == AppPermission.WorkManagementMain)
            {
                return allDepts.Where(r => (r.Id ?? "").StartsWith(idDept2Word, StringComparison.OrdinalIgnoreCase));
            }

            return allDepts;
        }

        private bool IsEmployedOnDate(dm_User user, DateTime date)
        {
            if (user == null)
            {
                return false;
            }

            return user.DateCreate.Date <= date.Date
                && (!user.ResignDate.HasValue || user.ResignDate.Value.Date > date.Date);
        }

        private bool IsIncludedInCurrentSheets(dm_User user)
        {
            return user != null
                && user.Status.HasValue
                && (user.Status.Value == 0 || user.Status.Value == 2);
        }

        private IEnumerable<DateTime> GetMonthSequence(DateTime startMonth, DateTime endMonth)
        {
            DateTime current = new DateTime(startMonth.Year, startMonth.Month, 1);
            DateTime end = new DateTime(endMonth.Year, endMonth.Month, 1);

            while (current <= end)
            {
                yield return current;
                current = current.AddMonths(1);
            }
        }

        private DateTime GetMonthEnd(DateTime month)
        {
            return new DateTime(month.Year, month.Month, 1).AddMonths(1).AddDays(-1);
        }

        private void ApplyDefaultSheetStyle(ExcelWorksheet ws)
        {
            ws.Cells.Style.Font.Name = "DFKai-SB";
            ws.Cells.Style.Font.Size = 12;
            ws.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            ws.Cells.Style.WrapText = true;
        }

        private void WriteHeaderRow(ExcelWorksheet ws, int row, string[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cells[row, i + 1].Value = headers[i];
            }

            ws.Cells[row, 1, row, headers.Length].Style.Font.Bold = true;
            ws.Cells[row, 1, row, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[row, 1, row, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(242, 242, 242));
        }

        private void ApplyTableBorder(ExcelWorksheet ws, int fromRow, int fromCol, int toRow, int toCol)
        {
            var range = ws.Cells[fromRow, fromCol, toRow, toCol];
            range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        private void SetColumnWidths(ExcelWorksheet ws, double[] widths)
        {
            for (int i = 0; i < widths.Length; i++)
            {
                ws.Column(i + 1).Width = widths[i];
            }
        }

        private void SetColumnWidthsDynamic(ExcelWorksheet ws, int columnCount, double defaultWidth)
        {
            for (int i = 1; i <= columnCount; i++)
            {
                ws.Column(i).Width = defaultWidth;
            }
        }

        private void gvData_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRowCell && e.HitInfo.InDataRow && IsSysAdmin)
            {
                e.Menu.Items.Add(itemEditRole);
                e.Menu.Items.Add(itemEditSign);
                e.Menu.Items.Add(itemEditGroup);
            }
        }

        private class DepartmentHeadcountManageForm : XtraForm
        {
            readonly BindingSource source = new BindingSource();
            readonly GridControl grid = new GridControl();
            readonly DevExpress.XtraGrid.Views.Grid.GridView view = new DevExpress.XtraGrid.Views.Grid.GridView();
            readonly SimpleButton btnSave = new SimpleButton();
            readonly SimpleButton btnClose = new SimpleButton();

            public DepartmentHeadcountManageForm()
            {
                Text = "部門編制管理";
                Width = 900;
                Height = 600;
                StartPosition = FormStartPosition.CenterParent;

                grid.Dock = DockStyle.Fill;
                grid.MainView = view;
                grid.ViewCollection.Add(view);
                Controls.Add(grid);

                var panel = new PanelControl { Dock = DockStyle.Bottom, Height = 56 };
                btnSave.Text = "儲存";
                btnSave.Width = 100;
                btnSave.Left = 12;
                btnSave.Top = 12;
                btnSave.Click += BtnSave_Click;
                btnSave.ImageOptions.SvgImage = TPSvgimages.Confirm;

                btnClose.Text = "關閉";
                btnClose.Width = 100;
                btnClose.Left = 120;
                btnClose.Top = 12;
                btnClose.Click += (s, e) => Close();
                btnClose.ImageOptions.SvgImage = TPSvgimages.Close;

                panel.Controls.Add(btnSave);
                panel.Controls.Add(btnClose);
                Controls.Add(panel);

                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowAutoFilterRow = true;
                view.OptionsView.ColumnAutoWidth = false;
                view.Appearance.HeaderPanel.Font = new Font("Microsoft JhengHei UI", 14.25F);
                view.Appearance.HeaderPanel.Options.UseFont = true;
                view.Appearance.Row.Font = new Font("Microsoft JhengHei UI", 12F);
                view.Appearance.Row.Options.UseFont = true;
                view.OptionsBehavior.Editable = true;

                LoadData();
            }

            private void LoadData()
            {
                var data = dm_DeptBUS.Instance.GetList()
                    .OrderBy(r => r.Id)
                    .Select(r => new DepartmentHeadcountRow
                    {
                        Id = r.Id,
                        DisplayName = r.DisplayName,
                        DisplayNameVN = r.DisplayNameVN,
                        AuthorizedHeadcount = r.AuthorizedHeadcount
                    })
                    .ToList();

                source.DataSource = data;
                grid.DataSource = source;

                if (view.Columns.Count == 0)
                {
                    view.PopulateColumns();
                    view.Columns[nameof(DepartmentHeadcountRow.Id)].Caption = "部門代號";
                    view.Columns[nameof(DepartmentHeadcountRow.DisplayName)].Caption = "部門名稱";
                    view.Columns[nameof(DepartmentHeadcountRow.DisplayNameVN)].Caption = "越文名稱";
                    view.Columns[nameof(DepartmentHeadcountRow.AuthorizedHeadcount)].Caption = "編制";

                    view.Columns[nameof(DepartmentHeadcountRow.Id)].OptionsColumn.ReadOnly = true;
                    view.Columns[nameof(DepartmentHeadcountRow.DisplayName)].OptionsColumn.ReadOnly = true;
                    view.Columns[nameof(DepartmentHeadcountRow.DisplayNameVN)].OptionsColumn.ReadOnly = true;
                }

                view.BestFitColumns();
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                var rows = source.DataSource as List<DepartmentHeadcountRow>;
                if (rows == null)
                {
                    return;
                }

                foreach (var row in rows)
                {
                    var dept = dm_DeptBUS.Instance.GetItemById(row.Id);
                    if (dept == null)
                    {
                        continue;
                    }

                    dept.AuthorizedHeadcount = row.AuthorizedHeadcount;
                    dm_DeptBUS.Instance.AddOrUpdate(dept);
                }

                XtraMessageBox.Show("已儲存部門編制。", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        private class DepartmentHeadcountRow
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string DisplayNameVN { get; set; }
            public int? AuthorizedHeadcount { get; set; }
        }
    }
}
