using System;
using Com.Danliris.Service.Inventory.Lib.Helpers;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentWasteProductionViewModel.ReceiptWaste
{
    public class GarmentReceiptWasteProductionItemViewModel : BasicViewModel
    {
        public string BCNo { get; set; }
        public string BCType { get; set; }
        public DateTimeOffset? BCDate { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }
        public double Quantity { get; set; }
        public string RONo { get; set; }
        public string Article { get; set; }
    }
}
