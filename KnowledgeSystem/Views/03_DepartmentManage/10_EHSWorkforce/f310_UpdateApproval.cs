using BusinessLayer;
using DataAccessLayer;
using DevExpress.CodeParser;
using DevExpress.XtraEditors;
using KnowledgeSystem.Helpers;
using Newtonsoft.Json;
using Scriban;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce.f310_UpdateLeaveUser_Info;

namespace KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce
{
    public partial class f310_UpdateApproval : DevExpress.XtraEditors.XtraForm
    {
        public f310_UpdateApproval()
        {
            InitializeComponent();
            InitializeIcon();
        }

        public int idDataUpdate = -1;
        dt310_UpdateLeaveUser updateLeaveUser = new dt310_UpdateLeaveUser();

        private void InitializeIcon()
        {
            btnApproval.ImageOptions.SvgImage = TPSvgimages.EmailSend;
            btnCancel.ImageOptions.SvgImage = TPSvgimages.Close;
            btnClose.ImageOptions.SvgImage = TPSvgimages.Confirm;
        }

        private object GetSignatureInfo(dt310_UpdateLeaveUser_detail detail, List<dm_User> users)
        {
            if (detail == null || string.IsNullOrWhiteSpace(detail.IdUser))
                return null;

            var user = users.FirstOrDefault(x => x.Id == detail.IdUser);
            if (user == null)
                return null;

            bool isReject = !string.IsNullOrWhiteSpace(detail.Description);
            string status = isReject ? "已退回" : "已簽名";

            return new
            {
                name = user.DisplayName,
                time = detail.TimeSubmit?.ToString("yyyy/MM/dd HH:mm") ?? "",
                isreject = isReject ? "true" : "false",
                status = status,
                statusColor = isReject ? "red" : "green",
                reason = detail.Description ?? ""
            };
        }

