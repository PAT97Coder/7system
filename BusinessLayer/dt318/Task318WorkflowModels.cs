using DataAccessLayer;
using System;
using System.Collections.Generic;

namespace BusinessLayer
{
    public sealed class Task318Participants
    {
        public string User7810 { get; set; }
        public string User7820 { get; set; }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(User7810) || string.IsNullOrWhiteSpace(User7820)
                || User7810.Length > 10 || User7820.Length > 10
                || string.Equals(User7810, User7820, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("請設定兩位不同的 7810 / 7820 收件人。");
        }

        public string GetTeam(string userId)
        {
            if (string.Equals(User7810, userId, StringComparison.OrdinalIgnoreCase)) return "7810";
            if (string.Equals(User7820, userId, StringComparison.OrdinalIgnoreCase)) return "7820";
            return null;
        }

        public string GetUser(string team)
        {
            if (team == "7810") return User7810;
            if (team == "7820") return User7820;
            throw new InvalidOperationException("課別必須是 7810 或 7820。");
        }
    }

    public sealed class Task318StepDraft
    {
        public int Id { get; set; }
        public string TenBuoc { get; set; }
        public string VaiTro { get; set; }
        public string IdNguoiXuLy { get; set; }
        public int? IdNhomXuLy { get; set; }
        public string TrangThai { get; set; }
    }

    public sealed class Task318UserOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string JobName { get; set; }
    }

    public sealed class Task318GroupOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class Task318TeamSnapshot
    {
        public dt318_TaskTeams Team { get; set; }
        public bool? Decision { get; set; }
        public List<dt318_TaskWorkflowSteps> Steps { get; set; }
        public bool CanEdit { get; set; }
        public bool CanComplete { get; set; }
    }

    public sealed class Task318WorkflowSnapshot
    {
        public dt318_Tasks Task { get; set; }
        public List<Task318TeamSnapshot> Teams { get; set; }
        public List<dt318_TaskHistory> History { get; set; }
        public List<Task318UserOption> Users { get; set; }
        public List<Task318GroupOption> Groups { get; set; }
    }

    public static class Task318WorkflowState
    {
        public static string Caption(string state)
        {
            switch (state)
            {
                case "ChuaXacNhan": case "ChoXacNhanTo": return "待確認參與課別";
                case "KhongThamGia": return "不參與";
                case "ChoXuLy": return "待設定流程";
                case "DangXuLy": return "處理中";
                case "ChoXacNhan": return "待後續確認";
                case "HoanThanh": return "已完成";
                case "ChuaDen": return "尚未輪到";
                case "DangCho": return "待本步驟處理";
                case "DaXacNhan": return "已確認";
                default: return state;
            }
        }
    }
}
