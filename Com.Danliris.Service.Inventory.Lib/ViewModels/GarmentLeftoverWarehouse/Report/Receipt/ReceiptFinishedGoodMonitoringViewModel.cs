using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.Report.Receipt
{
    public class ReceiptFinishedGoodMonitoringViewModel
    {
        public int index { get; set; }
        public string ReceiptNoteNo { get; set; }
        public DateTimeOffset ReceiptDate { get; set; }

        public string UnitFromCode { get; set; }
        public string ExpenditureGoodNo { get; set; }
        public string RONo { get; set; }
        public string ComodityName { get; set; }
        public double Quantity { get; set; }
        public string UomUnit { get; set; }
    }
}
