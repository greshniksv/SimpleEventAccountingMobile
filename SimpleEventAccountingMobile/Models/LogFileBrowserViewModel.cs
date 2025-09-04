using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Models
{
    // ViewModels/LogFileBrowserViewModel.cs
    public class LogFileBrowserViewModel
    {
        private readonly ILogFileService _logFileService;

        public List<FileInfo> Files { get; private set; } = new();
        public string? SelectedFileName { get; set; }
        public List<string> FileContent { get; private set; } = new();
        public int CurrentLineNumber { get; private set; }
        public long TotalLinesInFile { get; private set; }
        public string ErrorMessage { get; private set; } = string.Empty;
        public string LogsDirectory => _logFileService.LogsDirectory;

        public event Action? OnStateChanged;

        public LogFileBrowserViewModel(ILogFileService logFileService)
        {
            _logFileService = logFileService;
        }

        public void LoadLogFiles()
        {
            try
            {
                ErrorMessage = string.Empty;
                Files = _logFileService.GetLogFiles();
                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                Files = new List<FileInfo>();
                NotifyStateChanged();
            }
        }

        public async Task ReadFile()
        {
            if (string.IsNullOrEmpty(SelectedFileName))
                return;

            try
            {
                ErrorMessage = string.Empty;
                var filePath = Path.Combine(_logFileService.LogsDirectory, SelectedFileName);

                if (!File.Exists(filePath))
                {
                    ErrorMessage = "File not found!";
                    NotifyStateChanged();
                    return;
                }

                TotalLinesInFile = await _logFileService.CountLinesInFile(filePath);
                CurrentLineNumber = 1;
                FileContent = await _logFileService.ReadLinesFromFile(filePath, CurrentLineNumber, 100);

                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                FileContent = new List<string>();
                NotifyStateChanged();
            }
        }

        public async Task ReadNext()
        {
            if (string.IsNullOrEmpty(SelectedFileName) || !HasMoreLines())
                return;

            try
            {
                ErrorMessage = string.Empty;
                var filePath = Path.Combine(_logFileService.LogsDirectory, SelectedFileName);

                CurrentLineNumber += 100;
                FileContent = await _logFileService.ReadLinesFromFile(filePath, CurrentLineNumber, 100);

                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                NotifyStateChanged();
            }
        }

        public async Task ReadPrevious()
        {
            if (string.IsNullOrEmpty(SelectedFileName) || !HasPreviousLines())
                return;

            try
            {
                ErrorMessage = string.Empty;
                var filePath = Path.Combine(_logFileService.LogsDirectory, SelectedFileName);

                CurrentLineNumber = Math.Max(1, CurrentLineNumber - 200);
                FileContent = await _logFileService.ReadLinesFromFile(filePath, CurrentLineNumber, 100);

                NotifyStateChanged();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                NotifyStateChanged();
            }
        }

        public async Task ReloadFile()
        {
            if (string.IsNullOrEmpty(SelectedFileName))
                return;

            await ReadFile();
        }

        public void ClearContent()
        {
            FileContent = new List<string>();
            CurrentLineNumber = 0;
            TotalLinesInFile = 0;
            ErrorMessage = string.Empty;
            NotifyStateChanged();
        }

        public void ClearSelection()
        {
            SelectedFileName = null;
            ClearContent();
        }

        public bool HasMoreLines() => CurrentLineNumber + 100 <= TotalLinesInFile;
        public bool HasPreviousLines() => CurrentLineNumber > 1;

        public string GetFileDisplayName(FileInfo file)
        {
            return $"{file.Name} ({file.LastWriteTime:yyyy-MM-dd HH:mm}) - {_logFileService.FormatFileSize(file.Length)}";
        }

        private void NotifyStateChanged()
        {
            OnStateChanged?.Invoke();
        }
    }
}
