using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseStockBookkeepingReportViewModel
    {

        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double BeginingbalanceQty { get; set; }
        public double BeginingbalancePrice { get; set; }
        public double QuantityReceipt { get; set; }
        public double PriceReceipt { get; set; }
        public double QuantityExpend { get; set; }
        public double PriceExpend { get; set; }
        public double EndbalanceQty { get; set; }
        public double EndbalancePrice { get; set; }
        public string UomUnit { get; set; }
        public double BasicPrice { get; set; }
    }
}
