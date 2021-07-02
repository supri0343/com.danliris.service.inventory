using Com.Moonlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Danliris.Service.Inventory.Lib.Models.GarmentLeftoverWarehouse.BalanceStock
{
    public class GarmentLeftoverWarehouseBalanceStockItem : StandardEntity
    {
        public int BalanceStockId { get; set; }
        public GarmentLeftoverWarehouseBalanceStock GarmentLeftoverWarehouseBalanceStock { get; set; }

        public long UnitId { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }

        public string PONo { get; set; }

        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductRemark { get; set; }

        public string Yarn { get; set; }
        public string Width { get; set; }

        public string Composition { get; set; }
        public string Construction { get; set; }
        public double Quantity { get; set; }
        public string RONo { get; set; }

        public long UomId { get; set; }
        public string UomUnit { get; set; }

        public long LeftoverComodityId { get; set; }
        public string LeftoverComodityCode { get; set; }
        public string LeftoverComodityName { get; set; }

        public double BasicPrice { get; set; }
    }
}
