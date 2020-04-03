using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.GarmentLeftoverWarehouseReceiptAvalViewModels
{
    public class GarmentLeftoverWarehouseReceiptAvalItemViewModel : BasicViewModel
    {
        public int AvalReceiptId { get; set; }
        public Guid GarmentAvalProductId { get; set; }
        public string RONo { get; set; }
        public Guid GarmentAvalProductItemId { get; set; }
        public ProductViewModel Product { get; set; }
        public string ProductRemark { get; set; }

        public double Quantity { get; set; }

        public UomViewModel Uom { get; set; }
    }
}
