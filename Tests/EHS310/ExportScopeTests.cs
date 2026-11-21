using System;
using System.Collections.Generic;
using System.Reflection;
using BusinessLayer;
using DataAccessLayer;
using DevExpress.XtraBars;
using KnowledgeSystem.Helpers;
using KnowledgeSystem.Views._03_DepartmentManage._10_EHSWorkforce;

class ExportScopeTests
{
    static readonly BindingFlags Private = BindingFlags.Instance | BindingFlags.NonPublic;
    static void Assert(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
        Console.WriteLine("PASS: " + message);
    }
    [STAThread]
    static void Main()
    {
        TPConfigs.LoginUser = new dm_User { Id = "VNW0014732", IdDepartment = "7730" };
        using (var control = new uc310_Area5SResponsible())
        {
            var type = control.GetType();
            var button = (BarButtonItem)type.GetField("btnExportAll", Private).GetValue(control);
            Assert(button.Visibility == BarItemVisibility.Never, "Full export hidden before permissions load");
            Assert(button.ImageOptions.SvgImage != null && button.ImageOptions.SvgImageSize.Width == 32,
                "Full export uses shared 32px SVG");
            type.GetField("users", Private).SetValue(control, new List<dm_User>());
            type.GetField("areaResps", Private).SetValue(control, new List<dt310_Area5SResponsible> {
                new dt310_Area5SResponsible { AreaId=1, DeptId="7730", AreaCode="A", AreaName="原料", EmployeeId="VNW0000001" },
                new dt310_Area5SResponsible { AreaId=1, DeptId="7720", AreaCode="B", AreaName="快分" },
                new dt310_Area5SResponsible { AreaId=1, DeptId="7420", AreaCode="C", AreaName="軋鋼" },
                new dt310_Area5SResponsible { AreaId=1, DeptId="7810", AreaCode="D", AreaName="物理" },
                new dt310_Area5SResponsible { AreaId=1, DeptId="7100", AreaCode="E", DeletedAt=DateTime.Now },
                new dt310_Area5SResponsible { AreaId=2, DeptId="7730", AreaCode="F" }
            });
            var method = type.GetMethod("BuildAreaReportRows", Private);
            var scoped = (List<object[]>)method.Invoke(control, new object[] {1, false});
            var all = (List<object[]>)method.Invoke(control, new object[] {1, true});
            Assert(scoped.Count == 2, "Department export includes both 7720 and 7730 only");
            Assert(all.Count == 4, "Full export includes 74, 77 and 78; excludes deleted rows and other areas");
            Assert((string)all[0][3] == "VNW0000001", "Missing employee profile retains ID");
            TPConfigs.LoginUser.IdDepartment = "7420";
            scoped = (List<object[]>)method.Invoke(control, new object[] {1, false});
            Assert(scoped.Count == 1 && (string)scoped[0][2] == "7420", "Department scope follows login department");
        }
        Assert(!dt310_Area5SResponsibleBUS.Instance.CanExportAll(null), "Anonymous user cannot export all");
        bool denied = false;
        try { dt310_Area5SResponsibleBUS.Instance.GetListForFullExport(null); }
        catch (UnauthorizedAccessException) { denied = true; }
        Assert(denied, "Business layer rejects unauthorized full export before reading data");
    }
}
