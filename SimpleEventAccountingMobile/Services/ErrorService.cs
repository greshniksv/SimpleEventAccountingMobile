using Serilog;
using SimpleEventAccountingMobile.Services.Interfaces;

namespace SimpleEventAccountingMobile.Services
{
    public class ErrorService(ILogger logger) : IErrorService
    {

        public event Action<string, string> OnErrorChanged;

        private (string Title, string Message) _currentError;

        public (string Title, string Message) CurrentError => _currentError;

        public void ShowError(Exception ex, string message)
        {
            string title = "Критическая ошибка";
            logger.Fatal(ex, message);
            _currentError = (title, $"{message} \nEx: {ex.Message}");
            OnErrorChanged?.Invoke(title, message);
        }

        public void ShowError(string message)
        {
            string title = "Ошибка";
            logger.Error(message);
            _currentError = (title, message);
            OnErrorChanged?.Invoke(title, message);
        }

        public void HideError()
        {
            _currentError = (string.Empty, string.Empty);
            OnErrorChanged?.Invoke(string.Empty, string.Empty);
        }
    }
}
