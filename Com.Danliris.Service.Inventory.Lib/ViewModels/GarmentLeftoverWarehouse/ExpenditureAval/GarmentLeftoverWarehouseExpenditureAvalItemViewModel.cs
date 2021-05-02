using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAval
{
    public class GarmentLeftoverWarehouseExpenditureAvalItemViewModel : BasicViewModel
    {
        public int StockId { get; set; }

        public UnitViewModel Unit { get; set; }
        public string AvalReceiptNo { get; set; }
        public int AvalReceiptId { get; set; }
        public double Quantity { get; set; }
        public double ActualQuantity { get; set; }
        public int AvalExpenditureId { get; set; }

        public double StockQuantity { get; set; }
        public UomViewModel Uom { get; set; }
        public ProductViewModel Product { get; set; }
    }
}
