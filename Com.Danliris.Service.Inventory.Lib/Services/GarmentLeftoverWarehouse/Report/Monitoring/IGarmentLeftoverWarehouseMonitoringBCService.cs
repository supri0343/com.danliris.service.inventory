using Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Monitoring;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentLeftoverWarehouse.Report.Monitoring
{
    public interface IGarmentLeftoverWarehouseMonitoringBCService
    {
        List<GarmentLeftoverWarehouseMonitoringViewModel> GetReportQuery(List<GarmentLeftoverWarehouseMonitoringParameterViewModel> param);
    }
}
