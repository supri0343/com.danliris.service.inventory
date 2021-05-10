using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt
{
    public class ReceiptAvalMonitoringViewModel
    {
        public int index { get; set; }
        public string ReceiptNoteNo { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }
        public string AvalType { get; set; }
        public double Weight { get; set; }
        public string UomUnit { get; set; }
        public string Uom { get; set; }
        public string UnitCode { get; set; }
        public string RONo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Remark { get; set; }
        public double Quantity { get; set; }
        public string AvalComponentNo { get; set; }
        public long Id { get; set; }
    }
}
