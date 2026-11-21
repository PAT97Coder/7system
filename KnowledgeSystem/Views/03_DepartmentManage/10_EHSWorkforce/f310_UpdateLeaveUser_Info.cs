using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UpdateLeaveUser_Info : DevExpress.XtraEditors.XtraForm
    {
        public f310_UpdateLeaveUser_Info()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idDataUpdate = -1;
        string userId = "";

        dt310_UpdateLeaveUser updateLeaveUser;

        BindingSource sourceUnitEHSOrg = new BindingSource();
        BindingSource sourceEHSFunction = new BindingSource();
        BindingSource sourceArea5SResponsible = new BindingSource();
        private List<EquipmentHandoverData> equipmentHandovers = new List<EquipmentHandoverData>();

        public class EquipmentHandoverData
        {
            public int EquipmentId { get; set; }
            public string Code { get; set; }
            public string NameVN { get; set; }
            public string NameTW { get; set; }
            public string DeptId { get; set; }
            public string PreviousManagerId { get; set; }
            public string UserId { get; set; }
        }

        public class UpdateLeaveUserData
        {
            public string ActionType { get; set; }
            public string ActionText { get; set; }
            public string UserId { get; set; }
            public string Desc { get; set; }
            public string DeptId { get; set; }
            public Nullable<int> RoleId { get; set; }
            public Nullable<int> FunctionId { get; set; }
            public Nullable<int> AreaId { get; set; }
            public string EmployeeId { get; set; }
            public string AgentId { get; set; }
            public string BossId { get; set; }
            public string AreaCode { get; set; }
            public string AreaName { get; set; }
            public Nullable<DateTime> StartDate { get; set; }
            public dt310_UnitEHSOrg UnitEHSOrgData { get; set; }
            public dt310_Role RoleData { get; set; }
            public dt310_EHSFunction EHSFunctionData { get; set; }
            public dt310_Function FuncData { get; set; }
            public dt310_Area5SResponsible Area5SResponsibleData { get; set; }
            public dt310_Area5S AreaData { get; set; }
            public string FieldName { get; set; }
            public string ColName { get; set; }
        }

        public class UpdateLeaveUserJson
        {
            public List<UpdateLeaveUserData> UnitEHSOrg { get; set; }
            public List<UpdateLeaveUserData> EHSFunction { get; set; }
            public List<UpdateLeaveUserData> Area5SResponsible { get; set; }
            public List<EquipmentHandoverData> Equipment { get; set; }
        }

        private void ConfigureGridEdit(DevExpress.XtraGrid.Views.Grid.GridView view)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
            {
                // Kiểm tra FieldName (dùng OrdinalIgnoreCase để không phân biệt hoa thường cho chắc chắn)
                if (col.FieldName.Equals("UserId", StringComparison.OrdinalIgnoreCase))
                {
                    col.OptionsColumn.AllowEdit = true;
                    // Nếu muốn cho phép sửa nhưng không cho focus vào các ô khác thì mở thêm dòng dưới
                    // col.OptionsColumn.AllowFocus = true; 
                }
                else
                {
                    col.OptionsColumn.AllowEdit = false;
                    // col.OptionsColumn.AllowFocus = false; // Tùy chọn: chặn click chuột vào ô
                }
            }
        }

        private void InitializeIcon()
        {
            btnConfirm.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private void f310_UpdateLeaveUser_Info_Load(object sender, EventArgs e)
        {
            ConfigureGridEdit(gvUnitEHSOrg);
            ConfigureGridEdit(gvEHSFunction);
            ConfigureGridEdit(gvArea5SResponsible);

            updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            if (updateLeaveUser == null || updateLeaveUser.IdGroupProcess == -1 ||
                !dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).Any(r => r.IdGroup == updateLeaveUser.IdGroupProcess))
            {
                MsgTP.MsgNoPermission();
                Close();
                return;
            }
            userId = updateLeaveUser.IdUserLeave;

            var usrs = dm_UserBUS.Instance.GetList().Where(r => r.Status == 0 && r.Id != userId).Select(r => new
            {
                DisplayName = $"LG{r.IdDepartment}/{r.DisplayName}",
                Id = r.Id,
                DeptId = r.IdDepartment
            }).ToList();

            itemCbbUser01.DataSource = usrs;
            itemCbbUser01.DisplayMember = "DisplayName";
            itemCbbUser01.ValueMember = "Id";

            itemCbbUser02.DataSource = usrs;
            itemCbbUser02.DisplayMember = "DisplayName";
            itemCbbUser02.ValueMember = "Id";

            itemCbbUser03.DataSource = usrs;
            itemCbbUser03.DisplayMember = "DisplayName";
            itemCbbUser03.ValueMember = "Id";

            itemCbbUser01.NullText = itemCbbUser02.NullText = itemCbbUser03.NullText = "【請選擇接任人】";
            equipmentUserEditor.DataSource = usrs;
            equipmentHandovers = dt310_EquipmentInfoBUS.Instance.GetListByManagerId(userId)
                .Select(r => new EquipmentHandoverData
                {
                    EquipmentId = r.Id, Code = r.Code, NameVN = r.DisplayNameVN,
                    NameTW = r.DisplayNameTW, DeptId = r.DeptId, PreviousManagerId = r.ManagerId
                }).ToList();
            gvEquipment.GridControl.DataSource = equipmentHandovers;

            var UnitEHSOrgData = dt310_UnitEHSOrgBUS.Instance.GetListByUserId(userId);
            var EHSFunctionData = dt310_EHSFunctionBUS.Instance.GetListByUserId(userId);
            var Area5SResponsibleData = dt310_Area5SResponsibleBUS.Instance.GetListByUserId(userId);

            var roles = dt310_RoleBUS.Instance.GetList();
            var funcs = dt310_FunctionBUS.Instance.GetList();
            var areas = dt310_Area5SBUS.Instance.GetList();

            var UnitEHSOrgUpdate = (from data in UnitEHSOrgData
                                    join role in roles on data.RoleId equals role.Id
                                    select new UpdateLeaveUserData
                                    {
                                        UnitEHSOrgData = data,
                                        RoleData = role,
                                        Desc = $"{data.DeptId}：{role.DisplayName}"
                                    }).ToList();

            var EHSFunctionUpdate = (from data in EHSFunctionData
                                     join func in funcs on data.FunctionId equals func.Id
                                     select new UpdateLeaveUserData
                                     {
                                         EHSFunctionData = data,
                                         FuncData = func,
                                         Desc = $"{data.DeptId}：{func.DisplayName}"
                                     }).ToList();

            var Area5SResponsibleUpdate = (from data in Area5SResponsibleData
                                           join area in areas on data.AreaId equals area.Id
                                           let matchEmp = data.EmployeeId == userId
                                           let matchAgent = data.AgentId == userId
                                           where matchEmp || matchAgent
                                           select new UpdateLeaveUserData
                                           {
                                               Area5SResponsibleData = data,
                                               AreaData = area,
                                               Desc = $"{data.DeptId}：{area.DisplayName}",
                                               FieldName = matchEmp ? "EmployeeId" : "AgentId",
                                               ColName = matchEmp ? "責任人員" : "代理人",
                                           }).ToList();

            sourceUnitEHSOrg.DataSource = UnitEHSOrgUpdate;
            gcUnitEHSOrg.DataSource = sourceUnitEHSOrg;
            gvUnitEHSOrg.ReadOnlyGridView(false);
            gvUnitEHSOrg.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvUnitEHSOrg.BestFitColumns();

            sourceEHSFunction.DataSource = EHSFunctionUpdate;
            gcEHSFunction.DataSource = sourceEHSFunction;
            //gvEHSFunction.ReadOnlyGridView();
            gvEHSFunction.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvEHSFunction.BestFitColumns();

            sourceArea5SResponsible.DataSource = Area5SResponsibleUpdate;
            gcArea5SResponsible.DataSource = sourceArea5SResponsible;
            //gvArea5SResponsible.ReadOnlyGridView();
            gvArea5SResponsible.KeyDown += GridControlHelper.GridViewCopyCellData_KeyDown;
            gvArea5SResponsible.BestFitColumns();
        }

        private void btnConfirm_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // Focus ra khỏi ô đang edit để đảm bảo dữ liệu được lưu
            gvUnitEHSOrg.CloseEditor();
            gvEHSFunction.CloseEditor();
            gvArea5SResponsible.CloseEditor();
            gvEquipment.CloseEditor();
            gvEquipment.UpdateCurrentRow();

            // Focus vào control khác để kích hoạt validation
            this.ActiveControl = null;
            var latest = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            if (latest == null || !latest.IsProcess || latest.DataJson != updateLeaveUser.DataJson ||
                latest.IdGroupProcess != updateLeaveUser.IdGroupProcess || latest.IdGroupProcess == -1 ||
                !dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).Any(r => r.IdGroup == latest.IdGroupProcess))
            {
                MsgTP.MsgNoPermission();
                return;
            }
            var activeUsers = new HashSet<string>(dm_UserBUS.Instance.GetList().Where(r => r.Status == 0 && r.Id != userId).Select(r => r.Id));
            if (equipmentHandovers.Any(r => string.IsNullOrWhiteSpace(r.UserId) || !activeUsers.Contains(r.UserId)))
            {
                MsgTP.MsgError("請為每台設備選擇在職接任人，且不可選擇原管理人。");
                return;
            }
            var currentEquipment = dt310_EquipmentInfoBUS.Instance.GetListByManagerId(userId);
            if (currentEquipment.Count != equipmentHandovers.Count || currentEquipment.Any(r =>
                !equipmentHandovers.Any(h => h.EquipmentId == r.Id && h.DeptId == r.DeptId)))
            {
                MsgTP.MsgError("設備資料已變更，請關閉後重新載入交接申請。");
                return;
            }

            var unitEHSOrgList = sourceUnitEHSOrg.DataSource as List<UpdateLeaveUserData>;
            var ehsFunctionList = sourceEHSFunction.DataSource as List<UpdateLeaveUserData>;
            var area5SResponsibleList = sourceArea5SResponsible.DataSource as List<UpdateLeaveUserData>;

            // Kiểm tra tất cả UserId phải khác null
            var emptyUsers = new List<string>();

            if (unitEHSOrgList != null && unitEHSOrgList.Any(r => string.IsNullOrWhiteSpace(r.UserId)))
            {
                emptyUsers.Add("一。安衛環組織表");
            }

            if (ehsFunctionList != null && ehsFunctionList.Any(r => string.IsNullOrWhiteSpace(r.UserId)))
            {
                emptyUsers.Add("二。各機能設定表");
            }

            if (area5SResponsibleList != null && area5SResponsibleList.Any(r => string.IsNullOrWhiteSpace(r.UserId)))
            {
                emptyUsers.Add("三。紅線區域負責人");
            }

            if (emptyUsers.Count > 0)
            {
                string message = "以下區域尚未選擇接任人，請補充完整：\n\n" + string.Join("\n", emptyUsers);
                XtraMessageBox.Show(message, TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var jsonData = new
            {
                UnitEHSOrg = unitEHSOrgList,
                EHSFunction = ehsFunctionList,
                Area5SResponsible = area5SResponsibleList,
                Equipment = equipmentHandovers
            };

            string json = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            try
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var workflow = context.dt310_UpdateLeaveUser.First(r => r.Id == idDataUpdate);
                    if (!workflow.IsProcess || workflow.IdGroupProcess != latest.IdGroupProcess || workflow.DataJson != latest.DataJson ||
                        !context.dm_GroupUser.Any(r => r.IdUser == TPConfigs.LoginUser.Id && r.IdGroup == workflow.IdGroupProcess))
                        throw new InvalidOperationException("申請狀態或權限已變更，請重新載入。");
                    var owned = context.dt310_EquipmentInfo.Where(r => r.ManagerId == userId && r.DeletedAt == null).ToList();
                    if (owned.Count != equipmentHandovers.Count || owned.Any(r => !equipmentHandovers.Any(h => h.EquipmentId == r.Id && h.DeptId == r.DeptId)))
                        throw new InvalidOperationException("設備資料已變更，請重新載入。");
                    var receiverIds = equipmentHandovers.Select(r => r.UserId)
                        .Concat(unitEHSOrgList.Select(r => r.UserId)).Concat(ehsFunctionList.Select(r => r.UserId))
                        .Concat(area5SResponsibleList.Select(r => r.UserId)).Distinct().ToList();
                    if (receiverIds.Any(r => r == userId) || context.dm_User.Count(r => receiverIds.Contains(r.Id) && r.Status == 0) != receiverIds.Count)
                        throw new InvalidOperationException("接任人必須是在職人員，且不可為原管理人。");
                    var detail = context.dt310_UpdateLeaveUser_detail.First(r => r.IdUpdateData == workflow.Id && r.IdGroup == workflow.IdGroupProcess);
                    var nextStep = context.dt310_UpdateLeaveUser_detail.FirstOrDefault(r => r.IdUpdateData == workflow.Id && r.IndexStep == detail.IndexStep + 1);
                    if (nextStep == null) throw new InvalidOperationException("交接申請缺少下一級審核設定，請聯絡管理員。");
                    detail.IdUser = TPConfigs.LoginUser.Id;
                    detail.TimeSubmit = DateTime.Now;
                    detail.Description = null;
                    workflow.IdGroupProcess = nextStep.IdGroup;
                    workflow.DataJson = json;
                    workflow.IsCancel = false;
                    foreach (var step in context.dt310_UpdateLeaveUser_detail.Where(r => r.IdUpdateData == workflow.Id && r.IndexStep >= nextStep.IndexStep))
                    {
                        step.IdUser = null;
                        step.TimeSubmit = null;
                        step.TimeNotify = null;
                        step.Description = null;
                    }
                    context.SaveChanges();
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                MsgTP.MsgError(ex.Message);
                return;
            }


            XtraMessageBox.Show("已送交二級主管審核！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);

            Close();
        }
    }
}
