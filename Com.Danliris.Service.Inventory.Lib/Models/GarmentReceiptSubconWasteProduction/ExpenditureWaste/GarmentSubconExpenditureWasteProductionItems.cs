using System;
using System.Collections.Generic;
using System.Text;
using Com.Moonlay.Models;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentReceiptSubconWasteProduction.ExpenditureWaste
{
    public class GarmentSubconExpenditureWasteProductionItems : StandardEntity
    {
        public int GarmentExpenditureWasteId { get; set; }
        public string GarmentReceiptWasteNo { get; set; }
        public int GarmentReceiptWasteId { get; set; }
        public double Quantity { get; set; }
        

        public GarmentSubconExpenditureWasteProductions GarmentSubconExpenditureWasteProductions { get; set; }
    }
}
