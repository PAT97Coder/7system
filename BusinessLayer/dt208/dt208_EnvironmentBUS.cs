using DataAccessLayer;
using System;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;

namespace BusinessLayer
{
    // The existing EF model is database-first. Keep new tables out of generated EDMX files.
    public sealed class dt208_EnvironmentBUS
    {
        private static dt208_EnvironmentBUS instance;
        public static dt208_EnvironmentBUS Instance => instance ?? (instance = new dt208_EnvironmentBUS());
        private readonly string connectionString;
        private dt208_EnvironmentBUS() : this(new EntityConnectionStringBuilder(SingleConnection.ConString).ProviderConnectionString) { }
        public dt208_EnvironmentBUS(string providerConnectionString) { connectionString = providerConnectionString; }

        private DataTable Query(string sql, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(sql, connection))
            using (var adapter = new SqlDataAdapter(command))
            {
                command.CommandTimeout = 30;
                command.Parameters.AddRange(parameters);
                var result = new DataTable();
                adapter.Fill(result);
                return result;
            }
        }
        private static SqlParameter Text(string name, string value, int size = 50)
        {
            return new SqlParameter(name, SqlDbType.NVarChar, size) { Value = (object)value ?? DBNull.Value };
        }
        private static SqlParameter Number(string name, decimal value, byte precision = 8)
        {
            return new SqlParameter(name, SqlDbType.Decimal) { Precision = precision, Scale = 2, Value = value };
        }
        public bool IsInstalled()
        {
            return Convert.ToBoolean(Query("SELECT CAST(CASE WHEN OBJECT_ID('dbo.usp208_SaveDevice') IS NOT NULL THEN 1 ELSE 0 END AS BIT) Installed").Rows[0][0]);
        }
        public DataTable GetDepartments(string userId)
        {
            return Query(@"SELECT Id,DisplayName,
 dbo.fn208_EnvironmentPermission(@UserId,Id,'EDIT') CanEdit,dbo.fn208_EnvironmentPermission(@UserId,Id,'HANDLE') CanHandle
 FROM dbo.dm_Departments WHERE IsActive=1 AND dbo.fn208_EnvironmentPermission(@UserId,Id,'VIEW')=1 ORDER BY Id", Text("@UserId", userId));
        }
        public DataTable GetManagers()
        {
            return Query("SELECT Id,DisplayName FROM dbo.dm_User ORDER BY Id");
        }
        public bool IsAdmin(string userId)
        {
            return Convert.ToBoolean(Query(@"SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM dbo.dm_Group g
 JOIN dbo.dm_GroupUser u ON u.IdGroup=g.Id WHERE u.IdUser=@UserId AND g.DisplayName=N'溫濕度【管理】' AND g.IdDept='*')
 THEN 1 ELSE 0 END AS BIT)", Text("@UserId",userId)).Rows[0][0]);
        }
        public DataTable GetDevices(string userId, string deptId)
        {
            return Query(@"SELECT d.Id,d.SN,d.DisplayName,d.Model,c.Id ConfigId,c.IdDept,c.Location,c.IdManager,
 u.DisplayName ManagerName,c.TempMin,c.TempMax,c.HumidityMin,c.HumidityMax,c.ExpectedMinutes,c.DeviceState,c.EffectiveFrom,
 r.Id ReadingId,r.MeasuredAt,r.ReceivedAt,r.Temperature,r.Humidity,
 (SELECT COUNT(*) FROM dbo.dt208_ReadingConflicts x WHERE x.ReadingId=r.Id) ConflictCount
 FROM dbo.dt208_Devices d
 CROSS APPLY(SELECT TOP 1 * FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id ORDER BY EffectiveFrom DESC)c
 LEFT JOIN dbo.dm_User u ON u.Id=c.IdManager
 OUTER APPLY(SELECT TOP 1 * FROM dbo.dt208_Readings WHERE SN=d.SN AND MeasuredAt>=c.EffectiveFrom ORDER BY MeasuredAt DESC)r
 WHERE c.IdDept=@IdDept AND dbo.fn208_EnvironmentPermission(@UserId,c.IdDept,'VIEW')=1 ORDER BY c.Location,d.SN",
                Text("@UserId",userId),Text("@IdDept",deptId));
        }
        public DataTable GetHistory(string userId, string sn, DateTime from, DateTime to)
        {
            return Query(@"SELECT TOP (10001) r.Id,r.SN,r.MeasuredAt,r.ReceivedAt,r.Temperature,r.Humidity,
 c.DisplayName,c.Model,c.IdDept,c.Location,c.TempMin,c.TempMax,c.HumidityMin,c.HumidityMax,
 dbo.fn208_EnvironmentPermission(@UserId,c.IdDept,'HANDLE') CanHandle,
 (SELECT COUNT(*) FROM dbo.dt208_ReadingConflicts x WHERE x.ReadingId=r.Id) ConflictCount,
 a.Cause,a.Action,a.Result,a.IsClosed,a.CreatedBy,a.CreatedAt ActionAt
 FROM dbo.dt208_Readings r JOIN dbo.dt208_Devices d ON d.SN=r.SN
 CROSS APPLY(SELECT TOP 1 IdDept FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id ORDER BY EffectiveFrom DESC)currentConfig
 OUTER APPLY(SELECT TOP 1 * FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id AND EffectiveFrom<=r.MeasuredAt ORDER BY EffectiveFrom DESC)c
 OUTER APPLY(SELECT TOP 1 * FROM dbo.dt208_ReadingActions WHERE ReadingId=r.Id ORDER BY Id DESC)a
 WHERE r.SN=@SN AND r.MeasuredAt>=@From AND r.MeasuredAt<@To
 AND dbo.fn208_EnvironmentPermission(@UserId,COALESCE(c.IdDept,currentConfig.IdDept),'VIEW')=1 ORDER BY r.MeasuredAt DESC",
                Text("@UserId",userId),Text("@SN",sn,100),
                new SqlParameter("@From",SqlDbType.DateTime2){Value=from},new SqlParameter("@To",SqlDbType.DateTime2){Value=to});
        }
        public DataTable GetConfigurationHistory(string userId, int deviceId)
        {
            return Query(@"SELECT c.*,u.DisplayName ManagerName FROM dbo.dt208_DeviceConfigs c LEFT JOIN dbo.dm_User u ON u.Id=c.IdManager
 WHERE IdDevice=@Id AND dbo.fn208_EnvironmentPermission(@UserId,c.IdDept,'VIEW')=1 ORDER BY EffectiveFrom DESC",
                Text("@UserId",userId),new SqlParameter("@Id",SqlDbType.Int){Value=deviceId});
        }
        public DataTable GetActions(string userId, long readingId)
        {
            return Query(@"SELECT a.* FROM dbo.dt208_ReadingActions a JOIN dbo.dt208_Readings r ON r.Id=a.ReadingId
 JOIN dbo.dt208_Devices d ON d.SN=r.SN
 CROSS APPLY(SELECT TOP 1 IdDept FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id AND EffectiveFrom<=r.MeasuredAt ORDER BY EffectiveFrom DESC)c
 WHERE a.ReadingId=@Id AND dbo.fn208_EnvironmentPermission(@UserId,c.IdDept,'VIEW')=1 ORDER BY a.Id DESC",
                Text("@UserId",userId),new SqlParameter("@Id",SqlDbType.BigInt){Value=readingId});
        }
        public DataTable GetUnknownDevices(string userId)
        {
            if (!IsAdmin(userId)) throw new UnauthorizedAccessException("無權查看未登記設備。");
            return Query(@"SELECT SN,COUNT_BIG(*) ReadingCount,MAX(MeasuredAt) LastMeasuredAt FROM dbo.dt208_Readings r
 WHERE NOT EXISTS(SELECT 1 FROM dbo.dt208_Devices d WHERE d.SN=r.SN) GROUP BY SN ORDER BY SN");
        }
        public DataTable GetConflicts(string userId, long readingId)
        {
            return Query(@"SELECT x.Id,x.Temperature,x.Humidity,x.ReceivedAt FROM dbo.dt208_ReadingConflicts x
 JOIN dbo.dt208_Readings r ON r.Id=x.ReadingId JOIN dbo.dt208_Devices d ON d.SN=r.SN
 CROSS APPLY(SELECT TOP 1 IdDept FROM dbo.dt208_DeviceConfigs WHERE IdDevice=d.Id AND EffectiveFrom<=r.MeasuredAt ORDER BY EffectiveFrom DESC)c
 WHERE x.ReadingId=@Id AND dbo.fn208_EnvironmentPermission(@UserId,c.IdDept,'VIEW')=1 ORDER BY x.Id DESC",
                Text("@UserId",userId),new SqlParameter("@Id",SqlDbType.BigInt){Value=readingId});
        }
        public int SaveDevice(string userId, dt208_DeviceEdit edit)
        {
            return Convert.ToInt32(Query(@"EXEC dbo.usp208_SaveDevice @UserId,@Id,@ExpectedConfigId,@SN,@DisplayName,@Model,
 @IdDept,@Location,@IdManager,@TempMin,@TempMax,@HumidityMin,@HumidityMax,@ExpectedMinutes,@DeviceState,@Reason",
                Text("@UserId",userId),new SqlParameter("@Id",SqlDbType.Int){Value=edit.Id},
                new SqlParameter("@ExpectedConfigId",SqlDbType.BigInt){Value=edit.ExpectedConfigId},
                Text("@SN",edit.SN,100),Text("@DisplayName",edit.DisplayName,200),Text("@Model",edit.Model,100),
                Text("@IdDept",edit.IdDept),Text("@Location",edit.Location,200),Text("@IdManager",edit.IdManager),
                Number("@TempMin",edit.TempMin),Number("@TempMax",edit.TempMax),Number("@HumidityMin",edit.HumidityMin,5),Number("@HumidityMax",edit.HumidityMax,5),
                new SqlParameter("@ExpectedMinutes",SqlDbType.Int){Value=edit.ExpectedMinutes},
                new SqlParameter("@DeviceState",SqlDbType.Int){Value=edit.DeviceState},Text("@Reason",edit.Reason,500)).Rows[0][0]);
        }
        public void AddAction(string userId,long readingId,string cause,string action,string result,bool closed)
        {
            Query("EXEC dbo.usp208_AddReadingAction @UserId,@ReadingId,@Cause,@Action,@Result,@IsClosed",
                Text("@UserId",userId),new SqlParameter("@ReadingId",SqlDbType.BigInt){Value=readingId},
                Text("@Cause",cause,1000),Text("@Action",action,1000),Text("@Result",result,1000),new SqlParameter("@IsClosed",SqlDbType.Bit){Value=closed});
        }
    }
}
