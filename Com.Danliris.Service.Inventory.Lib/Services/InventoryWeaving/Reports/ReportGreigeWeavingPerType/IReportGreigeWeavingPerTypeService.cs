using Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryWeavingViewModel.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Com.Danliris.Service.Inventory.Lib.Services.InventoryWeaving.Reports.ReportGreigeWeavingPerType
{
    public interface IReportGreigeWeavingPerTypeService
    {
        Tuple<List<ReportGreigeWeavingPerTypeViewModel>, int> GetStockReport(DateTime? dateTo, int offset, int page, int size, string Order);
        MemoryStream GenerateExcel(DateTime? dateTo, int offset);
    }
}
