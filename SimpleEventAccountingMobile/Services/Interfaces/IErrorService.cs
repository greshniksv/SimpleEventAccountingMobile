namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IErrorService
    {
        event Action<string, string> OnErrorChanged;
        void ShowError(Exception ex, string message);
        void ShowError(string message);
        void HideError();
        (string Title, string Message) CurrentError { get; }
    }
}
