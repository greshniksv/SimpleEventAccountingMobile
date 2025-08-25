using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEventAccountingMobile.Services.Interfaces
{
    public interface IImportExportService
    {
        Task ExportAsync();

        Task ImportAsync();
    }
}
