using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Monitoring
{
    public class GarmentLeftoverWarehouseMonitoringViewModel
    {
        public string ReceiptNo { get; set; }
        public double ReceiptQty { get; set; }
        public string ExpenditureNo { get; set; }
        public double ExpenditureQty { get; set; }
        public string UENNo { get; set; }
        public string PO { get; set; }
    }
}
