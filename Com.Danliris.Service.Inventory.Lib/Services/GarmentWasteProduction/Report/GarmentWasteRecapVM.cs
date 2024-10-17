using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Services.GarmentWasteProduction.Report
{
    public class GarmentWasteRecapVM
    {
        public string BonNo { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ReceiptSource { get; set; }
        public double Quantity { get; set; }
        public string Uom { get; set; }
        public string Remark { get; set; }
    }
}
