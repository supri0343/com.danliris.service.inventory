using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.ExpenditureAccessories
{
    public class GarmentLeftoverWarehouseExpenditureAccessoriesItem : StandardEntity
    {
        public int ExpenditureId { get; set; }
        public GarmentLeftoverWarehouseExpenditureAccessories GarmentLeftoverWarehouseExpenditureAccessories { get; set; }

        public int StockId { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public string PONo { get; set; }
        public double Quantity { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }

        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public double BasicPrice { get; set; }
    }
}
