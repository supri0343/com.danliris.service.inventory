using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.BalanceReportPerGrade
{
    public interface  IBalanceReportPerPieceService
    {
        Task<Tuple<List<BalanceReportPerPieceViewModel>, int>> GetStockReportGreige(DateTime? dateTo, int offset, int page, int size, string Order);
        Task<MemoryStream> GenerateExcel(DateTime? dateTo, int offset);
    }
}
