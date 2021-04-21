using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure
{
    public interface IGarmentLeftoverWarehouseReportExpenditureService
    {
        Tuple<List<GarmentLeftoverWarehouseReportExpenditureViewModel>, int> GetReport(DateTime? dateFrom, DateTime? dateTo, string receiptType, int page, int size, string Order, int offset);
        MemoryStream GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string receiptType, int offset);
    }
}
