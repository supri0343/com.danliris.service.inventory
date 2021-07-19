using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public interface IGarmentLeftoverWarehouseStockBookkeepingReportService
    {
        Tuple<List<GarmentLeftoverWarehouseStockBookkeepingReportViewModel>, int> GetReport(DateTime? dateTo, string type, int page, int size, string Order, int offset);
        Tuple<MemoryStream, string> GenerateExcel(DateTime? dateTo, string type, int offset);
    }
}
