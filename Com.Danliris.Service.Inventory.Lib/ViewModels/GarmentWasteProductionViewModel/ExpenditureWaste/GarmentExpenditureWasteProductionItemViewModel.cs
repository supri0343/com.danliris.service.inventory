using System;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ExpenditureWaste
{
    public class GarmentExpenditureWasteProductionItemViewModel : BasicViewModel
    {
        public string GarmentReceiptWasteNo { get; set; }
        public int GarmentReceiptWasteId { get; set; }
        public double Quantity { get; set; }
        
    }
}
