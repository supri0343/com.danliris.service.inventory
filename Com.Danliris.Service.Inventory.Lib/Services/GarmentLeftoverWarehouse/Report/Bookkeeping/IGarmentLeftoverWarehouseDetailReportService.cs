using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public interface IGarmentLeftoverWarehouseDetailReportService
    {
        Tuple<List<GarmentLeftoverWarehouseDetailReportViewModel>, int> GetMonitoringDetail(string category, DateTime? dateFrom, DateTime? dateTo,   int page, int size, string order, int offset);
        MemoryStream GenerateExcelDetail(string category, DateTime? dateFrom, DateTime? dateTo, int offset);
    }
}