        private object GetCreateSignatureInfo(dt310_UpdateLeaveUser record, List<dm_User> users)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.CreateBy))
                return null;

            var user = users.FirstOrDefault(x => x.Id == record.CreateBy);
            if (user == null)
                return null;

            return new
            {
                name = user.DisplayName,
                time = record.CreateAt.ToString("yyyy/MM/dd HH:mm"),
                isreject = "false",
                status = "已送出",
                statusColor = "green",
                reason = ""
            };
        }

        private object GetEmptySignatureInfo()
        {
            return new
            {
                name = "",
                time = "",
                isreject = "false",
                status = "",
                statusColor = "green",
                reason = ""
            };
        }

        private static string NormalizeChangeAction(string actionType)
        {
            if (string.Equals(actionType, "Create", StringComparison.OrdinalIgnoreCase))
            {
                return "Create";
            }

            if (string.Equals(actionType, "Delete", StringComparison.OrdinalIgnoreCase))
            {
                return "Delete";
            }

            return "Update";
        }

        private static string GetChangeActionText(string actionType)
        {
            switch (NormalizeChangeAction(actionType))
            {
                case "Create":
                    return "新增";
                case "Delete":
                    return "刪除";
                default:
                    return "修改";
            }
        }

        private static string FormatUserDisplay(string userId, List<dm_User> users)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return "";
            }

            dm_User user = users.FirstOrDefault(u => u.Id == userId);
            return user == null ? userId : $"{userId} {user.DisplayName}";
        }

        private static string BuildDeptRoleText(string deptId, int? roleId, List<dt310_Role> roles)
        {
            string roleName = roles.FirstOrDefault(r => r.Id == roleId)?.DisplayName ?? "";
            return $"{deptId}：{roleName}".Trim('：');
        }

        private static string BuildDeptFunctionText(string deptId, int? functionId, List<dt310_Function> funcs)
        {
            string funcName = funcs.FirstOrDefault(r => r.Id == functionId)?.DisplayName ?? "";
            return $"{deptId}：{funcName}".Trim('：');
        }

        private static string BuildAreaContextText(string deptId, int? areaId, List<dt310_Area5S> areas)
        {
            string areaName = areas.FirstOrDefault(r => r.Id == areaId)?.DisplayName ?? "";
            return $"{deptId}：{areaName}".Trim('：');
        }

        private static string BuildAreaPeopleSummary(string employeeId, string agentId, string bossId, List<dm_User> users)
        {
            List<string> parts = new List<string>();

            string employee = FormatUserDisplay(employeeId, users);
            string agent = FormatUserDisplay(agentId, users);
            string boss = FormatUserDisplay(bossId, users);

            if (!string.IsNullOrWhiteSpace(employee))
            {
                parts.Add($"責任:{employee}");
            }

            if (!string.IsNullOrWhiteSpace(agent))
            {
                parts.Add($"代理:{agent}");
            }

            if (!string.IsNullOrWhiteSpace(boss))
            {
                parts.Add($"主管:{boss}");
            }

            return string.Join(" / ", parts);
        }

        private static string BuildUnitDisplayText(UpdateLeaveUserData item, List<dt310_Role> roles)
        {
            string action = NormalizeChangeAction(item?.ActionType);
            string oldText = item?.UnitEHSOrgData != null
                ? BuildDeptRoleText(item.UnitEHSOrgData.DeptId, item.UnitEHSOrgData.RoleId, roles)
                : "";
            string newText = BuildDeptRoleText(
                item?.DeptId ?? item?.UnitEHSOrgData?.DeptId ?? "",
                item?.RoleId ?? item?.UnitEHSOrgData?.RoleId,
                roles);

            switch (action)
            {
                case "Create":
                    return $"[{GetChangeActionText(action)}] {newText}";
                case "Delete":
                    return $"[{GetChangeActionText(action)}] {oldText}";
                default:
                    return oldText != newText
                        ? $"[{GetChangeActionText(action)}] {oldText} -> {newText}"
                        : $"[{GetChangeActionText(action)}] {newText}";
            }
        }

        private static string BuildFuncDisplayText(UpdateLeaveUserData item, List<dt310_Function> funcs)
        {
            string action = NormalizeChangeAction(item?.ActionType);
            string oldText = item?.EHSFunctionData != null
                ? BuildDeptFunctionText(item.EHSFunctionData.DeptId, item.EHSFunctionData.FunctionId, funcs)
                : "";
            string newText = BuildDeptFunctionText(
                item?.DeptId ?? item?.EHSFunctionData?.DeptId ?? "",
                item?.FunctionId ?? item?.EHSFunctionData?.FunctionId,
                funcs);

            switch (action)
            {
                case "Create":
                    return $"[{GetChangeActionText(action)}] {newText}";
                case "Delete":
                    return $"[{GetChangeActionText(action)}] {oldText}";
                default:
                    return oldText != newText
                        ? $"[{GetChangeActionText(action)}] {oldText} -> {newText}"
                        : $"[{GetChangeActionText(action)}] {newText}";
            }
        }

        private static List<object> BuildAreaTemplateRows(List<UpdateLeaveUserData> areaList, List<dm_User> users, List<dt310_Area5S> areas)
        {
            return areaList.Select(item =>
            {
                string action = NormalizeChangeAction(item?.ActionType);
                bool isLegacySingleFieldChange =
                    item?.Area5SResponsibleData != null
                    && !string.IsNullOrWhiteSpace(item.FieldName)
                    && !string.IsNullOrWhiteSpace(item.UserId)
                    && string.IsNullOrWhiteSpace(item.EmployeeId)
                    && string.IsNullOrWhiteSpace(item.AgentId)
                    && string.IsNullOrWhiteSpace(item.BossId);

                if (isLegacySingleFieldChange)
                {
                    string oldUserId = item.FieldName == "EmployeeId"
                        ? item.Area5SResponsibleData.EmployeeId
                        : item.Area5SResponsibleData.AgentId;

                    return (object)new
                    {
                        oldemp = FormatUserDisplay(oldUserId, users),
                        reciver = FormatUserDisplay(item.UserId, users),
                        colname = item.ColName,
                        area = item.Desc,
                        areacode = item.Area5SResponsibleData?.AreaCode ?? "",
                        areaname = item.Area5SResponsibleData?.AreaName ?? "",
                    };
                }

                string oldContext = item?.Area5SResponsibleData != null
                    ? BuildAreaContextText(item.Area5SResponsibleData.DeptId, item.Area5SResponsibleData.AreaId, areas)
                    : "";
                string newContext = BuildAreaContextText(
                    item?.DeptId ?? item?.Area5SResponsibleData?.DeptId ?? "",
                    item?.AreaId ?? item?.Area5SResponsibleData?.AreaId,
                    areas);

                string areaText = action == "Create"
                    ? $"[{GetChangeActionText(action)}] {newContext}"
                    : action == "Delete"
                        ? $"[{GetChangeActionText(action)}] {oldContext}"
                        : oldContext != newContext
                            ? $"[{GetChangeActionText(action)}] {oldContext} -> {newContext}"
                            : $"[{GetChangeActionText(action)}] {newContext}";

                return (object)new
                {
                    oldemp = action == "Create"
                        ? ""
                        : BuildAreaPeopleSummary(
                            item?.Area5SResponsibleData?.EmployeeId,
                            item?.Area5SResponsibleData?.AgentId,
                            item?.Area5SResponsibleData?.BossId,
                            users),
                    reciver = action == "Delete"
                        ? ""
                        : BuildAreaPeopleSummary(
                            item?.EmployeeId ?? item?.Area5SResponsibleData?.EmployeeId,
                            item?.AgentId ?? item?.Area5SResponsibleData?.AgentId,
                            item?.BossId ?? item?.Area5SResponsibleData?.BossId,
                            users),
                    colname = GetChangeActionText(action),
                    area = areaText,
                    areacode = item?.AreaCode ?? item?.Area5SResponsibleData?.AreaCode ?? "",
                    areaname = item?.AreaName ?? item?.Area5SResponsibleData?.AreaName ?? "",
                };
            }).ToList();
        }

        private void f310_UpdateApproval_Load(object sender, EventArgs e)
        {
            try { LoadApproval(); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); Close(); }
        }

        private void LoadApproval()
        {
            var users = dm_UserBUS.Instance.GetList();
            var roles = dt310_RoleBUS.Instance.GetList();
            var funcs = dt310_FunctionBUS.Instance.GetList();
            var areas = dt310_Area5SBUS.Instance.GetList();
            updateLeaveUser = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            if (updateLeaveUser == null) throw new InvalidOperationException("交接申請已刪除或不存在。");
            var dataDetail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdData(idDataUpdate);
            bool isEHSAssign = updateLeaveUser.DataType == "EHSAssign";

            var userLeaveInfo = users.FirstOrDefault(r => r.Id == updateLeaveUser?.IdUserLeave);
            if (isEHSAssign)
            {
                Text = "審核安衛環人員異動申請";
            }
            else if (userLeaveInfo != null)
            {
                Text = $"審核離職人員權限轉移 - {updateLeaveUser.IdUserLeave} {userLeaveInfo.DisplayName}";
            }

            // Nếu đã hoàn thành hoặc đã trả về, chỉ hiển thị nút Confirm
            if (updateLeaveUser.IdGroupProcess == -1 || updateLeaveUser.IsCancel)
            {
                btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
            }
            else
            {
                var currentUserGroups = dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id);
                bool hasPermission = currentUserGroups.Any(g => g.IdGroup == updateLeaveUser.IdGroupProcess);

                if (!hasPermission)
                {
                    btnApproval.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                    btnCancel.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
                else
                {
                    btnClose.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
                }
            }

            string json = updateLeaveUser?.DataJson ?? "{}";
            var data = JsonConvert.DeserializeObject<UpdateLeaveUserJson>(json);

            // Sử dụng data
            var unitList = data?.UnitEHSOrg ?? new List<UpdateLeaveUserData>();
            var funcList = data?.EHSFunction ?? new List<UpdateLeaveUserData>();
            var areaList = data?.Area5SResponsible ?? new List<UpdateLeaveUserData>();

            userLeaveInfo = users.FirstOrDefault(r => r.Id == updateLeaveUser?.IdUserLeave);
            var userLeaveName = userLeaveInfo != null ? $"{updateLeaveUser.IdUserLeave} {userLeaveInfo.DisplayName}" : "";
            var signApplicant = isEHSAssign
                ? GetCreateSignatureInfo(updateLeaveUser, users)
                : GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == 0), users);
            var signLevel2 = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 0 : 1)), users);
            var signPsm = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 1 : 2)), users);
            var signLevel1 = GetSignatureInfo(dataDetail?.FirstOrDefault(r => r.IndexStep == (isEHSAssign ? 2 : 3)), users);

            var templateData = new
            {
                userleave = userLeaveName,
                createby = $"{updateLeaveUser.CreateBy} {users.FirstOrDefault(u => u.Id == updateLeaveUser.CreateBy)?.DisplayName ?? ""}",
                unitlist = unitList.Select(r => new
                {
                    oldemp = NormalizeChangeAction(r.ActionType) == "Create"
                        ? ""
                        : FormatUserDisplay(r.UnitEHSOrgData?.EmployeeId, users),
                    reciver = NormalizeChangeAction(r.ActionType) == "Delete"
                        ? ""
                        : FormatUserDisplay(r.UserId, users),
                    displaydata = BuildUnitDisplayText(r, roles)
                }).ToList(),
                funclist = funcList.Select(r => new
                {
                    oldemp = NormalizeChangeAction(r.ActionType) == "Create"
                        ? ""
                        : FormatUserDisplay(r.EHSFunctionData?.EmployeeId, users),
                    reciver = NormalizeChangeAction(r.ActionType) == "Delete"
                        ? ""
                        : FormatUserDisplay(r.UserId, users),
                    displaydata = BuildFuncDisplayText(r, funcs)
                }).ToList(),
                arealist = BuildAreaTemplateRows(areaList, users, areas),
                equipmentlist = (data?.Equipment ?? new List<EquipmentHandoverData>()).Select(r => new
                {
                    code = System.Net.WebUtility.HtmlEncode(r.Code),
                    namevn = System.Net.WebUtility.HtmlEncode(r.NameVN),
                    nametw = System.Net.WebUtility.HtmlEncode(r.NameTW),
                    dept = System.Net.WebUtility.HtmlEncode(r.DeptId),
                    oldemp = System.Net.WebUtility.HtmlEncode(FormatUserDisplay(r.PreviousManagerId, users)),
                    reciver = System.Net.WebUtility.HtmlEncode(FormatUserDisplay(r.UserId, users))
                }).ToList(),
                sign1st = signApplicant ?? GetEmptySignatureInfo(),
                sign2nd = signLevel2 ?? GetEmptySignatureInfo(),
                sign3th = signPsm ?? GetEmptySignatureInfo(),
                sign4th = signLevel1 ?? GetEmptySignatureInfo(),
            };

            string templateFile = isEHSAssign ? "310_updateehsassign.html" : "310_updateleaveruser.html";
            var templateContentSigner = File.ReadAllText(Path.Combine(TPConfigs.ResourcesPath, templateFile));

            var templateSigner = Template.Parse(templateContentSigner);

            var pageContent = templateSigner.Render(templateData);

            webViewUpdateData.DocumentText = pageContent;
        }

        private void ApplyUnitChange(UpdateLeaveUserData item, DBDocumentManagementSystemEntities context)
        {
            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId) || !item.RoleId.HasValue || string.IsNullOrWhiteSpace(item.UserId))
                    {
                        return;
                    }

                    context.dt310_UnitEHSOrg.Add(new dt310_UnitEHSOrg
                    {
                        DeptId = item.DeptId,
                        RoleId = item.RoleId.Value,
                        EmployeeId = item.UserId,
                        StartDate = item.StartDate ?? DateTime.Now.Date,
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.UnitEHSOrgData != null)
                    {
                        var removedUnit = context.dt310_UnitEHSOrg.FirstOrDefault(r => r.Id == item.UnitEHSOrgData.Id);
                        if (removedUnit == null) throw new InvalidOperationException("組織資料已變更，請重新載入。");
                        removedUnit.DeletedAt = DateTime.Now;
                        removedUnit.DeletedBy = TPConfigs.LoginUser.Id;
                    }
                    break;
                default:
                    if (item?.UnitEHSOrgData == null)
                    {
                        return;
                    }

                    var unitEHSOrg = context.dt310_UnitEHSOrg.FirstOrDefault(r => r.Id == item.UnitEHSOrgData.Id);
                    if (unitEHSOrg == null || unitEHSOrg.DeletedAt.HasValue)
                    {
                        throw new InvalidOperationException("組織資料已變更，請重新載入交接申請。");
                    }

                    unitEHSOrg.DeptId = item.DeptId ?? unitEHSOrg.DeptId;
                    unitEHSOrg.RoleId = item.RoleId ?? unitEHSOrg.RoleId;
                    unitEHSOrg.EmployeeId = item.UserId ?? unitEHSOrg.EmployeeId;
                    unitEHSOrg.StartDate = item.StartDate ?? unitEHSOrg.StartDate;
                    break;
            }
        }

        private void ApplyFuncChange(UpdateLeaveUserData item, DBDocumentManagementSystemEntities context)
        {
            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId) || !item.FunctionId.HasValue || string.IsNullOrWhiteSpace(item.UserId))
                    {
                        return;
                    }

                    context.dt310_EHSFunction.Add(new dt310_EHSFunction
                    {
                        DeptId = item.DeptId,
                        FunctionId = item.FunctionId.Value,
                        EmployeeId = item.UserId,
                        StartDate = item.StartDate ?? DateTime.Now.Date,
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.EHSFunctionData != null)
                    {
                        var removedFunc = context.dt310_EHSFunction.FirstOrDefault(r => r.Id == item.EHSFunctionData.Id);
                        if (removedFunc == null) throw new InvalidOperationException("機能資料已變更，請重新載入。");
                        removedFunc.DeletedAt = DateTime.Now;
                        removedFunc.DeletedBy = TPConfigs.LoginUser.Id;
                    }
                    break;
                default:
                    if (item?.EHSFunctionData == null)
                    {
                        return;
                    }

                    var ehsFunction = context.dt310_EHSFunction.FirstOrDefault(r => r.Id == item.EHSFunctionData.Id);
                    if (ehsFunction == null || ehsFunction.DeletedAt.HasValue)
                    {
                        throw new InvalidOperationException("機能資料已變更，請重新載入交接申請。");
                    }

                    ehsFunction.DeptId = item.DeptId ?? ehsFunction.DeptId;
                    ehsFunction.FunctionId = item.FunctionId ?? ehsFunction.FunctionId;
                    ehsFunction.EmployeeId = item.UserId ?? ehsFunction.EmployeeId;
                    ehsFunction.StartDate = item.StartDate ?? ehsFunction.StartDate;
                    break;
            }
        }

        private void ApplyAreaChange(UpdateLeaveUserData item, DBDocumentManagementSystemEntities context)
        {
            if (item?.Area5SResponsibleData != null && !string.IsNullOrWhiteSpace(item.FieldName) && !string.IsNullOrWhiteSpace(item.UserId))
            {
                var legacyArea = context.dt310_Area5SResponsible.FirstOrDefault(r => r.Id == item.Area5SResponsibleData.Id);
                if (legacyArea == null || legacyArea.DeletedAt.HasValue)
                {
                    throw new InvalidOperationException("區域資料已變更，請重新載入交接申請。");
                }

                if (item.FieldName == "EmployeeId")
                {
                    legacyArea.EmployeeId = item.UserId;
                }
                else if (item.FieldName == "AgentId")
                {
                    legacyArea.AgentId = item.UserId;
                }
                return;
            }

            string action = NormalizeChangeAction(item?.ActionType);

            switch (action)
            {
                case "Create":
                    if (string.IsNullOrWhiteSpace(item?.DeptId)
                        || !item.AreaId.HasValue
                        || string.IsNullOrWhiteSpace(item.EmployeeId)
                        || string.IsNullOrWhiteSpace(item.AgentId)
                        || string.IsNullOrWhiteSpace(item.BossId))
                    {
                        return;
                    }

                    context.dt310_Area5SResponsible.Add(new dt310_Area5SResponsible
                    {
                        DeptId = item.DeptId,
                        AreaId = item.AreaId.Value,
                        EmployeeId = item.EmployeeId,
                        AgentId = item.AgentId,
                        BossId = item.BossId,
                        AreaCode = item.AreaCode ?? "",
                        AreaName = item.AreaName ?? "",
                        CreatedAt = DateTime.Now,
                        CreatedBy = updateLeaveUser.CreateBy
                    });
                    break;
                case "Delete":
                    if (item?.Area5SResponsibleData != null)
                    {
                        var removedArea = context.dt310_Area5SResponsible.FirstOrDefault(r => r.Id == item.Area5SResponsibleData.Id);
                        if (removedArea == null) throw new InvalidOperationException("區域資料已變更，請重新載入。");
                        context.dt310_Area5SResponsible.Remove(removedArea);
                    }
                    break;
                default:
                    if (item?.Area5SResponsibleData == null)
                    {
                        return;
                    }

                    var area5S = context.dt310_Area5SResponsible.FirstOrDefault(r => r.Id == item.Area5SResponsibleData.Id);
                    if (area5S == null || area5S.DeletedAt.HasValue)
                    {
                        throw new InvalidOperationException("區域資料已變更，請重新載入交接申請。");
                    }

                    area5S.DeptId = item.DeptId ?? area5S.DeptId;
                    area5S.AreaId = item.AreaId ?? area5S.AreaId;
                    area5S.EmployeeId = item.EmployeeId ?? area5S.EmployeeId;
                    area5S.AgentId = item.AgentId ?? area5S.AgentId;
                    area5S.BossId = item.BossId ?? area5S.BossId;
                    area5S.AreaCode = item.AreaCode ?? area5S.AreaCode;
                    area5S.AreaName = item.AreaName ?? area5S.AreaName;
                    break;
            }
        }

        private void ApplyRelatedChanges(UpdateLeaveUserJson data, DBDocumentManagementSystemEntities context)
        {
            var receivers = new List<string>();
            receivers.AddRange((data?.UnitEHSOrg ?? new List<UpdateLeaveUserData>())
                .Where(r => NormalizeChangeAction(r.ActionType) != "Delete").Select(r => r.UserId ?? r.UnitEHSOrgData?.EmployeeId));
            receivers.AddRange((data?.EHSFunction ?? new List<UpdateLeaveUserData>())
                .Where(r => NormalizeChangeAction(r.ActionType) != "Delete").Select(r => r.UserId ?? r.EHSFunctionData?.EmployeeId));
            foreach (var area in data?.Area5SResponsible ?? new List<UpdateLeaveUserData>())
            {
                if (NormalizeChangeAction(area.ActionType) == "Delete") continue;
                if (!string.IsNullOrWhiteSpace(area.FieldName)) receivers.Add(area.UserId);
                else
                {
                    receivers.Add(area.EmployeeId ?? area.Area5SResponsibleData?.EmployeeId);
                    receivers.Add(area.AgentId ?? area.Area5SResponsibleData?.AgentId);
                    receivers.Add(area.BossId ?? area.Area5SResponsibleData?.BossId);
                }
            }
            var uniqueIds = receivers.Distinct().ToList();
            if (uniqueIds.Any(string.IsNullOrWhiteSpace) || context.dm_User.Count(r => uniqueIds.Contains(r.Id) && r.Status == 0) != uniqueIds.Count)
                throw new InvalidOperationException("接任人狀態已變更，請退回重新選擇在職接任人。");
            foreach (var item in data?.UnitEHSOrg ?? new List<UpdateLeaveUserData>()) ApplyUnitChange(item, context);
            foreach (var item in data?.EHSFunction ?? new List<UpdateLeaveUserData>()) ApplyFuncChange(item, context);
            foreach (var item in data?.Area5SResponsible ?? new List<UpdateLeaveUserData>()) ApplyAreaChange(item, context);
        }

        private void btnApproval_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { ApproveCurrentWorkflow(); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private dt310_UpdateLeaveUser GetCurrentWorkflow(DBDocumentManagementSystemEntities context)
        {
            var workflow = context.dt310_UpdateLeaveUser.FirstOrDefault(r => r.Id == idDataUpdate);
            if (workflow == null || !workflow.IsProcess || workflow.IsCancel || workflow.IdGroupProcess == -1 ||
                workflow.IdGroupProcess != updateLeaveUser.IdGroupProcess || workflow.DataJson != updateLeaveUser.DataJson ||
                !context.dm_GroupUser.Any(r => r.IdUser == TPConfigs.LoginUser.Id && r.IdGroup == workflow.IdGroupProcess))
                throw new InvalidOperationException("申請狀態或審核權限已變更，請重新載入。");
            return workflow;
        }

        private void ApproveCurrentWorkflow()
        {
            var latest = dt310_UpdateLeaveUserBUS.Instance.GetItemById(idDataUpdate);
            if (latest == null || !latest.IsProcess || latest.IsCancel || latest.IdGroupProcess == -1 ||
                latest.IdGroupProcess != updateLeaveUser.IdGroupProcess || latest.DataJson != updateLeaveUser.DataJson ||
                !dm_GroupUserBUS.Instance.GetListByUID(TPConfigs.LoginUser.Id).Any(r => r.IdGroup == latest.IdGroupProcess))
            {
                MsgTP.MsgError("申請狀態或審核權限已變更，請重新載入。");
                return;
            }
            var detail = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndIdGroup(updateLeaveUser.Id, (int)updateLeaveUser.IdGroupProcess);
            detail.IdUser = TPConfigs.LoginUser.Id;
            detail.TimeSubmit = DateTime.Now;

            var nextStep = dt310_UpdateLeaveUser_detailBUS.Instance.GetItemByIdDataAndStep(updateLeaveUser.Id, detail.IndexStep + 1);

            if (nextStep == null)
            {
                string json = updateLeaveUser?.DataJson ?? "{}";
                var data = JsonConvert.DeserializeObject<UpdateLeaveUserJson>(json);

                try
                {
                    if (updateLeaveUser.DataType != "EHSAssign")
                    {
                        dt310_EquipmentInfoBUS.Instance.TransferManagers(
                            (data?.Equipment ?? new List<EquipmentHandoverData>()).Select(r => new dt310_EquipmentInfoBUS.ManagerChange
                            {
                                EquipmentId = r.EquipmentId, PreviousManagerId = r.PreviousManagerId,
                                DeptId = r.DeptId, NewManagerId = r.UserId
                            }).ToList(), TPConfigs.LoginUser.Id, updateLeaveUser.Id, updateLeaveUser.DataJson,
                            context => ApplyRelatedChanges(data, context));
                    }
                    else
                    {
                        using (var context = new DBDocumentManagementSystemEntities())
                        using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                        {
                            var workflow = GetCurrentWorkflow(context);
                            ApplyRelatedChanges(data, context);
                            var currentDetail = context.dt310_UpdateLeaveUser_detail.First(r => r.Id == detail.Id);
                            currentDetail.IdUser = TPConfigs.LoginUser.Id;
                            currentDetail.TimeSubmit = DateTime.Now;
                            workflow.IsProcess = false;
                            workflow.IdGroupProcess = -1;
                            context.SaveChanges();
                            transaction.Commit();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MsgTP.MsgError(ex.Message);
                    return;
                }
                XtraMessageBox.Show("審核完成！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                using (var context = new DBDocumentManagementSystemEntities())
                using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    var workflow = GetCurrentWorkflow(context);
                    var currentDetail = context.dt310_UpdateLeaveUser_detail.First(r => r.Id == detail.Id);
                    var next = context.dt310_UpdateLeaveUser_detail.First(r => r.IdUpdateData == workflow.Id && r.IndexStep == currentDetail.IndexStep + 1);
                    currentDetail.IdUser = TPConfigs.LoginUser.Id;
                    currentDetail.TimeSubmit = DateTime.Now;
                    workflow.IdGroupProcess = next.IdGroup;
                    context.SaveChanges();
                    transaction.Commit();
                }
                XtraMessageBox.Show("已送交下一級審核！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            Close();
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try { RejectCurrentWorkflow(); }
            catch (Exception ex) { MsgTP.MsgError(ex.Message); }
        }

        private void RejectCurrentWorkflow()
        {
            XtraInputBoxArgs args = new XtraInputBoxArgs
            {
                Caption = TPConfigs.SoftNameTW,
                Prompt = "退回文件原因",
                DefaultButtonIndex = 0,
                Editor = new MemoEdit(),
                DefaultResponse = ""
            };

            var result = XtraInputBox.Show(args);
            if (result == null) return;

            string reason = result.ToString();
            if (string.IsNullOrWhiteSpace(reason))
            {
                XtraMessageBox.Show("請輸入退回原因！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var context = new DBDocumentManagementSystemEntities())
            using (var transaction = context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                var workflow = GetCurrentWorkflow(context);
                var detail = context.dt310_UpdateLeaveUser_detail.First(r => r.IdUpdateData == workflow.Id && r.IdGroup == workflow.IdGroupProcess);
                var firstStep = context.dt310_UpdateLeaveUser_detail.First(r => r.IdUpdateData == workflow.Id && r.IndexStep == 0);
                detail.IdUser = TPConfigs.LoginUser.Id;
                detail.TimeSubmit = DateTime.Now;
                detail.Description = reason;
                workflow.IdGroupProcess = firstStep.IdGroup;
                workflow.IsCancel = true;
                context.SaveChanges();
                transaction.Commit();
            }

            XtraMessageBox.Show("已退回！", TPConfigs.SoftNameTW, MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
        }

        private void btnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Close();
        }
    }
}
