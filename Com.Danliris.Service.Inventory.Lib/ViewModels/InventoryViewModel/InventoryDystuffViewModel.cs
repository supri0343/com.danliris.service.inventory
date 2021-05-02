using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.InventoryViewModel
{
    public class InventoryDystuffViewModel
    {
        public string ProductCode {get; set;}
        public string ProductName { get; set; }
        public string UomUnit { get; set; }
        public double BeginningQty { get; set; }
        public double ReceiptQty { get; set; }
        public double ExpandQty { get; set; }
        public double EndingQty { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Type { get; set; }
        
    }
}
