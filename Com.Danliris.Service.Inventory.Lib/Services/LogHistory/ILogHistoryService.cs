using Com.Danliris.Service.Inventory.Lib.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.LogHistories
{
    public interface ILogHistoryService
    {
        void CreateAsync(string division, string activity);
        Task<Tuple<List<LogHistoryViewModel>, int>> GetMonitoring(DateTime dateFrom, DateTime dateTo, int page, int size);
        Task<MemoryStream> GenerateExcelReport(DateTime dateFrom, DateTime dateTo);
    }
}
