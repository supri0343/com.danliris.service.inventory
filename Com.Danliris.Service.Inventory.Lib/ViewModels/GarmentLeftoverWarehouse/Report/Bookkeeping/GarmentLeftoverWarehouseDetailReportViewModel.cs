using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Bookkeeping
{
    public class GarmentLeftoverWarehouseDetailReportViewModel
    {
        public string Description { get; set; }
        public string Uom { get; set; }
        public double Unit2aQty { get; set; }
        public double Unit2aPrice { get; set; }
        public double Unit2bQty { get; set; }
        public double Unit2bPrice { get; set; }
        public double Unit2cQty { get; set; }
        public double Unit2cPrice { get; set; }
        public double Unit1aQty { get; set; }
        public double Unit1aPrice { get; set; }
        public double Unit1bQty { get; set; }
        public double Unit1bPrice { get; set; }
        public double TotalQty { get; set; }
        public double TotalPrice { get; set; }
    }
}
