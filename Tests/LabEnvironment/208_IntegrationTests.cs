using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using DataAccessLayer;

public static class Lab208IntegrationTests
{
    private static string connectionString;
    private static void Check(bool condition, string message) { if (!condition) throw new Exception(message); }
    private static DataTable Query(string text, params SqlParameter[] parameters)
    {
        using (var connection = new SqlConnection(connectionString))
        using (var command = new SqlCommand(text, connection))
        using (var adapter = new SqlDataAdapter(command))
        { command.Parameters.AddRange(parameters); var table = new DataTable(); adapter.Fill(table); return table; }
    }
    private static void Denied(Action action, string message)
    {
        try { action(); } catch (SqlException) { return; } catch (UnauthorizedAccessException) { return; }
        throw new Exception(message);
    }
    private static string Ingest(string sn, DateTime time, decimal temperature, decimal humidity)
    {
        return Query("EXEC dbo.usp208_IngestReading @SN,@Time,@T,@H",
            new SqlParameter("@SN", sn), new SqlParameter("@Time", SqlDbType.DateTime2) { Value = time },
            new SqlParameter("@T", temperature), new SqlParameter("@H", humidity)).Rows[0]["Result"].ToString();
    }
    public static int Main()
    {
        try
        {
            connectionString = Environment.GetEnvironmentVariable("LAB208_TEST_CONNECTION");
            var bus = new dt208_EnvironmentBUS(connectionString);
            Check(bus.IsInstalled(), "Module not installed");
            Check(bus.GetDepartments("viewer1").Rows.Count == 1, "Viewer can see another department");
            Check(bus.GetDepartments("editorBoth").Rows.Count == 2, "Editor department list incorrect");
            Check(bus.GetDepartments("nobody").Rows.Count == 0, "Unknown actor granted permission");
            var edit = new dt208_DeviceEdit { SN = "LAB-001", DisplayName = "Sensor 1", Model = "M1", IdDept = "D1", Location = "Room A",
                IdManager = "manager", TempMin = 15, TempMax = 25, HumidityMin = 50, HumidityMax = 70, ExpectedMinutes = 60, DeviceState = 0, Reason = "Create" };
            Denied(() => bus.SaveDevice("viewer1", edit), "Viewer can edit");
            int id = bus.SaveDevice("editorBoth", edit);
            edit.Id = id;
            DateTime now = (DateTime)Query("SELECT DATEADD(HOUR,7,SYSUTCDATETIME())").Rows[0][0];
            // Backdate the fixture configuration to exercise delayed readings, without changing production data.
            Query("UPDATE dbo.dt208_DeviceConfigs SET EffectiveFrom=@Date WHERE IdDevice=@Id",
                new SqlParameter("@Date", now.Date.AddDays(-2)), new SqlParameter("@Id", id));
            DateTime yesterday = now.Date.AddDays(-1).AddHours(8);
            Check(Ingest(" lab-001 ", yesterday, 30, 65) == "Accepted", "SN normalization failed");
            Check(Ingest("LAB-001", yesterday, 30, 65) == "Duplicate", "Retry not idempotent");
            Check(Ingest("LAB-001", yesterday, 31, 65) == "Conflict", "Conflict not recorded");
            Check(Ingest("LAB-001", yesterday, 31, 65) == "Conflict", "Conflict retry incorrect");
            Check(Convert.ToInt32(Query("SELECT COUNT(*) FROM dbo.dt208_ReadingConflicts").Rows[0][0]) == 1, "Conflict retries duplicated rows");
            var historical = bus.GetHistory("viewer1", edit.SN, yesterday.Date, now.AddMinutes(5));
            Check(historical.Rows.Count == 1 && (decimal)historical.Rows[0]["Temperature"] == 30, "Original reading overwritten");
            Check(dt208_EnvironmentRules.MeasurementStatus(historical.Rows[0]) == "溫度超限", "Historical limit evaluation incorrect");
            long readingId = Convert.ToInt64(historical.Rows[0]["Id"]);
            Denied(() => bus.AddAction("viewer1", readingId, "A", "B", "C", true), "Viewer can handle alarm");
            Denied(() => bus.AddAction("handler1", readingId, "A", "B", "", true), "Closed action missing result");
            bus.AddAction("handler1", readingId, "Air conditioner", "Repair", "Measured again: normal", true);
            Check(bus.GetActions("viewer1", readingId).Rows.Count == 1, "Action audit not visible");
            Check(bus.GetActions("viewer2", readingId).Rows.Count == 0, "Other department can view actions");
            Check(bus.GetConflicts("viewer2", readingId).Rows.Count == 0, "Other department can view conflicts");
            var current = bus.GetDevices("viewer1", "D1").Rows[0];
            Check(dt208_EnvironmentRules.DeviceStatus(current, now).Contains("逾時"), "Stale readings treated as current");
            Check(dt208_EnvironmentRules.DeviceStatus(current, now).Contains("超限"), "Stale status hides alarm");
            edit.ExpectedConfigId = Convert.ToInt64(current["ConfigId"]);
            edit.IdDept = "D2"; edit.Location = "Room B"; edit.DisplayName = "Sensor renamed"; edit.TempMin = 18; edit.TempMax = 22; edit.Reason = "Move";
            Denied(() => bus.SaveDevice("editor1", edit), "Move allowed without destination permission");
            bus.SaveDevice("editorBoth", edit);
            Denied(() => bus.SaveDevice("editorBoth", edit), "Lost-update check missing");
            Check(bus.GetDevices("viewer1", "D2").Rows.Count == 0, "Department read protection missing");
            Check(bus.GetDevices("viewer1", "D1").Rows.Count == 0, "Device still shown in old department");
            Check(bus.GetHistory("viewer2", edit.SN, yesterday.Date, now.AddMinutes(5)).Rows.Count == 0, "Moved device leaks old department readings");
            Check(bus.GetHistory("viewer1", edit.SN, yesterday.Date, now.AddMinutes(5)).Rows.Count == 1, "Old department history lost after move");
            var moved = bus.GetDevices("viewer2", "D2").Rows[0];
            Check(moved.IsNull("MeasuredAt"), "Old configuration reading evaluated with new limits");
            DateTime movedAt = (DateTime)moved["EffectiveFrom"];
            Check(Ingest(edit.SN, movedAt.AddMilliseconds(1), 22, 70) == "Accepted", "New department reading rejected");
            var latest = bus.GetDevices("viewer2", "D2").Rows[0];
            Check(dt208_EnvironmentRules.MeasurementStatus(latest) == "正常", "Inclusive limits incorrect");
            Ingest(edit.SN, yesterday.AddMinutes(1), 24, 65);
            Check((DateTime)bus.GetDevices("viewer2", "D2").Rows[0]["MeasuredAt"] == movedAt.AddMilliseconds(1), "Delayed reading replaced latest reading");
            Check((decimal)bus.GetHistory("viewer1", edit.SN, yesterday.Date, now.AddMinutes(5)).Rows[0]["TempMax"] == 25, "Delayed reading uses wrong limit version");
            Check(bus.GetConfigurationHistory("viewer1", id).Rows.Count == 1, "Configuration history leaks destination department");
            Check(bus.GetConfigurationHistory("admin", id).Rows.Count == 2, "Admin cannot view config history");
            Check(bus.GetConfigurationHistory("admin", id).Rows[1]["DisplayName"].ToString() == "Sensor 1", "Name audit missing");
            Ingest("UNKNOWN-001", now, 20, 60);
            Check(bus.GetUnknownDevices("admin").Rows.Count == 1, "Unknown SN not retained");
            Denied(() => bus.GetUnknownDevices("viewer1"), "Viewer can view unassigned SN");
            Denied(() => Ingest("BAD-H", now, 20, 101), "Humidity validation missing");
            Denied(() => Ingest(new string('X',101), now, 20, 60), "Long SN silently truncated");
            Denied(() => Ingest("FUTURE", now.AddHours(1), 20, 60), "Future timestamp validation missing");
            var concurrent = Enumerable.Range(0, 12).Select(i => Task.Run(() => Ingest("CONCURRENT", now, 20, 60))).ToArray();
            Task.WaitAll(concurrent);
            Check(concurrent.Count(t => t.Result == "Accepted") == 1, "Concurrent ingest creates duplicate reading");
            Query("CREATE USER lab208_ingest_test WITHOUT LOGIN; ALTER ROLE lab_environment_ingest ADD MEMBER lab208_ingest_test;");
            Query("EXECUTE AS USER='lab208_ingest_test'; EXEC dbo.usp208_IngestReading 'ROLE-TEST',@Time,20,60; REVERT;", new SqlParameter("@Time", now));
            Denied(() => Query("EXECUTE AS USER='lab208_ingest_test'; INSERT dbo.dt208_Readings(SN,MeasuredAt,Temperature,Humidity) VALUES('FORBIDDEN',GETDATE(),20,60); REVERT;"), "Ingest role can bypass procedure");
            Check(dt208_EnvironmentRules.IsOutside(25, 15, 25) == false, "Upper boundary flagged");
            Check(dt208_EnvironmentRules.IsOutside(15, 15, 25) == false, "Lower boundary flagged");
            Console.WriteLine("PASS: department permissions; inclusive limits; delayed/stale data; immutable originals; retry/conflict/concurrency; unknown SN; historical configurations; optimistic updates; action audit; restricted ingest role.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.GetType().Name + ": " + ex.Message);
            return 1;
        }
    }
}
