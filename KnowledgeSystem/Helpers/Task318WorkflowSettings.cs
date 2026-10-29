using BusinessLayer;
using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;

namespace KnowledgeSystem.Helpers
{
    internal sealed class Task318PathSettings
    {
        public string Source7810 { get; set; }
        public string Source7820 { get; set; }
        public string CommonRoot { get; set; }
    }

    internal static class Task318WorkflowSettings
    {
        private static string PathsFile => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "7system", "Module318", "paths.config");

        public static Task318PathSettings LoadPaths(string filePath = null)
        {
            filePath = filePath ?? PathsFile;
            var values = new Task318PathSettings
            {
                Source7810 = ConfigurationManager.AppSettings["Module318:SourceTuan"] ?? "",
                Source7820 = ConfigurationManager.AppSettings["Module318:SourceHung"] ?? "",
                CommonRoot = ConfigurationManager.AppSettings["Module318:CommonRoot"] ?? ""
            };
            if (!File.Exists(filePath)) return values;
            var root = XDocument.Load(filePath).Element("Module318Paths")
                ?? throw new ConfigurationErrorsException("路徑設定格式不正確。");
            values.Source7810 = (string)root.Element("Source7810") ?? values.Source7810;
            values.Source7820 = (string)root.Element("Source7820") ?? values.Source7820;
            values.CommonRoot = (string)root.Element("CommonRoot") ?? values.CommonRoot;
            return values;
        }

        public static void SavePaths(Task318PathSettings values, string filePath = null)
        {
            string source7810 = FolderPath(values.Source7810, "7810 下載路徑", true);
            string source7820 = FolderPath(values.Source7820, "7820 下載路徑", true);
            string common = FolderPath(values.CommonRoot, "建立檔案路徑", false);
            if (string.Equals(source7810, source7820, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("7810 與 7820 的下載路徑不能相同。");
            if (IsWithin(common, source7810) || IsWithin(common, source7820))
                throw new InvalidOperationException("建立檔案路徑不能放在下載資料夾內，請選擇另一個資料夾。");

            filePath = filePath ?? PathsFile;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            string temporaryFile = filePath + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                new XDocument(new XElement("Module318Paths",
                    new XElement("Source7810", source7810), new XElement("Source7820", source7820),
                    new XElement("CommonRoot", common))).Save(temporaryFile);
                if (File.Exists(filePath)) File.Replace(temporaryFile, filePath, null);
                else File.Move(temporaryFile, filePath);
            }
            finally { if (File.Exists(temporaryFile)) File.Delete(temporaryFile); }
        }

        private static string FolderPath(string value, string caption, bool mustExist)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new InvalidOperationException("請填寫「" + caption + "」。");
            value = value.Trim();
            if (!Path.IsPathRooted(value) || !(value.StartsWith(@"\\")
                || (value.Length > 2 && value[1] == ':' && (value[2] == '\\' || value[2] == '/'))))
                throw new InvalidOperationException("「" + caption + "」請填寫完整的資料夾路徑。");
            string full = Path.GetFullPath(value);
            if (full.Length > Path.GetPathRoot(full).Length) full = full.TrimEnd('\\', '/');
            if (File.Exists(full) || (mustExist && !Directory.Exists(full)))
                throw new InvalidOperationException("找不到「" + caption + "」的資料夾，請確認路徑及網路連線。");
            return full;
        }

        private static bool IsWithin(string path, string parent)
            => string.Equals(path, parent, StringComparison.OrdinalIgnoreCase)
                || path.StartsWith(parent.TrimEnd('\\', '/') + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);

        public static Task318Participants Participants()
        {
            var result = new Task318Participants
            {
                User7810 = Required("Module318:UserTuan"),
                User7820 = Required("Module318:UserHung")
            };
            result.Validate();
            return result;
        }

        public static string Required(string key, string filePath = null)
        {
            string value;
            string caption = null;
            switch (key)
            {
                case "Module318:SourceTuan": value = LoadPaths(filePath).Source7810; caption = "7810 下載路徑"; break;
                case "Module318:SourceHung": value = LoadPaths(filePath).Source7820; caption = "7820 下載路徑"; break;
                case "Module318:CommonRoot": value = LoadPaths(filePath).CommonRoot; caption = "建立檔案路徑"; break;
                default: value = ConfigurationManager.AppSettings[key]; break;
            }
            if (caption != null && string.IsNullOrWhiteSpace(value))
                throw new ConfigurationErrorsException("請先開啟「設定」，填寫「" + caption + "」。");
            if (string.IsNullOrWhiteSpace(value)) throw new ConfigurationErrorsException($"尚未設定 {key}。");
            return value.Trim();
        }
    }
}
