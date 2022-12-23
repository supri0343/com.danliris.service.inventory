using System;
using System.Collections.Generic;
using System.Text;
using Com.Moonlay.Models;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentWasteProduction.ExpenditureWaste
{
    public class GarmentExpenditureWasteProductionItems : StandardEntity
    {
        public int GarmentExpenditureWasteId { get; set; }
        public string GarmentReceiptWasteNo { get; set; }
        public int GarmentReceiptWasteId { get; set; }
        public double Quantity { get; set; }
        

        public GarmentExpenditureWasteProductions GarmentExpenditureWasteProductions { get; set; }
    }
}
