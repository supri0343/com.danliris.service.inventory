using Com.Danliris.Service.Inventory.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.ViewModels.GarmentLeftoverWarehouse.ExpenditureFinishedGood
{
    public class GarmentLeftoverWarehouseExpenditureFinishedGoodItemViewModel : BasicViewModel
    {
        public int StockId { get; set; }
        public UnitViewModel Unit { get; set; }
        public string RONo { get; set; }
        public double ExpenditureQuantity { get; set; }
        public double StockQuantity { get; set; }
        public int FinishedGoodExpenditureId { get; set; }
        public LeftoverComodityViewModel LeftoverComodity { get; set; }
        public double BasicPrice { get; set; }
    }
}
