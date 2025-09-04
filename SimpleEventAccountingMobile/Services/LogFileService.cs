using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class LogFileService : ILogFileService
    {
        public string LogsDirectory { get; private set; }

        public LogFileService()
        {
            LogsDirectory = Path.Combine(FileSystem.AppDataDirectory, "logs");
            EnsureLogsDirectoryExists();
        }

        public void EnsureLogsDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                {
                    Directory.CreateDirectory(LogsDirectory);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create logs directory: {ex.Message}", ex);
            }
        }

        public List<FileInfo> GetLogFiles()
        {
            try
            {
                if (!Directory.Exists(LogsDirectory))
                    return new List<FileInfo>();

                var directoryInfo = new DirectoryInfo(LogsDirectory);
                return directoryInfo.GetFiles("*.txt").ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get log files: {ex.Message}", ex);
            }
        }

        public async Task<long> CountLinesInFile(string filePath)
        {
            if (!File.Exists(filePath))
                return 0;

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                long lineCount = 0;
                while (await reader.ReadLineAsync() != null)
                {
                    lineCount++;
                }

                return lineCount;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to count lines in file: {ex.Message}", ex);
            }
        }

        public async Task<List<string>> ReadLinesFromFile(string filePath, int startLine, int count)
        {
            if (!File.Exists(filePath))
                return new List<string>();

            var lines = new List<string>();

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = new StreamReader(stream);

                int currentLine = 1;
                string? line;

                // Skip to the start line
                while (currentLine < startLine && (line = await reader.ReadLineAsync()) != null)
                {
                    currentLine++;
                }

                // Read the requested number of lines
                while (lines.Count < count && (line = await reader.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                    currentLine++;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to read lines from file: {ex.Message}", ex);
            }

            return lines;
        }

        public string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }
    }
}
