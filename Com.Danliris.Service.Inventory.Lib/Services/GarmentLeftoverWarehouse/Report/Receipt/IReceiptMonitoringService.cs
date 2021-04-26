using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Receipt
{
    public interface IReceiptMonitoringService
    {
        Tuple<List<ReceiptMonitoringViewModel>, int> GetFabricReceiptMonitoring(DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order, int offset);

    }
}
