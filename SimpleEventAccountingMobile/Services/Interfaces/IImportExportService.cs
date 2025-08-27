namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IImportExportService
    {
        Task<string> ExportDataAsync();
    }
}
