using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BusinessLayer
{
    internal static class OffboardingFileStorage
    {
        internal sealed class StoredFile
        {
            public string OriginalName { get; set; }
            public string StoredPath { get; set; }
            public long FileSize { get; set; }
        }

        internal sealed class Batch : IDisposable
        {
            internal readonly string Folder;
            internal readonly List<string> CreatedPaths = new List<string>();
            internal readonly List<StoredFile> Files = new List<StoredFile>();
            private bool completed;

            internal Batch(string folder) { Folder = folder; }
            internal void Complete() { completed = true; }
            public void Dispose()
            {
                if (completed) return;
                foreach (string path in CreatedPaths)
                {
                    try { File.Delete(path); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }
                }
                try { if (Directory.Exists(Folder)) Directory.Delete(Folder, false); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
        }

        internal static Batch Prepare(string root, IEnumerable<string> sourceFiles)
        {
            var sources = sourceFiles.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            if (sources.Count == 0) throw new OffboardingException("請至少選擇一個附件。");
            foreach (string path in sources)
            {
                var info = new FileInfo(path);
                if (!info.Exists || info.Length == 0 || info.Length > EmployeeOffboardingService.MaxFileSize || info.Name.Length > 256)
                    throw new OffboardingException("附件必須存在且不可為空，每個檔案不得超過 100 MB，檔名不得超過 256 個字。");
            }
            string batchName = Guid.NewGuid().ToString("N");
            var batch = new Batch(Path.Combine(root, batchName));
            try
            {
                Directory.CreateDirectory(batch.Folder);
                foreach (string path in sources)
                {
                    string storedName = Guid.NewGuid().ToString("N") + Path.GetExtension(path);
                    string destination = Path.Combine(batch.Folder, storedName);
                    batch.CreatedPaths.Add(destination);
                    using (var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        if (input.Length == 0 || input.Length > EmployeeOffboardingService.MaxFileSize)
                            throw new OffboardingException("附件大小不符合限制。");
                        input.CopyTo(output);
                        batch.Files.Add(new StoredFile
                        {
                            OriginalName = Path.GetFileName(path), StoredPath = batchName + "/" + storedName,
                            FileSize = output.Length
                        });
                    }
                }
                return batch;
            }
            catch
            {
                batch.Dispose();
                throw;
            }
        }

        internal static string ResolvePath(string root, string storedPath)
        {
            root = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string source = Path.GetFullPath(Path.Combine(root, storedPath.Replace('/', Path.DirectorySeparatorChar)));
            if (!source.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                throw new OffboardingException("附件路徑無效。");
            return source;
        }

        internal static void CopyToDestination(string source, string destination)
        {
            if (!File.Exists(source)) throw new OffboardingException("附件不存在，請聯絡系統管理員。");
            string fullDestination = Path.GetFullPath(destination);
            if (string.Equals(Path.GetFullPath(source), fullDestination, StringComparison.OrdinalIgnoreCase))
                throw new OffboardingException("請選擇與原始附件不同的儲存位置。");
            string staging = Path.Combine(Path.GetDirectoryName(fullDestination), Guid.NewGuid().ToString("N") + ".tmp");
            try
            {
                File.Copy(source, staging, false);
                if (File.Exists(fullDestination)) File.Replace(staging, fullDestination, null);
                else File.Move(staging, fullDestination);
            }
            finally
            {
                if (File.Exists(staging)) File.Delete(staging);
            }
        }
    }
}
