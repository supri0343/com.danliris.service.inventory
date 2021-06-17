using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesItemViewModel : BasicViewModel
    {
        public int StockId { get; set; }
        public UnitViewModel Unit { get; set; }
        public string PONo { get; set; }
        public double Quantity { get; set; }
        public UomViewModel Uom { get; set; }
        public ProductViewModel Product { get; set; }
        public double BasicPrice { get; set; }
    }
}
