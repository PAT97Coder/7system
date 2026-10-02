using BusinessLayer;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KnowledgeSystem.Helpers
{
    public sealed class Task318ImportSource
    {
        public string RootPath { get; set; }
        public string AssigneeId { get; set; }
        public string FolderName { get; set; }
    }

    public sealed class Task318ImportOptions
    {
        public string CommonRootPath { get; set; }
        public string ImportedBy { get; set; }
        public IList<Task318ImportSource> Sources { get; } = new List<Task318ImportSource>();
    }

    public sealed class Task318ImportItemResult
    {
        public string SourceFolderPath { get; set; }
        public string DestinationFolderPath { get; set; }
        public int TaskId { get; set; }
        public string Subject { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssigneeId { get; set; }
        public bool Ready { get; set; }
        public bool Imported { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
    }

    public sealed class Task318ImportSummary
    {
        public IList<Task318ImportItemResult> Items { get; } =
            new List<Task318ImportItemResult>();

        public int ImportedCount => Items.Count(r => r.Imported);
        public int SkippedCount => Items.Count(r => r.Skipped);
        public int ReadyCount => Items.Count(r => r.Ready);
        public int FailedCount => Items.Count(r => !r.Ready && !r.Imported && !r.Skipped);
    }

    /// <summary>
    /// Nhập các thư mục công việc cá nhân vào thư mục chung của module 318.
    /// Mỗi thư mục con được xem là một công việc độc lập.
    /// </summary>
    public sealed class Task318FolderImportService
    {
        public Task318ImportSummary Preview(Task318ImportOptions options)
        {
            ValidateOptions(options);
            var summary = new Task318ImportSummary();
            foreach (var source in options.Sources)
            {
                if (!Directory.Exists(source.RootPath))
                {
                    AddResult(summary, Failed(
                        source.RootPath,
                        "找不到來源資料夾或目前無法連線。"), source);
                    continue;
                }

                try
                {
                    foreach (string folder in Directory.EnumerateDirectories(
                        source.RootPath,
                        "*",
                        SearchOption.TopDirectoryOnly))
                        AddResult(summary, PreviewFolder(folder, source, options), source);
                }
                catch (Exception ex)
                {
                    AddResult(summary, Failed(source.RootPath, ex.Message), source);
                }
            }

            return summary;
        }

        public Task318ImportSummary Import(Task318ImportOptions options)
        {
            ValidateOptions(options);
            Directory.CreateDirectory(options.CommonRootPath);

            var summary = new Task318ImportSummary();
            foreach (var source in options.Sources)
            {
                if (!Directory.Exists(source.RootPath))
                {
                    AddResult(summary, Failed(
                        source.RootPath,
                        "找不到來源資料夾或目前無法連線。"), source);
                    continue;
                }

                IEnumerable<string> folders;
                try
                {
                    folders = Directory.EnumerateDirectories(
                        source.RootPath,
                        "*",
                        SearchOption.TopDirectoryOnly).ToList();
                }
                catch (Exception ex)
                {
                    AddResult(summary, Failed(source.RootPath, ex.Message), source);
                    continue;
                }

                foreach (string folder in folders)
                    AddResult(summary, ImportFolder(folder, source, options), source);
            }

            return summary;
        }

        private Task318ImportItemResult PreviewFolder(
            string sourceFolder,
            Task318ImportSource source,
            Task318ImportOptions options)
        {
            try
            {
                string sourceKey = CreateSourceKey(sourceFolder);
                if (dt318_TasksBUS.Instance.ExistsBySourceKey(sourceKey))
                    return Skipped(sourceFolder, "此資料夾已匯入。", null);

                var oxpsFiles = GetOxpsFiles(sourceFolder);
                if (oxpsFiles.Count == 0)
                    return Failed(sourceFolder, "資料夾中沒有 XPS/OXPS 檔案。");
                if (oxpsFiles.Count > 1)
                    return Failed(sourceFolder, "資料夾中有多個 XPS/OXPS 檔案，無法判斷來源文件。");

                var document = OxpsParser.ReadSubjectAndDescription(oxpsFiles[0]);
                if (!document.DueDate.HasValue)
                    return Failed(sourceFolder, "XPS/OXPS 中找不到「待覆日期」。");

                return new Task318ImportItemResult
                {
                    SourceFolderPath = sourceFolder,
                    DestinationFolderPath = Path.Combine(
                        options.CommonRootPath,
                        $"{document.DueDate.Value:yyyyMMdd}_{SanitizeFolderName(source.FolderName)}"),
                    Subject = document.Subject,
                    DueDate = document.DueDate.Value.Date,
                    AssigneeId = source.AssigneeId,
                    Ready = true,
                    Message = "準備匯入。"
                };
            }
            catch (Exception ex)
            {
                return Failed(sourceFolder, ex.Message);
            }
        }

        private Task318ImportItemResult ImportFolder(
            string sourceFolder,
            Task318ImportSource source,
            Task318ImportOptions options)
        {
            string sourceKey = CreateSourceKey(sourceFolder);
            try
            {
                if (dt318_TasksBUS.Instance.ExistsBySourceKey(sourceKey))
                    return Skipped(sourceFolder, "此資料夾已匯入。", null);

                var oxpsFiles = GetOxpsFiles(sourceFolder);
                if (oxpsFiles.Count == 0)
                    return Failed(sourceFolder, "資料夾中沒有 XPS/OXPS 檔案。");
                if (oxpsFiles.Count > 1)
                    return Failed(sourceFolder, "資料夾中有多個 XPS/OXPS 檔案，無法判斷來源文件。");

                string oxpsPath = oxpsFiles[0];
                var document = OxpsParser.ReadSubjectAndDescription(oxpsPath);
                if (!document.DueDate.HasValue)
                    return Failed(sourceFolder, "XPS/OXPS 中找不到「待覆日期」。");

                using (var destination = ReserveDestinationFolder(
                    options.CommonRootPath,
                    document.DueDate.Value,
                    source.FolderName))
                {
                    string temporaryFolder = destination.TemporaryPath;
                    string finalFolder = destination.FinalPath;
                    bool finalFolderCreated = false;
                    try
                    {
                        CopyDirectory(sourceFolder, temporaryFolder);
                        Directory.Move(temporaryFolder, finalFolder);
                        finalFolderCreated = true;

                        DateTime now = DateTime.Now;
                        var task = new dt318_Tasks
                        {
                            ChuDe = document.Subject,
                            GiaiThich = document.Description,
                            NgayDenHan = document.DueDate.Value.Date,
                            TrangThai = "ChoXuLy",
                            DuongDanThuMucChung = options.CommonRootPath,
                            DuongDanThuMucCongViec = finalFolder,
                            DuongDanThuMucNguon = sourceFolder,
                            MaNguon = sourceKey,
                            TrangThaiOCR = "HoanThanh",
                            ThongBaoOCR = "Đọc trực tiếp nội dung XPS/OXPS thành công.",
                            ThoiGianOCR = now,
                            NgayTao = now,
                            NguoiTao = options.ImportedBy
                        };
                        var attachments = BuildAttachments(finalFolder, oxpsPath);
                        int taskId = dt318_TasksBUS.Instance.Import(
                            task,
                            source.AssigneeId,
                            attachments);

                        if (taskId == 0)
                        {
                            Directory.Delete(finalFolder, true);
                            finalFolderCreated = false;
                            return Skipped(sourceFolder, "此資料夾已由其他使用者匯入。", null);
                        }
                        if (taskId < 0)
                            throw new InvalidOperationException("寫入資料庫失敗。");

                        return new Task318ImportItemResult
                        {
                            SourceFolderPath = sourceFolder,
                            DestinationFolderPath = finalFolder,
                            TaskId = taskId,
                            Imported = true,
                            Message = "匯入完成。"
                        };
                    }
                    catch
                    {
                        if (Directory.Exists(temporaryFolder))
                            Directory.Delete(temporaryFolder, true);
                        if (finalFolderCreated && Directory.Exists(finalFolder))
                            Directory.Delete(finalFolder, true);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                return Failed(sourceFolder, ex.Message);
            }
        }

        private static List<Task318ImportAttachment> BuildAttachments(
            string destinationFolder,
            string sourceOxpsPath)
        {
            string sourceOxpsName = Path.GetFileName(sourceOxpsPath);
            return Directory.EnumerateFiles(destinationFolder, "*", SearchOption.AllDirectories)
                .Select(path =>
                {
                    string relativePath = path.Substring(destinationFolder.Length)
                        .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                    return new Task318ImportAttachment
                    {
                        Attachment = new dm_Attachment
                        {
                            Thread = "318",
                            EncryptionName = relativePath,
                            ActualName = relativePath
                        },
                        AttachmentType = string.Equals(
                            Path.GetFileName(path),
                            sourceOxpsName,
                            StringComparison.OrdinalIgnoreCase)
                                ? "XpsNguon"
                                : "DinhKem"
                    };
                })
                .ToList();
        }

        private static DestinationReservation ReserveDestinationFolder(
            string commonRoot,
            DateTime dueDate,
            string ownerName)
        {
            string safeOwnerName = SanitizeFolderName(ownerName);
            string baseName = $"{dueDate:yyyyMMdd}_{safeOwnerName}";
            for (int index = 1; index <= 999; index++)
            {
                string folderName = index == 1 ? baseName : $"{baseName}_{index:00}";
                string finalPath = Path.Combine(commonRoot, folderName);
                string lockPath = Path.Combine(commonRoot, $".dt318_{folderName}.lock");
                if (Directory.Exists(finalPath) || File.Exists(lockPath)) continue;

                try
                {
                    var lockStream = new FileStream(
                        lockPath,
                        FileMode.CreateNew,
                        FileAccess.ReadWrite,
                        FileShare.None);
                    string temporaryPath = Path.Combine(
                        commonRoot,
                        $".{folderName}.importing_{Guid.NewGuid():N}");
                    return new DestinationReservation(
                        finalPath,
                        temporaryPath,
                        lockPath,
                        lockStream);
                }
                catch (IOException)
                {
                    // Máy khác vừa giữ tên này; thử số tiếp theo.
                }
            }

            throw new IOException("Không thể tạo tên thư mục công việc không trùng.");
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (string directory in Directory.EnumerateDirectories(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                string relative = directory.Substring(source.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                Directory.CreateDirectory(Path.Combine(destination, relative));
            }

            foreach (string file in Directory.EnumerateFiles(
                source,
                "*",
                SearchOption.AllDirectories))
            {
                string relative = file.Substring(source.Length)
                    .TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string target = Path.Combine(destination, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(target));
                File.Copy(file, target, false);
            }
        }

        private static string CreateSourceKey(string sourceFolder)
        {
            string normalized = Path.GetFullPath(sourceFolder)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToUpperInvariant();
            using (var sha256 = SHA256.Create())
                return string.Concat(sha256.ComputeHash(Encoding.UTF8.GetBytes(normalized))
                    .Select(value => value.ToString("x2")));
        }

        private static bool IsOxpsFile(string path)
        {
            string extension = Path.GetExtension(path);
            return string.Equals(extension, ".oxps", StringComparison.OrdinalIgnoreCase)
                || string.Equals(extension, ".xps", StringComparison.OrdinalIgnoreCase);
        }

        private static List<string> GetOxpsFiles(string folder)
        {
            return Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
                .Where(IsOxpsFile)
                .ToList();
        }

        private static string SanitizeFolderName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Unknown";
            var invalid = Path.GetInvalidFileNameChars();
            string result = new string(value.Trim()
                .Select(character => invalid.Contains(character) ? '_' : character)
                .ToArray());
            return string.IsNullOrWhiteSpace(result) ? "Unknown" : result;
        }

        private static void ValidateOptions(Task318ImportOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.CommonRootPath))
                throw new ArgumentException("尚未設定共用資料夾。", nameof(options));
            if (string.IsNullOrWhiteSpace(options.ImportedBy))
                throw new ArgumentException("尚未設定匯入人員。", nameof(options));
            if (options.Sources.Count == 0)
                throw new ArgumentException("尚未設定來源資料夾。", nameof(options));
            if (options.Sources.Any(source => source == null
                || string.IsNullOrWhiteSpace(source.RootPath)
                || string.IsNullOrWhiteSpace(source.AssigneeId)
                || string.IsNullOrWhiteSpace(source.FolderName)))
                throw new ArgumentException("來源資料夾設定不完整。", nameof(options));
        }

        private static Task318ImportItemResult Failed(string source, string message)
        {
            return new Task318ImportItemResult
            {
                SourceFolderPath = source,
                Message = message
            };
        }

        private static void AddResult(
            Task318ImportSummary summary,
            Task318ImportItemResult result,
            Task318ImportSource source)
        {
            if (string.IsNullOrWhiteSpace(result.AssigneeId))
                result.AssigneeId = source.AssigneeId;
            summary.Items.Add(result);
        }

        private static Task318ImportItemResult Skipped(
            string source,
            string message,
            string destination)
        {
            return new Task318ImportItemResult
            {
                SourceFolderPath = source,
                DestinationFolderPath = destination,
                Skipped = true,
                Message = message
            };
        }

        private sealed class DestinationReservation : IDisposable
        {
            private readonly string lockPath;
            private readonly FileStream lockStream;

            public DestinationReservation(
                string finalPath,
                string temporaryPath,
                string lockPath,
                FileStream lockStream)
            {
                FinalPath = finalPath;
                TemporaryPath = temporaryPath;
                this.lockPath = lockPath;
                this.lockStream = lockStream;
            }

            public string FinalPath { get; }
            public string TemporaryPath { get; }

            public void Dispose()
            {
                lockStream.Dispose();
                if (File.Exists(lockPath)) File.Delete(lockPath);
            }
        }
    }
}
