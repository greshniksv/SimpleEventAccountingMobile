namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IImportExportService
    {
        Task<bool> ExportDataAsync();
        Task<bool> ImportDataAsync();
    }
}
