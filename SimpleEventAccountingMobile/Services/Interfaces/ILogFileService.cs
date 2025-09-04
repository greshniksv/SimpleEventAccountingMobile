namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface ILogFileService
    {
        string LogsDirectory { get; }
        List<FileInfo> GetLogFiles();
        Task<long> CountLinesInFile(string filePath);
        Task<List<string>> ReadLinesFromFile(string filePath, int startLine, int count);
        string FormatFileSize(long bytes);
        void EnsureLogsDirectoryExists();
    }
}
