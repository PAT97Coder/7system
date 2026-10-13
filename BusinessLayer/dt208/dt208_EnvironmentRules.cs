using DataAccessLayer;
using System;
using System.Data;

namespace BusinessLayer
{
    public static class dt208_EnvironmentRules
    {
        public static DateTime VietnamNow => TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time");
        public static string[] DeviceStates = { "使用中", "暫停", "故障", "停用" };
        public static string Validate(dt208_DeviceEdit device)
        {
            if (string.IsNullOrWhiteSpace(device.SN) || device.SN.Trim().Length>100 || string.IsNullOrWhiteSpace(device.DisplayName)
                || device.DisplayName.Trim().Length>200 || string.IsNullOrWhiteSpace(device.IdDept) || string.IsNullOrWhiteSpace(device.Location)
                || device.Location.Trim().Length>200 || string.IsNullOrWhiteSpace(device.IdManager) || string.IsNullOrWhiteSpace(device.Reason))
                return "請填寫SN、名稱、部門、位置、管理人與變更原因。";
            if ((device.Model?.Length ?? 0)>100 || device.Reason.Length>500) return "型號或變更原因過長。";
            if (device.TempMin>=device.TempMax || device.HumidityMin<0 || device.HumidityMax>100 || device.HumidityMin>=device.HumidityMax)
                return "下限必須小於上限；濕度範圍為0–100 %RH。";
            if (device.ExpectedMinutes<1 || device.ExpectedMinutes>10080 || device.DeviceState<0 || device.DeviceState>3)
                return "更新週期為1–10080分鐘，請選擇設備狀態。";
            return null;
        }
        public static bool IsOutside(decimal value,decimal min,decimal max) => value<min || value>max;
        public static string MeasurementStatus(DataRow row)
        {
            if (row.IsNull("TempMin")) return "未有有效設定";
            bool temperature=IsOutside(Convert.ToDecimal(row["Temperature"]),Convert.ToDecimal(row["TempMin"]),Convert.ToDecimal(row["TempMax"]));
            bool humidity=IsOutside(Convert.ToDecimal(row["Humidity"]),Convert.ToDecimal(row["HumidityMin"]),Convert.ToDecimal(row["HumidityMax"]));
            return temperature && humidity ? "溫度及濕度超限" : temperature ? "溫度超限" : humidity ? "濕度超限" : "正常";
        }
        public static string DeviceStatus(DataRow row,DateTime now)
        {
            int state=Convert.ToInt32(row["DeviceState"]);
            if(state!=0) return DeviceStates[state];
            if(row.IsNull("MeasuredAt")) return "尚無資料";
            string measured=MeasurementStatus(row);
            bool stale=now-(DateTime)row["MeasuredAt"]>TimeSpan.FromMinutes(Convert.ToInt32(row["ExpectedMinutes"]));
            string result=stale ? "資料逾時 / "+measured : measured;
            if(Convert.ToInt32(row["ConflictCount"])>0) result+=" / 資料衝突";
            return result;
        }
        public static string Range(decimal min,decimal max,string unit) => $"{(min+max)/2:0.##} ± {(max-min)/2:0.##} {unit} ({min:0.##}–{max:0.##})";
    }
}
