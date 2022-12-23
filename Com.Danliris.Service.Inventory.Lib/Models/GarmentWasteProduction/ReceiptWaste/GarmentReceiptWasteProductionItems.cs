using System;
using System.Collections.Generic;
using System.Text;
using Com.Moonlay.Models;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ReceiptWaste
{
    public class GarmentReceiptWasteProductionItems : StandardEntity
    {
        public int GarmentReceiptWasteId { get; set; }
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset? BCDate { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
        public GarmentReceiptWasteProductions GarmentReceiptWasteProductions { get; set; }
    }
}
