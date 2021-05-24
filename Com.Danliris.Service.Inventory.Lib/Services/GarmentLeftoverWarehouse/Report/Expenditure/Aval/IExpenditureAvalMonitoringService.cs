using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Expenditure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Expenditure.Aval
{
    public interface IExpenditureAvalMonitoringService
    {
        Tuple<List<ExpenditureAvalMonitoringViewModel>, int> GetMonitoring(DateTime? dateFrom, DateTime? dateTo, string type, int page, int size, string Order, int offset);
        Tuple<MemoryStream, string> GenerateExcel(DateTime? dateFrom, DateTime? dateTo, string type, int offset);
    }
}
